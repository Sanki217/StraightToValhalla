using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    public float moveSpeed = 2f;
    public float damage = 10f;
    public float attackSpeed = 1f;
    public float attackRange = 1.5f;

    [Header("Health UI")]
    public GameObject healthBarCanvas;
    public Image healthBarFill;

    [Header("Visuals")]
    public Renderer enemyRenderer;
    public Color normalColor = Color.white;
    public Color burnColor = Color.red;

    private float attackCooldown = 0f;
    private Transform wallTransform;
    private Wall wallScript;

    private Dictionary<StatusEffectType, StatusEffectInstance> activeEffects = new();

    private void Start()
    {
        currentHealth = maxHealth;
        healthBarCanvas.SetActive(false);

        if (enemyRenderer == null) enemyRenderer = GetComponentInChildren<Renderer>();
        enemyRenderer.material.color = normalColor;

        GameObject wallObj = GameObject.FindWithTag("Wall");
        if (wallObj != null)
        {
            wallTransform = wallObj.transform;
            wallScript = wallObj.GetComponent<Wall>();
        }
    }

    private void Update()
    {
        UpdateStatusEffects();

        if (wallTransform == null) return;

        float distanceToWall = Vector3.Distance(transform.position, wallTransform.position);

        if (distanceToWall > attackRange)
        {
            float finalSpeed = moveSpeed * GetSlowMultiplier();
            transform.position += Vector3.left * finalSpeed * Time.deltaTime;
        }
        else
        {
            attackCooldown -= Time.deltaTime;
            if (attackCooldown <= 0f)
            {
                wallScript.TakeDamage(damage);
                attackCooldown = 1f / attackSpeed;
            }
        }

        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (activeEffects.ContainsKey(StatusEffectType.Burn))
        {
            enemyRenderer.material.color = burnColor;
        }
        else
        {
            enemyRenderer.material.color = normalColor;
        }
    }

    private float GetSlowMultiplier()
    {
        if (activeEffects.TryGetValue(StatusEffectType.Slow, out StatusEffectInstance slow))
        {
            return 1f - slow.magnitude;
        }
        return 1f;
    }

    private void UpdateStatusEffects()
    {
        List<StatusEffectType> toRemove = new();

        foreach (var kvp in activeEffects)
        {
            StatusEffectInstance effect = kvp.Value;
            effect.timer -= Time.deltaTime;

            switch (effect.type)
            {
                case StatusEffectType.Burn:
                    if (effect.tickTimer <= 0f)
                    {
                        TakeDamage(effect.magnitude * effect.tickDamage);
                        effect.tickTimer = effect.tickInterval;
                    }
                    else
                    {
                        effect.tickTimer -= Time.deltaTime;
                    }
                    break;
            }

            if (effect.timer <= 0)
                toRemove.Add(kvp.Key);
        }

        foreach (var key in toRemove)
        {
            activeEffects.Remove(key);
        }
    }

    public void ApplyStatusEffect(StatusEffectInstance newEffect)
    {
        if (activeEffects.ContainsKey(newEffect.type))
        {
            activeEffects[newEffect.type].timer = newEffect.timer;
        }
        else
        {
            activeEffects[newEffect.type] = newEffect;
        }
    }

    public void TakeDamage(float dmg)
    {
        currentHealth -= dmg;
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            healthBarCanvas.SetActive(true);
            healthBarFill.fillAmount = currentHealth / maxHealth;
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
