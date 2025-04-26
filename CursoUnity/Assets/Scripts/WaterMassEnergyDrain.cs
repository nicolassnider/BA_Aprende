using UnityEngine;

public class WaterMassEnergyDrain : MonoBehaviour
{
    public GameObject mainChar; // Reference to the MainChar object
    public float energyDrainAmount = 10f; // Amount of energy to drain per WaterMass
    private MainCharEnergy mainCharEnergy; // Reference to the MainChar's energy script

    void Start()
    {
        // Get the MainCharEnergy component from the MainChar
        mainCharEnergy = mainChar.GetComponent<MainCharEnergy>();

        if (mainCharEnergy == null)
        {
            Debug.LogError("MainCharEnergy script is missing on MainChar!");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if MainChar enters the WaterMass
        if (other.gameObject == mainChar)
        {
            // Drain energy from MainChar
            mainCharEnergy.DrainEnergy(energyDrainAmount);
        }
    }
}