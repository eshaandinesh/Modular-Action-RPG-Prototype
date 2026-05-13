using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject winText;
    public GameObject loseText;

    void Start()
    {
        // Double-check that screens are hidden when the level starts
        if (winText != null) winText.SetActive(false);
        if (loseText != null) loseText.SetActive(false);
        
        // Ensure the game is running at normal speed
        Time.timeScale = 1f; 
    }

    public void ShowWinScreen()
    {
        if (winText != null) winText.SetActive(true); // Show the text
        Time.timeScale = 0f; // This instantly freezes/pauses the game!
    }

    public void ShowLoseScreen()
    {
        if (loseText != null) loseText.SetActive(true); // Show the text
        Time.timeScale = 0f; // Freeze the game
    }
}