using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider healthSlider; //Reference to the UI component that represents the health bar
    [SerializeField] private TestController player; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        healthSlider.maxValue = player.getMaxHealth();
    }

    // Update is called once per frame
    void Update()
    {
        healthSlider.value = player.getHealth();
    }
}
