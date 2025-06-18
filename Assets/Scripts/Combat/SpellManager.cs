using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpellManager : MonoBehaviour
{
    public enum Element { Fire, Water, Wind, Earth }
    public enum Effect { Single, AoE, Utility, Defense }

    private Element? selectedElement = null;
    private Effect? selectedEffect = null;

    public GameObject spellPrefab;

    public TextMeshProUGUI elementNumberText;
    public TextMeshProUGUI effectNumberText;

    private Color readyColor = Color.green;
    private Color defaultColor = Color.white;

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

    private void UpdateNumberColors()
    {
        bool ready = selectedElement.HasValue && selectedEffect.HasValue;
        Color color = ready ? readyColor : defaultColor;

        elementNumberText.color = color;
        effectNumberText.color = color;
    }

    void Update()
    {
        if (selectedElement.HasValue && selectedEffect.HasValue && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                Vector3 spawnPos = hit.point;
                spawnPos.y = 0; // Ensure spells always at Y = 0
                CastSpell(spawnPos);
                selectedElement = null;
                selectedEffect = null;

                elementNumberText.text = "0";
                effectNumberText.text = "0";
                UpdateNumberColors();
            }
        }
    }

    void CastSpell(Vector3 position)
    {
        GameObject spell = Instantiate(spellPrefab, position, Quaternion.identity);
        SpriteRenderer sr = spell.GetComponent<SpriteRenderer>();

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

        spell.transform.localScale = Vector3.one * size;
        Destroy(spell, 2f);
    }
}
