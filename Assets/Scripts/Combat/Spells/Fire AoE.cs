using UnityEngine;

public class FireAoE : SpellBase
{
    public BurnEffect burnEffect;
    public override float GetRange() => 2f; // Width for visualization

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            burnEffect.Apply(other.gameObject);
        }
    }
}
