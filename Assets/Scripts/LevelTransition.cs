using UnityEngine;
using UnityEngine.SceneManagement; // This is required to load new levels!

public class LevelTransition : MonoBehaviour
{
    [Header("Next Level Setup")]
    public string nextSceneName; // Type the exact name of the next level here

    // This function runs automatically when something touches the Trigger box
    private void OnTriggerEnter(Collider other)
    {
        // Did the Player touch it?
        if (other.CompareTag("Player"))
        {
            Debug.Log("Transitioning to: " + nextSceneName);
            SceneManager.LoadScene(nextSceneName);
        }
    }
}