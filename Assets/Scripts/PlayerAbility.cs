using UnityEngine;
using Switch.Core;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerStats))]
public class PlayerAbility : MonoBehaviour
{
    // abilities you can unlock from bosses
    public enum AbilityType { None, Dash, AOEBlast }

    [Header("Active Ability")]
    public AbilityType currentAbility = AbilityType.None;

    [Header("Cooldown Settings")]
    public float abilityCooldown = 3.0f; // 3 seconds before you can use it again
    private float cooldownTimer = 0f;

    [Header("AOE Settings")]
    public float blastRadius = 5f;
    public float blastDamage = 30f;
    public LayerMask enemyLayer;

    private CharacterController controller;
    private PlayerStats stats;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        stats = GetComponent<PlayerStats>();
    }

    private void Update()
    {
        // cooldown countdown
        if (cooldownTimer > 0)
            cooldownTimer -= Time.deltaTime;

        // listen for the ability button
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (currentAbility == AbilityType.None)
            {
                Debug.Log("No ability unlocked yet! Go kill a boss.");
                return;
            }

            if (cooldownTimer <= 0)
                ExecuteAbility();
            else
                Debug.Log($"Ability on cooldown! {cooldownTimer:F1} seconds left.");
        }
    }

    private void ExecuteAbility()
    {
        cooldownTimer = abilityCooldown; // reset timer

        switch (currentAbility)
        {
            case AbilityType.Dash:
                PerformDash();
                break;
            case AbilityType.AOEBlast:
                PerformAOEBlast();
                break;
        }
    }

    private void PerformDash()
    {
        Debug.Log("<color=magenta>ABILITY USED: DASH!</color>");

        // quick game-jam dash: We instantly move the player forward by 5 units.
        // as we use controller.Move(), it will still respect walls and won't clip out of bounds!
        Vector3 dashDirection = transform.forward * 5f;
        controller.Move(dashDirection);
    }

    private void PerformAOEBlast()
    {
        Debug.Log("<color=magenta>ABILITY USED: AOE BLAST!</color>");

        // find all enemies in a radius around the player
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, blastRadius, enemyLayer);

        foreach (Collider enemyCollider in hitEnemies)
        {
            HealthManager enemyHealth = enemyCollider.GetComponent<HealthManager>();
            if (enemyHealth != null)
            {
                // hit them with the aoe blast! we use stats.currentClass so the rps dmg is considered
                enemyHealth.TakeDamage(blastDamage, stats.currentClass);
            }
        }
    }

    // draw the AOE blast radius in the editor for easy tweaking
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, blastRadius);
    }
}