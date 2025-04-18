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

    private Status berserkStatus; // Berserk status
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer

    void Start()
    {
        // Initialize the Berserk status with a duration of 10 seconds and a cooldown of 30 seconds
        berserkStatus = new Status("Berserk", 10f, 30f);
        berserkStatus.OnActivate = ActivateBerserkEffects;
        berserkStatus.OnDeactivate = DeactivateBerserkEffects;

        // Get the SpriteRenderer component
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer not found on the character. Please attach a SpriteRenderer component.");
        }

        // Start a timer to activate Berserk after 2 minutes
        Invoke(nameof(ActivateBerserk), 120f);
    }

    // Update is called once per frame
    void Update()
    {
        // Update the Berserk status
        berserkStatus.Update(Time.deltaTime);

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

    void ActivateBerserk()
    {
        // Activate the Berserk status
        berserkStatus.Activate();
    }

    void ActivateBerserkEffects()
    {
        Debug.Log("Berserk mode activated! Damage is now multiplied.");

        // Set the character's color to red
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
        }
    }

    void DeactivateBerserkEffects()
    {
        Debug.Log("Berserk mode deactivated! Damage is back to normal.");

        // Reset the character's color
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }
    }

    public float GetDamage()
    {
        // Return the damage value, applying the Berserk multiplier if active
        return berserkStatus.IsActive ? damage * berserkMultiplier : damage;
    }
}