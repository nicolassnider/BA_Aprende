using UnityEngine;

public class MainCharEnergy : MonoBehaviour
{
    public float energy = 100f; // Initial energy

    public void DrainEnergy(float amount)
    {
        energy -= amount;
        energy = Mathf.Max(energy, 0); // Ensure energy doesn't go below 0
        Debug.Log("Energy: " + energy);
    }
}