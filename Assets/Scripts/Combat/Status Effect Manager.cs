using UnityEngine;
using System.Collections.Generic;

public class StatusEffectManager : MonoBehaviour
{
    public static StatusEffectManager Instance { get; private set; }

    [Header("Status Effects Registry")]
    public List<StatusEffectData> statusEffects = new List<StatusEffectData>();

    private Dictionary<StatusEffectType, StatusEffectData> statusEffectDict;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Initialize dictionary for fast lookup
        statusEffectDict = new Dictionary<StatusEffectType, StatusEffectData>();
        foreach (var effect in statusEffects)
        {
            if (!statusEffectDict.ContainsKey(effect.type))
            {
                statusEffectDict.Add(effect.type, effect);
            }
            else
            {
                Debug.LogWarning($"Duplicate status effect type detected: {effect.type}");
            }
        }
    }

    public StatusEffectData GetStatusEffect(StatusEffectType type)
    {
        if (statusEffectDict.TryGetValue(type, out StatusEffectData data))
            return data;

        Debug.LogWarning($"StatusEffect of type {type} not found!");
        return null;
    }

    // Example method for upgrading globally
    public void UpgradeEffect(StatusEffectType type, float durationMultiplier, float damageMultiplier)
    {
        if (statusEffectDict.TryGetValue(type, out StatusEffectData data))
        {
            data.durationMultiplier = durationMultiplier;
            data.damageMultiplier = damageMultiplier;
        }
    }
}
