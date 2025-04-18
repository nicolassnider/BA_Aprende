using UnityEngine;

/// <summary>
/// Manages the character's actions, including movement, sprinting, energy management, and status effects.
/// </summary>
public class CharacterActions : MonoBehaviour
{
    /// <summary>
    /// The normal movement speed of the character.
    /// </summary>
    public float moveSpeed = 5f;

    /// <summary>
    /// The multiplier applied to movement speed when sprinting.
    /// </summary>
    public float sprintMultiplier = 2f;

    /// <summary>
    /// The base damage dealt by the character.
    /// </summary>
    public float damage = 10f;

    /// <summary>
    /// The multiplier applied to damage when Berserk mode is active.
    /// </summary>
    public float berserkMultiplier = 1.25f;

    /// <summary>
    /// The current energy level of the character.
    /// </summary>
    public float energy = 100f;

    /// <summary>
    /// The maximum energy level of the character.
    /// </summary>
    public float maxEnergy = 100f;

    /// <summary>
    /// The rate at which energy is depleted while sprinting (per second).
    /// </summary>
    public float energyDepletionRate = 20f;

    /// <summary>
    /// The rate at which energy regenerates when not sprinting (per second).
    /// </summary>
    public float energyRegenRate = 10f;

    /// <summary>
    /// The Berserk status effect applied to the character.
    /// </summary>
    private Status berserkStatus;

    /// <summary>
    /// The SpriteRenderer component used to visually represent the character.
    /// </summary>
    private SpriteRenderer spriteRenderer;

    /// <summary>
    /// The Animator component used to control character animations.
    /// </summary>
    private Animator animator;

    /// <summary>
    /// Initializes the character's components and sets up the Berserk status.
    /// </summary>
    private void Start()
    {
        // Initialize the Berserk status
        berserkStatus = new Status("Berserk", 10f, 30f);
        berserkStatus.OnActivate = ActivateBerserkEffects;
        berserkStatus.OnDeactivate = DeactivateBerserkEffects;

        // Ensure the SpriteRenderer is assigned
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer not found on the character. Please attach a SpriteRenderer component.");
        }

        // Ensure the Animator is assigned
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator not found on the character. Please attach an Animator component.");
        }

        // Start a timer to activate Berserk after 2 minutes
        Invoke(nameof(ActivateBerserk), 120f);
    }

    /// <summary>
    /// Updates the character's state, including movement, energy regeneration, and status effects.
    /// </summary>
    private void Update()
    {
        // Check if berserkStatus is initialized
        if (berserkStatus != null)
        {
            // Update the Berserk status
            berserkStatus.Update(Time.deltaTime);
        }
        else
        {
            Debug.LogError("BerserkStatus is null. Ensure it is initialized in the Start method.");
        }

        // Handle character movement
        MoveCharacter();

        // Regenerate energy when not sprinting
        RegenerateEnergy();

        // Trigger the Attack_1 animation when the 'J' key is pressed
        if (Input.GetKeyDown(KeyCode.J))
        {
            PlayAttackAnimation();
        }
    }

    /// <summary>
    /// Handles the character's movement based on player input.
    /// </summary>
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

        // Update the Animator's IsMoving parameter
        if (animator != null)
        {
            animator.SetBool("IsMoving", movement.magnitude > 0);
        }

        // Get the current speed (normal or sprinting)
        float currentSpeed = Sprint();

        // Move the character
        transform.Translate(movement * currentSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Determines the character's current speed, applying the sprint multiplier if sprinting.
    /// </summary>
    /// <returns>The current movement speed of the character.</returns>
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

    /// <summary>
    /// Regenerates the character's energy when not sprinting.
    /// </summary>
    void RegenerateEnergy()
    {
        // Regenerate energy when not sprinting
        if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
        {
            energy += energyRegenRate * Time.deltaTime;
            energy = Mathf.Clamp(energy, 0, maxEnergy); // Ensure energy doesn't exceed maxEnergy
        }
    }

    /// <summary>
    /// Activates the Berserk status effect.
    /// </summary>
    void ActivateBerserk()
    {
        berserkStatus.Activate();
    }

    /// <summary>
    /// Applies the effects of Berserk mode, such as increasing damage and changing the character's color.
    /// </summary>
    void ActivateBerserkEffects()
    {
        Debug.Log("Berserk mode activated! Damage is now multiplied.");

        // Set the character's color to red
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
        }
    }

    /// <summary>
    /// Removes the effects of Berserk mode, such as resetting damage and the character's color.
    /// </summary>
    void DeactivateBerserkEffects()
    {
        Debug.Log("Berserk mode deactivated! Damage is back to normal.");

        // Reset the character's color
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }
    }

    /// <summary>
    /// Gets the character's current damage value, applying the Berserk multiplier if active.
    /// </summary>
    /// <returns>The current damage value of the character.</returns>
    public float GetDamage()
    {
        return berserkStatus.IsActive ? damage * berserkMultiplier : damage;
    }

    /// <summary>
    /// Triggers the Attack_1 animation.
    /// </summary>
    private void PlayAttackAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack_1");
        }
        else
        {
            Debug.LogError("Animator component not found on the character.");
        }
    }
}