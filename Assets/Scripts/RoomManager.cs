using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [Header("Room Setup")]
    public GameObject exitDoor; // We will assign the door cube here

    [Header("Enemy Tracking")]
    // This list will automatically keep track of who is alive
    public List<GameObject> activeEnemies = new List<GameObject>();

    void Start()
    {
        // Make sure the door is solid and blocking the exit when the level starts
        if (exitDoor != null) 
        {
            exitDoor.SetActive(true);
        }
    }

    // Enemies will call this exact function right before they die
    public void EnemyDefeated(GameObject deadEnemy)
    {
        if (activeEnemies.Contains(deadEnemy))
        {
            activeEnemies.Remove(deadEnemy);
            Debug.Log("An enemy died! Remaining: " + activeEnemies.Count);
        }

        // Check if that was the last enemy
        if (activeEnemies.Count <= 0)
        {
            UnlockRoom();
        }
    }

    private void UnlockRoom()
    {
        Debug.Log("Sector Cleared! Opening the door.");
        if (exitDoor != null)
        {
            exitDoor.SetActive(false); // This makes the door vanish!
        }
    }
}