using UnityEngine;
using Switch.Core; 

public class PlayerDeathHandler : MonoBehaviour
{
    private HealthManager myHealth;
    private GameUIManager uiManager;

    void Start()
    {
        myHealth = GetComponent<HealthManager>();
        uiManager = FindObjectOfType<GameUIManager>();

        if (myHealth != null)
        {
            myHealth.OnDeath += TriggerGameOver;
        }
    }

    void OnDestroy()
    {
        if (myHealth != null)
        {
            myHealth.OnDeath -= TriggerGameOver;
        }
    }

    private void TriggerGameOver(CharClassData myData)
    {
        Debug.Log("PLAYER HAS DIED! Triggering UI...");
        
        // Show your Game Over screen and freeze time!
        if (uiManager != null)
        {
            uiManager.ShowLoseScreen();
        }
        
        // WE DELETED THE SETACTIVE(FALSE) LINE HERE!
    }
}