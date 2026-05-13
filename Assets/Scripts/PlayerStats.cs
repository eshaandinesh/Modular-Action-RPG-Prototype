using System;
using UnityEngine;
using Switch.Core;

public class PlayerStats : MonoBehaviour
{
    [Header("Starting Data")]
    public CharClassData startingClassData;

    [Header("Active Stats")]
    public ClassType currentClass;
    public float currentHealth;
    public float maxHealth;
    public float currentDamage;
    public float currentSpeed;
    public float currentAttackRange;

    // Loudspeaker that shouts when stats change
    public event Action OnStatsSwapped;

    // NEW: We need to talk to the HealthManager!
    private HealthManager myHealthManager;

    private void Awake()
    {
        myHealthManager = GetComponent<HealthManager>();
    }

    private void Start()
    {
        if (startingClassData != null)
            SwapStats(startingClassData);
    }

    public void SwapStats(CharClassData newClass)
    {
        currentClass = newClass.classType;
        maxHealth = newClass.maxHealth;
        currentDamage = newClass.damage;
        currentSpeed = newClass.speed;

        currentAttackRange = newClass.attackRange;
        
        currentHealth = maxHealth; 

        if (myHealthManager != null)
        {
            myHealthManager.myClassData = newClass;
            myHealthManager.currentHealth = maxHealth;
        }

        Debug.Log($"Swapped to {currentClass}! Speed is now {currentSpeed}");

        OnStatsSwapped?.Invoke();
    }
}