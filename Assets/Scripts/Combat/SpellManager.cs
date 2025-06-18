using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SpellManager : MonoBehaviour
{
    public enum Element { Fire, Water, Wind, Earth }
    public enum Effect { Single, AoE, Utility, Ultimate }

    [System.Serializable]
    public class SpellData
    {
        public GameObject prefab;
        public int manaCost = 10;
        public int damage = 10;
        public float range = 1f; // NEW: range for indicator
        public float lifetime = 3f; // NEW: lifetime for auto-destroy
    }

    public TextMeshProUGUI elementNumberText;
    public TextMeshProUGUI effectNumberText;
    public TextMeshProUGUI manaText;
    public TextMeshProUGUI[] cooldownTexts = new TextMeshProUGUI[4];

    private Element? selectedElement = null;
    private Effect? selectedEffect = null;

    private Color readyColor = Color.green;
    private Color defaultColor = Color.white;

    public int maxMana = 100;
    private float currentMana; // Change to float for smoother regen
    public float manaRegenRate = 5f; // Mana per second

    public List<SpellData> spellPrefabs = new List<SpellData>();
    private Dictionary<(Element, Effect), SpellData> spellBook = new Dictionary<(Element, Effect), SpellData>();

    private float[] effectCooldownTimers = new float[4];
    public float[] effectCooldownDurations = new float[4] { 1f, 2f, 3f, 5f };

    [Header("Range Indicator")]
    public GameObject rangeIndicatorPrefab; // Assign flat circle prefab in Inspector
    private GameObject activeRangeIndicator;

    private void Start()
    {
        currentMana = maxMana;
        UpdateManaUI();

        // Load spells into dictionary
        int i = 0;
        foreach (Element elem in System.Enum.GetValues(typeof(Element)))
        {
            foreach (Effect eff in System.Enum.GetValues(typeof(Effect)))
            {
                spellBook[(elem, eff)] = spellPrefabs[i];
                i++;
            }
        }

        activeRangeIndicator = Instantiate(rangeIndicatorPrefab);
        activeRangeIndicator.SetActive(false);
    }

    private void Update()
    {
        UpdateCooldowns();
        RegenerateMana();
        HandleCasting();
        HandleRangeIndicator();
    }

    private void HandleCasting()
    {
        if (Input.GetMouseButtonDown(1)) // Right Click Reset
        {
            ResetSelection();
        }

        if (selectedElement.HasValue && selectedEffect.HasValue && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                Vector3 spawnPos = hit.point;
                spawnPos.y = 0;

                Effect effect = selectedEffect.Value;
                if (effectCooldownTimers[(int)effect] <= 0)
                {
                    var key = (selectedElement.Value, effect);
                    SpellData spell = spellBook[key];

                    if (currentMana >= spell.manaCost)
                    {
                        GameObject spellGO = Instantiate(spell.prefab, spawnPos, Quaternion.identity);
                        SpellEffect effectScript = spellGO.GetComponent<SpellEffect>();
                        if (effectScript != null)
                        {
                            effectScript.damage = spell.damage;
                        }
                        Destroy(spellGO, spell.lifetime); // Auto-destroy after X seconds

                        currentMana -= spell.manaCost;
                        UpdateManaUI();
                        effectCooldownTimers[(int)effect] = effectCooldownDurations[(int)effect];

                        ResetSelection();
                    }
                    else
                    {
                        Debug.Log("Not enough mana!");
                    }
                }
            }
        }
    }

    private void RegenerateMana()
    {
        if (currentMana < maxMana)
        {
            currentMana += manaRegenRate * Time.deltaTime;
            if (currentMana > maxMana) currentMana = maxMana;
            UpdateManaUI();
        }
    }

    private void UpdateCooldowns()
    {
        for (int i = 0; i < 4; i++)
        {
            if (effectCooldownTimers[i] > 0)
                effectCooldownTimers[i] -= Time.deltaTime;

            cooldownTexts[i].text = effectCooldownTimers[i] > 0 ? Mathf.Ceil(effectCooldownTimers[i]).ToString() : "";
        }
    }

    private void HandleRangeIndicator()
    {
        if (selectedElement.HasValue && selectedEffect.HasValue)
        {
            var key = (selectedElement.Value, selectedEffect.Value);
            SpellData spell = spellBook[key];

            Effect effect = selectedEffect.Value;

            if (currentMana >= spell.manaCost && effectCooldownTimers[(int)effect] <= 0)
            {
                activeRangeIndicator.SetActive(true);
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit, 100f))
                {
                    Vector3 pos = hit.point;
                    pos.y = 0;
                    activeRangeIndicator.transform.position = pos;
                    activeRangeIndicator.transform.localScale = new Vector3(spell.range, 1f, spell.range);
                }
            }
            else
            {
                activeRangeIndicator.SetActive(false);
            }
        }
        else
        {
            activeRangeIndicator.SetActive(false);
        }
    }


    public void SelectElement(int index)
    {
        selectedElement = (Element)index;
        elementNumberText.text = (index + 1).ToString();
        UpdateNumberColors();
    }

    public void SelectEffect(int index)
    {
        selectedEffect = (Effect)index;
        effectNumberText.text = (index + 1).ToString();
        UpdateNumberColors();
    }

    private void ResetSelection()
    {
        selectedElement = null;
        selectedEffect = null;
        elementNumberText.text = "0";
        effectNumberText.text = "0";
        UpdateNumberColors();
        activeRangeIndicator.SetActive(false);
    }

    private void UpdateNumberColors()
    {
        bool ready = selectedElement.HasValue && selectedEffect.HasValue;
        Color color = ready ? readyColor : defaultColor;

        elementNumberText.color = color;
        effectNumberText.color = color;
    }

    private void UpdateManaUI()
    {
        manaText.text = $"Mana: {Mathf.FloorToInt(currentMana)}/{maxMana}";
    }
}
