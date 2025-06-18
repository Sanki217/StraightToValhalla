using UnityEngine;

public class SpellEffect : MonoBehaviour
{
    public int damage = 10;
    public float tickInterval = 0.2f; // Jak często sprawdza wrogów (np. co 0.2 sekundy)

    private float nextTick = 0f;

    private void Update()
    {
        if (Time.time >= nextTick)
        {
            DealDamage();
            nextTick = Time.time + tickInterval;
        }
    }

    private void DealDamage()
    {
        float radius = transform.localScale.x / 2f; // Zakładamy, że plane ma scale = średnica koła
        Collider[] hits = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider col in hits)
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }
}
