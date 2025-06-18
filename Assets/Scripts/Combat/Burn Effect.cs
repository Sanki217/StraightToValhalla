using UnityEngine;

public class BurnEffect : MonoBehaviour
{
    public float burnDuration = 3f;
    public float tickDamage = 1f;
    public float tickInterval = 1f;

    public void Apply(GameObject enemy)
    {
        if (!enemy.TryGetComponent(out BurnStatus existingBurn))
        {
            var burn = enemy.AddComponent<BurnStatus>();
            burn.Initialize(burnDuration, tickDamage, tickInterval);
        }
    }
}

public class BurnStatus : MonoBehaviour
{
    float duration, damage, interval, timer;

    public void Initialize(float dur, float dmg, float inter)
    {
        duration = dur;
        damage = dmg;
        interval = inter;
        timer = interval;
        StartCoroutine(BurnCoroutine());
    }

    IEnumerator BurnCoroutine()
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (timer >= interval)
            {
                // Apply damage (implement EnemyHealth elsewhere)
                GetComponent<EnemyHealth>().TakeDamage(damage);
                timer = 0f;
            }
            timer += Time.deltaTime;
            elapsed += Time.deltaTime;
            yield return null;
        }
        Destroy(this);
    }
}
