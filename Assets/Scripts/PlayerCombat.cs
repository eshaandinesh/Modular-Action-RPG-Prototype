using UnityEngine;
using Switch.Core;
using System.Collections; // Required for Person A's Coroutines!

[RequireComponent(typeof(PlayerStats))]
public class PlayerCombat : MonoBehaviour
{
    private PlayerStats myStats;

    [Header("Combat Settings")]
    public Transform attackPoint;
    private float nextAttackTime = 0f; 

    [Header("Melee Settings")]
    public float meleeCooldown = 1f; 
    public LayerMask enemyLayer;

    [Header("Ranged Settings (Mage)")]
    public GameObject fireballPrefab;
    public float rangedCooldown = 2.0f; 

    private void Start()
    {
        myStats = GetComponent<PlayerStats>();
    }

    private void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Attack();
            }
        }
    }

    private void Attack()
    {
        if (attackPoint == null) return;

        if (myStats.currentClass == ClassType.Mage)
        {
            PerformRangedAttack();
            nextAttackTime = Time.time + rangedCooldown; 
        }
        else
        {
            PerformMeleeAttack();
            nextAttackTime = Time.time + meleeCooldown; 
        }
    }

    // --- OUR TRUE CROSSHAIR FINDER ---
    private Vector3 GetCrosshairTargetPoint()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))
            return hit.point;
        
        return ray.GetPoint(100f);
    }

    // --- PERSON A'S ANIMATIONS + OUR RANGED AIMING ---
    private void PerformRangedAttack()
    {
        Animator currentAnimator = GetComponentInChildren<Animator>();
        if (currentAnimator != null) currentAnimator.SetTrigger("Attack");

        StartCoroutine(SpawnFireballWithDelay(0.55f));
    }

    private IEnumerator SpawnFireballWithDelay(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        if (fireballPrefab == null) yield break;

        // Use our crosshair math instead of A's old math!
        Vector3 targetPoint = GetCrosshairTargetPoint();
        Vector3 aimDirection = (targetPoint - attackPoint.position).normalized;

        GameObject projectile = Instantiate(fireballPrefab, attackPoint.position, Quaternion.identity);
        projectile.transform.forward = aimDirection;

        Projectile magicScript = projectile.GetComponent<Projectile>();
        if (magicScript != null)
        {
            magicScript.Initialize(myStats.currentDamage, myStats.currentClass);
        }

        Debug.Log("Mage hurled a True-Aim fireball!");
    }

    // --- PERSON A'S ANIMATIONS + OUR MELEE AIMING/RANGE ---
    private void PerformMeleeAttack()
    {
        Animator currentAnimator = GetComponentInChildren<Animator>();
        if (currentAnimator != null) currentAnimator.SetTrigger("Attack");

        StartCoroutine(MeleeHitWithDelay(0.55f));
    }

    private IEnumerator MeleeHitWithDelay(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        // Find exactly where we are looking
        Vector3 targetPoint = GetCrosshairTargetPoint();
        Vector3 aimDirection = (targetPoint - attackPoint.position).normalized;

        // Use our dynamic range from PlayerStats, NOT a hardcoded number!
        float range = myStats.currentAttackRange;

        // Push the damage sphere out towards the crosshair
        Vector3 hitCenter = attackPoint.position + (aimDirection * (range * 0.5f));

        Collider[] hitEnemies = Physics.OverlapSphere(hitCenter, range, enemyLayer);

        foreach (Collider enemyCollider in hitEnemies)
        {
            HealthManager enemyHealth = enemyCollider.GetComponent<HealthManager>();
            if (enemyHealth != null)
            {
                CharClassData enemyClass = enemyHealth.myClassData;
                enemyHealth.TakeDamage(myStats.currentDamage, myStats.currentClass);

                if (enemyHealth.currentHealth <= 0)
                {
                    Debug.Log($"<color=cyan>FATAL BLOW! Stealing {enemyClass.classType} stats!</color>");
                    myStats.SwapStats(enemyClass);
                }
            }
        }
    }

    // --- OUR DYNAMIC GIZMOS ---
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        
        float rangeToDraw = 1.5f;
        
        if (Application.isPlaying && myStats != null)
        {
            rangeToDraw = myStats.currentAttackRange;
        }

        if (Application.isPlaying && Camera.main != null)
        {
            Vector3 targetPoint = GetCrosshairTargetPoint();
            Vector3 aimDirection = (targetPoint - attackPoint.position).normalized;
            Vector3 hitCenter = attackPoint.position + (aimDirection * (rangeToDraw * 0.5f));
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(hitCenter, rangeToDraw);
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(attackPoint.position, targetPoint);
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position + (attackPoint.forward * (rangeToDraw * 0.5f)), rangeToDraw);
        }
    }
}