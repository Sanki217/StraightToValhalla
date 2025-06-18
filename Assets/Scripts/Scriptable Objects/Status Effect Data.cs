using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffectData", menuName = "Game/StatusEffectData")]
public class StatusEffectData : ScriptableObject
{
    public StatusEffectType type;
    public float baseDuration;
    public float tickDamage;
    public float tickInterval;

    // Add upgrade multipliers if you want
    public float durationMultiplier = 1f;
    public float damageMultiplier = 1f;

    public StatusEffectInstance CreateInstance()
    {
        return new StatusEffectInstance
        {
            type = type,
            timer = baseDuration * durationMultiplier,
            magnitude = 1f,
            tickDamage = tickDamage * damageMultiplier,
            tickInterval = tickInterval
        };
    }
}
