using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Switch.Core;

[RequireComponent(typeof(NavMeshAgent))]
public class FinalBossAI : MonoBehaviour
{
    [Header("Setup")]
    public Transform player;
    public GameObject aoeMarkerPrefab;
    private NavMeshAgent agent;
    private Animator anim;

    [Header("Boss Stats")]
    public float attackRange = 6f;
    public float attackCooldown = 3f;
    public float dashDamage = 25f;
    public float aoeDamage = 50f;
    public ClassType bossClass = ClassType.Tank; 
    private bool isAttacking = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        StartCoroutine(BossThinkRoutine());
    }

    IEnumerator BossThinkRoutine()
    {
        while (true)
        {
            if (player == null || isAttacking)
            {
                yield return null;
                continue;
            }

            float dist = Vector3.Distance(transform.position, player.position);

            if (dist > attackRange)
            {
                agent.isStopped = false;
                agent.SetDestination(player.position);
            }
            else
            {
                agent.isStopped = true;
                int attackChoice = Random.Range(0, 2);

                if (attackChoice == 0)
                    yield return StartCoroutine(DashAttack());
                else
                    yield return StartCoroutine(AoeSlam());
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator DashAttack()
    {
        isAttacking = true;

        if (anim) anim.SetTrigger("Attack");
        Debug.Log("<color=orange>BOSS: WINDING UP DASH!</color>");
        yield return new WaitForSeconds(0.5f);

        float originalSpeed = agent.speed;
        float originalAccel = agent.acceleration;

        agent.speed = 25f;
        agent.acceleration = 60f;
        agent.isStopped = false;

        Vector3 dashTarget = player.position + (player.position - transform.position).normalized * 2f;
        agent.SetDestination(dashTarget);

        yield return new WaitForSeconds(0.6f);

        if (Vector3.Distance(transform.position, player.position) < 3f)
        {
            Debug.Log("<color=red>BOSS DASH HIT THE PLAYER!</color>");
            HealthManager pHealth = player.GetComponent<HealthManager>();
            if (pHealth != null) 
                pHealth.TakeDamage(dashDamage, bossClass);
        }

        agent.speed = originalSpeed;
        agent.acceleration = originalAccel;
        agent.isStopped = true;
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    IEnumerator AoeSlam()
    {
        isAttacking = true;

        if (anim) anim.SetTrigger("Attack");
        Debug.Log("<color=magenta>BOSS: AOE SLAM INCOMING!</color>");

        Vector3 groundPos = new Vector3(transform.position.x, 0.1f, transform.position.z);
        GameObject marker = Instantiate(aoeMarkerPrefab, groundPos, Quaternion.identity);

        yield return new WaitForSeconds(1.5f);

        Destroy(marker);
        Debug.Log("BOOM!");

        // dmg 
        if (Vector3.Distance(transform.position, player.position) < 5f)
        {
            Debug.Log("<color=red>PLAYER CAUGHT IN AOE!</color>");
            HealthManager pHealth = player.GetComponent<HealthManager>();
            if (pHealth != null) pHealth.TakeDamage(aoeDamage, bossClass);
        }

        // cooldown
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    void Update()
    {
        if (anim != null) anim.SetFloat("Speed", agent.velocity.magnitude);
    }
}