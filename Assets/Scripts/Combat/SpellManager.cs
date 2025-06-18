using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class SpellManager : MonoBehaviour
{
    public enum Element { Fire, Water, Wind, Earth }
    public enum Effect { Single, AoE, Utility, Ultimate }

    [System.Serializable]
    public class SpellData
    {
        public GameObject prefab; // Prefab WITH RangeIndicator child inside
        public int manaCost = 10;
        public float lifetime = 3f;
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
    private float currentMana;
    public float manaRegenRate = 5f;

    public List<SpellData> spellPrefabs = new List<SpellData>();
    private Dictionary<(Element, Effect), SpellData> spellBook = new Dictionary<(Element, Effect), SpellData>();

    private float[] effectCooldownTimers = new float[4];
    public float[] effectCooldownDurations = new float[4] { 1f, 2f, 3f, 5f };

    private GameObject previewInstance;

    private void Start()
    {
        currentMana = maxMana;
        UpdateManaUI();

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
        HandleCasting();
        HandleRangePreview();
    }

    private void HandleCasting()
    {
        if (Input.GetMouseButtonDown(1)) ResetSelection();

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
                        // CAST REAL SPELL
                        GameObject spellGO = Instantiate(spell.prefab, spawnPos, Quaternion.identity);
                        spellGO.SetActive(true);

                        currentMana -= spell.manaCost;
                        UpdateManaUI();
                        effectCooldownTimers[(int)effect] = effectCooldownDurations[(int)effect];

                        ResetSelection();
                    }
                }
            }
        }
    }

    private void HandleRangePreview()
    {
        if (selectedElement.HasValue && selectedEffect.HasValue)
        {
            var key = (selectedElement.Value, selectedEffect.Value);
            SpellData spell = spellBook[key];

            if (currentMana >= spell.manaCost && effectCooldownTimers[(int)selectedEffect.Value] <= 0)
            {
                if (previewInstance == null)
                {
                    previewInstance = Instantiate(spell.prefab);
                    previewInstance.SetActive(true);

                    // Disable collider & spell logic
                    foreach (Collider col in previewInstance.GetComponentsInChildren<Collider>()) col.enabled = false;
                    foreach (MonoBehaviour script in previewInstance.GetComponentsInChildren<MonoBehaviour>()) script.enabled = false;

                    // Enable ONLY the RangeIndicator (assumes named "RangeIndicator")
                    Transform rangeIndicator = previewInstance.transform.Find("RangeIndicator");
                    if (rangeIndicator != null) rangeIndicator.gameObject.SetActive(true);
                }

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, 100f))
                {
                    Vector3 pos = hit.point;
                    pos.y = 0;
                    previewInstance.transform.position = pos;
                }
            }
            else
            {
                ClearPreviewInstance();
            }
        }
        else
        {
            ClearPreviewInstance();
        }
    }

    private void ClearPreviewInstance()
    {
        if (previewInstance != null) Destroy(previewInstance);
        previewInstance = null;
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
        for (int i = 0; i < effectCooldownTimers.Length; i++)
        {
            if (effectCooldownTimers[i] > 0)
            {
                effectCooldownTimers[i] -= Time.deltaTime;
                cooldownTexts[i].text = Mathf.CeilToInt(effectCooldownTimers[i]).ToString();
                cooldownTexts[i].color = defaultColor;
            }
            else
            {
                cooldownTexts[i].text = "0";
                cooldownTexts[i].color = readyColor;
            }
        }
    }

    private void UpdateManaUI()
    {
        manaText.text = Mathf.CeilToInt(currentMana).ToString();
    }

    private void ResetSelection()
    {
        selectedElement = null;
        selectedEffect = null;
        elementNumberText.text = "-";
        effectNumberText.text = "-";
        ClearPreviewInstance();
    }

    public void SelectElement(int index)
    {
        selectedElement = (Element)index;
        elementNumberText.text = (index + 1).ToString();
    }

    public void SelectEffect(int index)
    {
        selectedEffect = (Effect)index;
        effectNumberText.text = (index + 1).ToString();
    }
}
