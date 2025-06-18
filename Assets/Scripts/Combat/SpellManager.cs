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
        public GameObject prefab;  // Now unique prefab per spell
        public int manaCost = 10;
        public int damage = 10;
    }

    public TextMeshProUGUI elementNumberText;
    public TextMeshProUGUI effectNumberText;
    public TextMeshProUGUI manaText;
    public TextMeshProUGUI[] cooldownTexts = new TextMeshProUGUI[4]; // 0=Single, 1=AoE, etc.

    private Element? selectedElement = null;
    private Effect? selectedEffect = null;

    private Color readyColor = Color.green;
    private Color defaultColor = Color.white;

    public int maxMana = 100;
    private int currentMana;

    public float manaRegenRate = 5f; // Mana per second

    public List<SpellData> spellPrefabs = new List<SpellData>(); // 16 entries → assigned in Inspector
    private Dictionary<(Element, Effect), SpellData> spellBook = new Dictionary<(Element, Effect), SpellData>();

    private float[] effectCooldownTimers = new float[4];
    public float[] effectCooldownDurations = new float[4] { 1f, 2f, 3f, 5f };

    private void Start()
    {
        currentMana = maxMana;
        UpdateManaUI();

        // Assign all 16 spells → Match order manually in Inspector or write helper if needed
        int i = 0;
        foreach (Element elem in System.Enum.GetValues(typeof(Element)))
        {
            foreach (Effect eff in System.Enum.GetValues(typeof(Effect)))
            {
                spellBook[(elem, eff)] = spellPrefabs[i];
                i++;
            }
        }
    }

    private void Update()
    {
        UpdateCooldowns();
        RegenerateMana();

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
                        Instantiate(spell.prefab, spawnPos, Quaternion.identity);
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
            currentMana += Mathf.CeilToInt(manaRegenRate * Time.deltaTime);
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
        manaText.text = $"Mana: {currentMana}/{maxMana}";
    }

    public void UpgradeSpell(Element element, Effect effect, int manaCostReduction, int damageIncrease)
    {
        var key = (element, effect);
        if (spellBook.ContainsKey(key))
        {
            spellBook[key].manaCost = Mathf.Max(1, spellBook[key].manaCost - manaCostReduction);
            spellBook[key].damage += damageIncrease;
        }
    }
}
