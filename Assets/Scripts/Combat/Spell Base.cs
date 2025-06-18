using UnityEngine;

public abstract class SpellBase : MonoBehaviour
{
    public float damage;
    public float duration;
    public GameObject rangeIndicatorPrefab;
    protected GameObject activeIndicator;

    protected virtual void Start()
    {
        if (rangeIndicatorPrefab != null)
        {
            activeIndicator = Instantiate(rangeIndicatorPrefab, transform.position, Quaternion.identity);
            activeIndicator.transform.localScale = Vector3.one * GetRange();
            Destroy(activeIndicator, duration);
        }
        Destroy(gameObject, duration);
    }

    public abstract float GetRange();
}
