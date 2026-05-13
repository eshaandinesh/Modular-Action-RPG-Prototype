using UnityEngine;
using Switch.Core;

public class Projectile : MonoBehaviour
{
    public float speed = 15f;
    private float damage;
    private ClassType attackerClass;

    // destroy the fireball after 3 seconds if it misses everything
    public float lifetime = 3f;

    public void Initialize(float dmg, ClassType type)
    {
        damage = dmg;
        attackerClass = type;
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // move the fireball forward every frame
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        // if the fireball touches the player, ignore it and keep flying
        // if (other.CompareTag("Player")) 
        //     return;

        HealthManager enemyHealth = other.GetComponent<HealthManager>();

        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage, attackerClass);

            if (enemyHealth.currentHealth <= 0)
            {
                PlayerStats playerStats = FindObjectOfType<PlayerStats>();
                if (playerStats != null)
                    playerStats.SwapStats(enemyHealth.myClassData);
            }
        }
        Destroy(gameObject);
    }
}