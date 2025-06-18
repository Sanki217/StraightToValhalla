using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    public float moveSpeed = 2f;
    public float damage = 10f;
    public float attackSpeed = 1f;
    public float attackRange = 1.5f;

    [Header("Health UI")]
    public GameObject healthBarCanvas;
    public Image healthBarFill;

    private float attackCooldown = 0f;
    private Transform wallTransform;
    private Wall wallScript;

    private void Start()
    {
        currentHealth = maxHealth;
        healthBarCanvas.SetActive(false);

        GameObject wallObj = GameObject.FindWithTag("Wall");
        if (wallObj != null)
        {
            wallTransform = wallObj.transform;
            wallScript = wallObj.GetComponent<Wall>();
        }
    }

    private void Update()
    {
        if (wallTransform == null) return;

        float distanceToWall = Vector3.Distance(transform.position, wallTransform.position);

        if (distanceToWall > attackRange)
        {
            transform.position += Vector3.left * moveSpeed * Time.deltaTime;
        }
        else
        {
            attackCooldown -= Time.deltaTime;
            if (attackCooldown <= 0f)
            {
                wallScript.TakeDamage(damage);
                attackCooldown = 1f / attackSpeed;
            }
        }
    }

    public void TakeDamage(float dmg)
    {
        currentHealth -= dmg;
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            healthBarCanvas.SetActive(true);
            healthBarFill.fillAmount = currentHealth / maxHealth;
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
