using UnityEngine;
using System.Collections;

public class FireAoE : MonoBehaviour
{
    [Header("Spell Settings")]
    public float spellDuration = 5f;      // How long the spell stays active
    public float tickInterval = 1f;       // Time between damage ticks
    public float tickDamage = 5f;         // Damage dealt per tick
    public StatusEffectType statusEffectType = StatusEffectType.Burn;

    [Header("Collision")]
    public Collider areaCollider;         // Assign manually in prefab, BoxCollider, SphereCollider, etc.
    
    [Header("Range Indicator")]
    public GameObject rangeIndicator;     // Assign manually in prefab (use any shape you like)

    private void Awake()
    {

        if (rangeIndicator != null)
        {
            rangeIndicator.SetActive(true);
        }
    }

    private void Start()
    {
        ApplyEffectsToEnemiesInArea();
        if (rangeIndicator != null) rangeIndicator.SetActive(false);
        StartCoroutine(DamageOverTimeCoroutine());
        Destroy(gameObject, spellDuration);
    }

    private IEnumerator DamageOverTimeCoroutine()
    {
        float elapsed = 0f;

        yield return new WaitForSeconds(0.1f); // slight delay to avoid instant kill feel

        while (elapsed < spellDuration)
        {
            ApplyEffectsToEnemiesInArea();
            yield return new WaitForSeconds(tickInterval);
            elapsed += tickInterval;
        }

        if (rangeIndicator != null)
        {
            rangeIndicator.SetActive(false);
        }
    }

    private void ApplyEffectsToEnemiesInArea()
    {
        Collider[] hitColliders;

        // Determine collider type and use correct Overlap method
        if (areaCollider is BoxCollider box)
        {
            Vector3 center = box.bounds.center;
            Vector3 halfExtents = box.bounds.extents;
            hitColliders = Physics.OverlapBox(center, halfExtents, box.transform.rotation);
        }
        else if (areaCollider is SphereCollider sphere)
        {
            Vector3 center = sphere.bounds.center;
            float radius = sphere.bounds.extents.x; // Sphere should have equal extents
            hitColliders = Physics.OverlapSphere(center, radius);
        }
        else
        {
            Debug.LogError("Unsupported collider type for AoE spell.");
            return;
        }

        foreach (Collider hit in hitColliders)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(tickDamage);

                StatusEffectData statusEffectData = StatusEffectManager.Instance.GetStatusEffect(statusEffectType);
                if (statusEffectData != null)
                {
                    StatusEffectInstance effectInstance = statusEffectData.CreateInstance();
                    enemy.ApplyStatusEffect(effectInstance);
                }
            }
        }
    }

}
