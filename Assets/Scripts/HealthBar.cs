using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class HealthBar : MonoBehaviour
{
    public HealthManager targetHealth; 
    public Slider healthSlider;
    
    [Header("Optional")]
    public TMP_Text healthText; 

    void Start()
    {
        if (targetHealth != null && targetHealth.myClassData != null && healthSlider != null)
        {
            // Pull max health from Person A's class data
            healthSlider.maxValue = targetHealth.myClassData.maxHealth; 
            healthSlider.value = targetHealth.currentHealth;
        }
    }

    void Update()
    {
        if (targetHealth != null && healthSlider != null)
        {
            healthSlider.value = targetHealth.currentHealth;

            if (healthText != null && targetHealth.myClassData != null)
            {
                healthText.text = Mathf.RoundToInt(targetHealth.currentHealth) + "";
                // healthText.text = Mathf.RoundToInt(targetHealth.currentHealth) + " / " + Mathf.RoundToInt(targetHealth.myClassData.maxHealth);
            }
        }
    }
}