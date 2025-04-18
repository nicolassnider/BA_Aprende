/// <summary>
/// Represents a status effect with a duration and cooldown.
/// </summary>
public class Status
{
    /// <summary>
    /// Gets the name of the status.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the duration of the status in seconds.
    /// </summary>
    public float Duration { get; private set; }

    /// <summary>
    /// Gets the cooldown time of the status in seconds.
    /// </summary>
    public float CooldownDuration { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the status is currently active.
    /// </summary>
    public bool IsActive => activeTimer > 0;

    /// <summary>
    /// Gets a value indicating whether the status is currently on cooldown.
    /// </summary>
    public bool IsOnCooldown => cooldownTimer > 0;

    private float activeTimer; // Tracks the active duration of the status.
    private float cooldownTimer; // Tracks the cooldown duration of the status.

    /// <summary>
    /// Occurs when the status is activated.
    /// </summary>
    public System.Action OnActivate;

    /// <summary>
    /// Occurs when the status is deactivated.
    /// </summary>
    public System.Action OnDeactivate;

    /// <summary>
    /// Occurs when the cooldown ends.
    /// </summary>
    public System.Action OnCooldownEnd;

    /// <summary>
    /// Initializes a new instance of the <see cref="Status"/> class.
    /// </summary>
    /// <param name="name">The name of the status.</param>
    /// <param name="duration">The duration of the status in seconds.</param>
    /// <param name="cooldownDuration">The cooldown time of the status in seconds.</param>
    public Status(string name, float duration, float cooldownDuration)
    {
        Name = name;
        Duration = duration;
        CooldownDuration = cooldownDuration;
        activeTimer = 0f;
        cooldownTimer = 0f;
    }

    /// <summary>
    /// Activates the status if it is not already active or on cooldown.
    /// </summary>
    public void Activate()
    {
        if (IsActive || IsOnCooldown) return;

        activeTimer = Duration;
        OnActivate?.Invoke();
    }

    /// <summary>
    /// Updates the status timers for active duration and cooldown.
    /// </summary>
    /// <param name="deltaTime">The time elapsed since the last update.</param>
    public void Update(float deltaTime)
    {
        // Update active timer
        if (activeTimer > 0)
        {
            activeTimer -= deltaTime;
            if (activeTimer <= 0)
            {
                activeTimer = 0;
                StartCooldown();
                OnDeactivate?.Invoke();
            }
        }

        // Update cooldown timer
        if (cooldownTimer > 0)
        {
            cooldownTimer -= deltaTime;
            if (cooldownTimer <= 0)
            {
                cooldownTimer = 0;
                OnCooldownEnd?.Invoke();
            }
        }
    }

    /// <summary>
    /// Starts the cooldown timer for the status.
    /// </summary>
    private void StartCooldown()
    {
        cooldownTimer = CooldownDuration;
    }
}