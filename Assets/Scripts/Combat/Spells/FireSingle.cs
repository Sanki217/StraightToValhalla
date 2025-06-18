using UnityEngine;

public class FireSingle : MonoBehaviour
{
    [Header("Spell Settings")]
    public float baseDamage = 20f;
    public StatusEffectType statusEffectType = StatusEffectType.Burn;

    [Header("Radius Settings")]
    public SphereCollider radiusCollider;         // Assign in Inspector
    public float scaleMultiplier = 1f;

    [Header("Range Indicator")]
    public GameObject rangeIndicator;             // Flat circle prefab, same shape as collider

    private void Awake()
    {
        if (radiusCollider == null) Debug.LogError("Radius Collider not assigned!");
        if (rangeIndicator != null) rangeIndicator.SetActive(true);
       
    }

    private void Start()
    {
        ApplyEffectsToEnemies();
        if (rangeIndicator != null) rangeIndicator.SetActive(false);
        Destroy(gameObject, 0.1f);   // Instant cast
    }

    private void ApplyEffectsToEnemies()
    {
        Vector3 center = radiusCollider.bounds.center;
        float effectiveRadius = radiusCollider.bounds.extents.x;

        Collider[] hitColliders = Physics.OverlapSphere(center, effectiveRadius);

        foreach (Collider hit in hitColliders)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(baseDamage);

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
