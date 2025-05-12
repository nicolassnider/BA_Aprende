using UnityEngine;

/// <summary>
/// Manages the character's actions, including movement, sprinting, swimming, energy management, and status effects.
/// </summary>
public class CharacterActions : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float sprintMultiplier = 2f;
    public float swimmingSpeedMultiplier = 0.5f;
    public float damage = 10f;
    public float berserkMultiplier = 1.25f;
    public float energy = 100f;
    public float maxEnergy = 100f;
    public float energyDepletionRate = 20f;
    public float energyRegenRate = 10f;
    public float waterMassEnergyDrain = 10f;
    public int health = 100;

    private bool isBurning = false;
    private float burnDamage = 5f; // Damage per second while burning
    private float burnDuration = 3f; // Duration of burning effect
    private float burnTimer = 0f;
    private bool isInWater = false;
    private Status berserkStatus;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private void Start()
    {
        // Initialize Berserk status
        berserkStatus = new Status("Berserk", 10f, 30f)
        {
            OnActivate = ActivateBerserkEffects,
            OnDeactivate = DeactivateBerserkEffects
        };

        // Assign components
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        if (spriteRenderer == null)
            Debug.LogError("SpriteRenderer not found. Please attach a SpriteRenderer component.");

        if (animator == null)
            Debug.LogError("Animator not found. Please attach an Animator component.");

        // Activate Berserk after 2 minutes
        Invoke(nameof(ActivateBerserk), 60f);
    }

    private void Update()
    {
        berserkStatus?.Update(Time.deltaTime);

        MoveCharacter();
        RegenerateEnergy();

        if (Input.GetKeyDown(KeyCode.J))
            PlayAttackAnimation();

        if (isBurning)
        {
            ApplyBurnDamage();
        }
    }

    private void MoveCharacter()
    {
        // Get input from "Horizontal" (A/D or Left/Right) and "Vertical" (W/S or Up/Down) axes
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Create a movement vector based on input
        Vector2 movement = new Vector2(horizontalInput, verticalInput).normalized;

        // Update the Animator's IsMoving parameter
        if (animator != null)
            animator.SetBool("IsMoving", movement.magnitude > 0);

        // Trigger animations based on vertical input
        if (verticalInput > 0)
        {
            ResetAnimationTriggers();
            PlayFrontAnimation(); // Moving up
        }
        else if (verticalInput < 0)
        {
            ResetAnimationTriggers();
            PlayDownAnimation(); // Moving down
        }
        else if (movement.magnitude == 0)
        {
            ResetAnimationTriggers(); // Reset animations when the character stops moving
        }

        // Determine the current speed (swimming or sprinting)
        float currentSpeed = isInWater ? Swim() : Sprint();

        // Apply movement
        transform.Translate(movement * currentSpeed * Time.deltaTime);
    }

    private float Sprint()
    {
        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && energy > 0)
        {
            energy -= energyDepletionRate * Time.deltaTime;
            energy = Mathf.Clamp(energy, 0, maxEnergy);

            ResetAnimationTriggers();
            PlayRunAnimation();

            return moveSpeed * sprintMultiplier;
        }

        return moveSpeed;
    }

    private void RegenerateEnergy()
    {
        if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
        {
            energy += energyRegenRate * Time.deltaTime;
            energy = Mathf.Clamp(energy, 0, maxEnergy);
        }
    }

    private float Swim()
    {
        if (energy > 0)
        {
            energy -= energyDepletionRate * Time.deltaTime;
            energy = Mathf.Clamp(energy, 0, maxEnergy);
            return moveSpeed * swimmingSpeedMultiplier; // Reduced speed while swimming
        }

        Debug.Log("Out of energy! Cannot swim.");
        return 0f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Character collided with: " + other.name);
        if (other.CompareTag("WaterMass"))
        {
            isInWater = true;
            DrainEnergy(waterMassEnergyDrain);
            Debug.Log("Character entered water.");
        }
        else if (other.name is "Fire")
        {
            StartBurning();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("WaterMass"))
        {
            isInWater = false;
            Debug.Log("Character exited water.");
        }
        else if (other.CompareTag("Fire"))
        {
            StopBurning();
        }
    }

    private void StartBurning()
    {
        isBurning = true;
        burnTimer = burnDuration;
        Debug.Log("Character started burning!");
    }

    private void StopBurning()
    {
        isBurning = false;
        Debug.Log("Character stopped burning.");
    }

    private void ApplyBurnDamage()
    {
        if (burnTimer > 0)
        {
            burnTimer -= Time.deltaTime;

            // Reduce health based on burn damage
            health -= Mathf.CeilToInt(burnDamage * Time.deltaTime);
            health = Mathf.Max(health, 0); // Ensure health doesn't go below 0

            // Log the current health while burning
            Debug.Log("Current Health: " + health);

            // Optionally, stop burning if health reaches 0
            if (health <= 0)
            {
                Debug.Log("Character has died from burning.");
                StopBurning();
            }
        }
        else
        {
            StopBurning();
        }
    }

    private void DrainEnergy(float amount)
    {
        energy -= amount;
        energy = Mathf.Clamp(energy, 0, maxEnergy);
    }

    private void ActivateBerserk()
    {
        berserkStatus.Activate();
    }

    private void ActivateBerserkEffects()
    {
        if (spriteRenderer != null)
            spriteRenderer.color = Color.red;
    }

    private void DeactivateBerserkEffects()
    {
        if (spriteRenderer != null)
            spriteRenderer.color = Color.white;
    }

    public float GetDamage()
    {
        return berserkStatus.IsActive ? damage * berserkMultiplier : damage;
    }

    private void PlayAttackAnimation()
    {
        if (animator != null)
            animator.SetTrigger("Attack_1");
    }

    private void PlayFrontAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Front");
        }
        else
        {
            Debug.LogWarning("Animator not found. Cannot play front animation.");
        }
    }

    private void PlayDownAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("IsMoving");
        }
        else
        {
            Debug.LogWarning("Animator not found. Cannot play down animation.");
        }
    }

    private void PlayRunAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("IsMoving");
        }
        else
        {
            Debug.LogWarning("Animator not found. Cannot play run animation.");
        }
    }

    private void ResetAnimationTriggers()
    {
        if (animator != null)
        {
            animator.ResetTrigger("Front");
            animator.ResetTrigger("Down");
            animator.ResetTrigger("Run");
            animator.ResetTrigger("Attack_1");
        }
        else
        {
            Debug.LogWarning("Animator not found. Cannot reset animation triggers.");
        }
    }
}
