using UnityEngine;
using UnityEngine.AI;
using Switch.Core; 
using System.Collections; // NEW: Required for the Coroutine attack delays!

[RequireComponent(typeof(HealthManager))] 
public class EnemyController : MonoBehaviour
{
    [Header("Enemy Identity")]
    public bool isBoss = false;
    private HealthManager myHealth; 
    private Animator myAnimator; // NEW: To handle animations!

    [Header("Sight & Detection")]
    public float detectionRange = 10f;
    public float viewAngle = 90f;
    private bool isChasing = false;

    [Header("Combat Stats")]
    public float attackRange = 2f;
    public float attackDamage = 10f;
    public float attackCooldown = 1.5f;
    private float nextAttackTime = 0f;

    [Header("Ranged Stats (For Mages)")]
    public GameObject fireballPrefab; // NEW: Projectile
    public Transform attackPoint; // NEW: Where the fireball spawns

    private NavMeshAgent agent;
    private Transform playerTarget;
    private RoomManager roomManager;
    private GameUIManager uiManager;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        myHealth = GetComponent<HealthManager>();
        
        // Find the Animator on the 3D model!
        myAnimator = GetComponentInChildren<Animator>();

        // Make sure the enemy actually stops moving when they reach attack range!
        agent.stoppingDistance = attackRange;

        if (myHealth != null)
        {
            myHealth.OnDeath += HandleDeath;
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTarget = playerObj.transform;
        }

        roomManager = FindObjectOfType<RoomManager>();
        if (roomManager != null)
        {
            roomManager.activeEnemies.Add(this.gameObject);
        }

        uiManager = FindObjectOfType<GameUIManager>();
    }

    void OnDestroy()
    {
        if (myHealth != null)
        {
            myHealth.OnDeath -= HandleDeath;
        }
    }

    void Update()
    {
        if (playerTarget == null) return;

        if (!isChasing)
        {
            CheckForPlayer();
        }
        else
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);

            if (distanceToPlayer <= attackRange)
            {
                // Stop walking and attack
                agent.SetDestination(transform.position); 
                FacePlayer();
                
                if (Time.time >= nextAttackTime)
                {
                    AttackPlayer();
                    nextAttackTime = Time.time + attackCooldown;
                }
            }
            else
            {
                // Keep chasing
                agent.SetDestination(playerTarget.position);
            }
            
            if (distanceToPlayer > detectionRange * 1.5f)
            {
                isChasing = false;
                agent.ResetPath(); 
            }
        }

        // NEW: Update the Run Animation based on how fast the NavMesh agent is moving!
        if (myAnimator != null)
        {
            myAnimator.SetFloat("Speed", agent.velocity.magnitude);
        }
    }

    private void CheckForPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);
        if (distanceToPlayer <= detectionRange)
        {
            Vector3 directionToFeet = (playerTarget.position - transform.position).normalized;
            float angleToPlayer = Vector3.Angle(transform.forward, directionToFeet);

            if (angleToPlayer < viewAngle / 2f)
            {
                RaycastHit hit;
                Vector3 rayStartPoint = transform.position + Vector3.up * 1f; 
                Vector3 playerCenter = playerTarget.position + Vector3.up * 1f; 
                Vector3 accurateAim = (playerCenter - rayStartPoint).normalized;

                Debug.DrawRay(rayStartPoint, accurateAim * detectionRange, Color.red);

                if (Physics.Raycast(rayStartPoint, accurateAim, out hit, detectionRange))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        isChasing = true;
                    }
                }
            }
        }
    }

    private void FacePlayer()
    {
        Vector3 direction = (playerTarget.position - transform.position).normalized;
        direction.y = 0; 
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    private void AttackPlayer()
    {
        // 1. Play the animation instantly!
        if (myAnimator != null) myAnimator.SetTrigger("Attack");

        // 2. Check if this enemy is a Mage. If yes, shoot fire! If no, do melee!
        if (myHealth != null && myHealth.myClassData != null && myHealth.myClassData.classType == ClassType.Mage)
        {
            StartCoroutine(SpawnFireballWithDelay(0.55f));
        }
        else
        {
            StartCoroutine(MeleeHitWithDelay(0.55f));
        }
    }

    // --- NEW: DELAYED ATTACK COROUTINES ---

    private IEnumerator SpawnFireballWithDelay(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        if (fireballPrefab == null || attackPoint == null || playerTarget == null) yield break;

        // Aim at the player's chest
        Vector3 targetPos = playerTarget.position + Vector3.up * 1f;
        Vector3 aimDirection = (targetPos - attackPoint.position).normalized;

        GameObject projectile = Instantiate(fireballPrefab, attackPoint.position, Quaternion.identity);
        projectile.transform.forward = aimDirection;

        Projectile magicScript = projectile.GetComponent<Projectile>();
        if (magicScript != null && myHealth != null && myHealth.myClassData != null)
        {
            magicScript.Initialize(attackDamage, myHealth.myClassData.classType);
        }
    }

    private IEnumerator MeleeHitWithDelay(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        if (playerTarget == null) yield break;

        // Check if the player is still in range when the animation swing finishes
        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);
        if (distanceToPlayer <= attackRange + 0.5f) 
        {
            HealthManager playerHealth = playerTarget.GetComponent<HealthManager>();
            if (playerHealth != null && myHealth != null && myHealth.myClassData != null)
            {
                Debug.Log($"{gameObject.name} swings and hits the player!");
                playerHealth.TakeDamage(attackDamage, myHealth.myClassData.classType);
            }
        }
        else
        {
            Debug.Log("Player dodged the enemy attack!");
        }
    }

    // --------------------------------------

    private void HandleDeath(CharClassData myData)
    {
        if (roomManager != null) roomManager.EnemyDefeated(this.gameObject);

        if (isBoss)
        {
            Debug.Log("BOSS DEFEATED! Player unlocks: DASH ABILITY.");
            if (uiManager != null) uiManager.ShowWinScreen();
            
            PlayerAbility playerAbility = playerTarget.GetComponent<PlayerAbility>();
            if (playerAbility != null)
            {
                playerAbility.currentAbility = PlayerAbility.AbilityType.Dash;
            }
        }
    }
}