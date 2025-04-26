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
        Invoke(nameof(ActivateBerserk), 120f);
    }

    private void Update()
    {
        berserkStatus?.Update(Time.deltaTime);

        MoveCharacter();
        RegenerateEnergy();

        if (Input.GetKeyDown(KeyCode.J))
            PlayAttackAnimation();
    }

    private void MoveCharacter()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(horizontalInput, verticalInput).normalized;

        if (animator != null)
            animator.SetBool("IsMoving", movement.magnitude > 0);

        float currentSpeed = isInWater ? Swim() : Sprint();
        transform.Translate(movement * currentSpeed * Time.deltaTime);
    }

    private float Sprint()
    {
        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && energy > 0)
        {
            energy -= energyDepletionRate * Time.deltaTime;
            energy = Mathf.Clamp(energy, 0, maxEnergy);
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
            // Use the same energy depletion rate as sprinting
            energy -= energyDepletionRate * Time.deltaTime;
            energy = Mathf.Clamp(energy, 0, maxEnergy);
            return moveSpeed * swimmingSpeedMultiplier; // Reduced speed while swimming
        }

        Debug.Log("Out of energy! Cannot swim.");
        return 0f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("WaterMass"))
        {
            isInWater = true;
            DrainEnergy(waterMassEnergyDrain);
            Debug.Log("Character entered water.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("WaterMass"))
        {
            isInWater = false;
            Debug.Log("Character exited water.");
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
}
