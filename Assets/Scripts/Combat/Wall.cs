using UnityEngine;
using TMPro;

public class Wall : MonoBehaviour
{
    public float maxHealth = 500f;
    private float currentHealth;

    [Header("UI Elements")]
    public TMP_Text wallHealthText;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log("Wall destroyed!");
            // Optional: Defeat logic here
        }
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        wallHealthText.text = $"Wall HP: {currentHealth} / {maxHealth}";
    }
}
