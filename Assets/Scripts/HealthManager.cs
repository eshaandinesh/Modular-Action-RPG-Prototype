using System;
using UnityEngine;
using Switch.Core;

public class HealthManager : MonoBehaviour
{
    [Header("Character Identity")]
    [Tooltip("Drag the CharClassData (e.g., TankData or BaseData) here.")]
    public CharClassData myClassData;

    [Header("Live Stats")]
    public float currentHealth;

    public event Action<CharClassData> OnDeath;

    private void Start()
    {
        if (myClassData != null)
            currentHealth = myClassData.maxHealth;
        else
            Debug.LogError($"{gameObject.name} is missing its CharClassData!");
    }

    public void TakeDamage(float rawDamage, ClassType attackerClass)
    {
        if (myClassData == null) return;

        // send the attack to calculator
        float finalDamage = DmgCalc.GetCalculatedDamage(rawDamage, attackerClass, myClassData.classType);

        // apply dmg
        currentHealth -= finalDamage;
        Debug.Log($"<color=orange>{gameObject.name} took {finalDamage} damage!</color> HP remaining: {currentHealth}");

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        Debug.Log($"<color=red>{gameObject.name} has been defeated!</color>");

        // broadcast that we died and pass our stats to whoever is listening
        OnDeath?.Invoke(myClassData);

        // if this is the player, we don't destroy the object, we just handle Game Over later.
        if (gameObject.CompareTag("Player"))
            Debug.Log("PLAYER DIED! Trigger Game Over screen.");
        else
            Destroy(gameObject);
    }
}