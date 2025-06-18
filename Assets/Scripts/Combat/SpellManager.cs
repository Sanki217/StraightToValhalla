using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SpellManager : MonoBehaviour
{
    public enum Element { Fire, Water, Wind, Earth }
    public enum Effect { Single, AoE, Utility, Defense }

    [System.Serializable]
    public class SpellData
    {
        public int manaCost = 10;
        public int damage = 10;
        // Add other effect-specific properties later
    }

    public GameObject spellPrefab;
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

    private Dictionary<(Element, Effect), SpellData> spellBook = new Dictionary<(Element, Effect), SpellData>();
    private float[] effectCooldownTimers = new float[4]; // Cooldowns for Single, AoE, Utility, Defense
    public float[] effectCooldownDurations = new float[4] { 1f, 2f, 3f, 5f }; // Example: 1s Single, 2s AoE, etc.

    private void Start()
    {
        currentMana = maxMana;
        UpdateManaUI();

        // Initialize all 16 spells with default values (can be customized)
        foreach (Element elem in System.Enum.GetValues(typeof(Element)))
        {
            foreach (Effect eff in System.Enum.GetValues(typeof(Effect)))
            {
                spellBook[(elem, eff)] = new SpellData()
                {
                    manaCost = Random.Range(5, 21), // Example: random mana cost 5-20
                    damage = Random.Range(10, 31),  // Example: random damage 10-30
                };
            }
        }
    }

    private void Update()
    {
        UpdateCooldowns();

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
                        CastSpell(spawnPos, spell);
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

    private void CastSpell(Vector3 position, SpellData spell)
    {
        GameObject s = Instantiate(spellPrefab, position, Quaternion.identity);
        SpriteRenderer sr = s.GetComponent<SpriteRenderer>();

        sr.color = selectedElement switch
        {
            Element.Fire => Color.red,
            Element.Water => Color.blue,
            Element.Wind => Color.green,
            Element.Earth => new Color(0.5f, 0.25f, 0f),
            _ => Color.white,
        };

        float size = selectedEffect switch
        {
            Effect.Single => 0.5f,
            Effect.AoE => 2f,
            Effect.Utility => 1f,
            Effect.Defense => 1.5f,
            _ => 1f,
        };

        s.transform.localScale = Vector3.one * size;
        Destroy(s, 2f);
        Debug.Log($"Cast spell: {selectedElement} {selectedEffect} | Damage: {spell.damage} | ManaCost: {spell.manaCost}");
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

    // Example of upgrading a single spell:
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
