using UnityEngine;

public class CharacterActions : MonoBehaviour
{
    // Variables
    public float moveSpeed = 5f; // Normal movement speed
    public float sprintMultiplier = 2f; // Multiplier for sprinting speed
    public float damage = 10f; // Base damage value
    public float berserkMultiplier = 1.25f; // Damage multiplier when Berserk is active

    public float energy = 100f; // Current energy level
    public float maxEnergy = 100f; // Maximum energy level
    public float energyDepletionRate = 20f; // Energy depletion rate while sprinting (per second)
    public float energyRegenRate = 10f; // Energy regeneration rate when not sprinting (per second)

    private bool berserkActive = false; // Tracks if Berserk mode is active
    private float berserkTimer = 0f; // Timer to track time since the game started
    private float berserkActivationTime = 120f; // Time (in seconds) to activate Berserk mode

    void Start()
    {
        // Example of type casting
        float f1 = 9.9f;
        int ni = (int)f1;
    }

    // Update is called once per frame
    void Update()
    {
        // Update the Berserk timer
        UpdateBerserkTimer();

        // Handle character movement
        MoveCharacter();

        // Regenerate energy when not sprinting
        RegenerateEnergy();
    }

    void MoveCharacter()
    {
        // Get input from "Horizontal" (A/D or Left/Right) and "Vertical" (W/S or Up/Down) axes
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Create a movement vector based on input
        Vector2 movement = new Vector2(horizontalInput, verticalInput);

        // Normalize the movement vector to ensure consistent speed in all directions
        if (movement.magnitude > 1)
        {
            movement.Normalize();
        }

        // Get the current speed (normal or sprinting)
        float currentSpeed = Sprint();

        // Move the character
        transform.Translate(movement * currentSpeed * Time.deltaTime);
    }

    float Sprint()
    {
        // Check if the "Shift" key is pressed and there is enough energy
        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && energy > 0)
        {
            // Deplete energy while sprinting
            energy -= energyDepletionRate * Time.deltaTime;
            energy = Mathf.Clamp(energy, 0, maxEnergy); // Ensure energy doesn't go below 0
            Debug.Log($"Sprinting... Energy: {energy}");
            return moveSpeed * sprintMultiplier;
        }

        // Return normal speed
        return moveSpeed;
    }

    void RegenerateEnergy()
    {
        // Regenerate energy when not sprinting
        if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
        {
            energy += energyRegenRate * Time.deltaTime;
            energy = Mathf.Clamp(energy, 0, maxEnergy); // Ensure energy doesn't exceed maxEnergy
        }
    }

    void UpdateBerserkTimer()
    {
        // Increment the timer
        berserkTimer += Time.deltaTime;

        // Check if the timer has reached the activation time
        if (!berserkActive && berserkTimer >= berserkActivationTime)
        {
            ActivateBerserk();
        }
    }

    void ActivateBerserk()
    {
        // Activate Berserk mode
        berserkActive = true;
        Debug.Log("Berserk mode activated! Damage is now multiplied.");
    }

    public float GetDamage()
    {
        // Return the damage value, applying the Berserk multiplier if active
        return berserkActive ? damage * berserkMultiplier : damage;
    }
}