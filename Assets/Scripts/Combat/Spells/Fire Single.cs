using UnityEngine;

public class FireSingle : SpellBase
{
    public float radius = 1f;
    public BurnEffect burnEffect;

    public override float GetRange() => radius;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyHealth>().TakeDamage(damage);
            burnEffect.Apply(other.gameObject);
        }
    }
}
