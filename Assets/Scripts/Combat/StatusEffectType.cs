using UnityEngine;

public enum StatusEffectType
{
    Burn,
    Slow,
    Freeze,
    // Add more as needed
}

public class StatusEffectInstance
{
    public StatusEffectType type;
    public float timer; // Total duration remaining
    public float magnitude; // Strength of effect (e.g., slow percentage or burn multiplier)

    // For damage-over-time (Burn, Poison, etc.)
    public float tickDamage;
    public float tickInterval;
    public float tickTimer;
}
