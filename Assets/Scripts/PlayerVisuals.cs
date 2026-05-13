using UnityEngine;
using Switch.Core;

public class PlayerVisuals : MonoBehaviour
{
    private PlayerStats stats;

    [Header("Assign Your KayKit Models Here")]
    public GameObject baseModel;
    public GameObject tankModel;
    public GameObject mageModel;
    public GameObject assassinModel;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
    }

    private void OnEnable()
    {
        // When the script turns on, tell it to listen for the stat swap event
        if (stats != null)
        {
            stats.OnStatsSwapped += UpdateVisualModel;
        }
    }

    private void OnDisable()
    {
        // Stop listening when destroyed to prevent memory leaks
        if (stats != null)
        {
            stats.OnStatsSwapped -= UpdateVisualModel;
        }
    }

    private void Start()
    {
        // run this once at the start so we don't look like an ugly amalgamation of 4 people
        UpdateVisualModel();
    }

    private void UpdateVisualModel()
    {
        // turn ALL models off first
        if (baseModel) baseModel.SetActive(false);
        if (tankModel) tankModel.SetActive(false);
        if (mageModel) mageModel.SetActive(false);
        if (assassinModel) assassinModel.SetActive(false);

        // turn on only the one that matches our current class
        switch (stats.currentClass)
        {
            case ClassType.Base:
                if (baseModel) baseModel.SetActive(true);
                break;
            case ClassType.Tank:
                if (tankModel) tankModel.SetActive(true);
                break;
            case ClassType.Mage:
                if (mageModel) mageModel.SetActive(true);
                break;
            case ClassType.Assassin:
                if (assassinModel) assassinModel.SetActive(true);
                break;
        }
    }
}