public class Status
{
    public string Name { get; private set; } // Name of the status
    public float Duration { get; private set; } // Duration of the status in seconds
    public float CooldownDuration { get; private set; } // Cooldown time in seconds
    public bool IsActive { get; private set; } // Whether the status is currently active
    public bool IsOnCooldown { get; private set; } // Whether the status is on cooldown

    private float activeTimer; // Timer to track the active duration
    private float cooldownTimer; // Timer to track the cooldown duration

    public System.Action OnActivate; // Callback for when the status is activated
    public System.Action OnDeactivate; // Callback for when the status is deactivated
    public System.Action OnCooldownEnd; // Callback for when the cooldown ends

    public Status(string name, float duration, float cooldownDuration)
    {
        Name = name;
        Duration = duration;
        CooldownDuration = cooldownDuration;
        IsActive = false;
        IsOnCooldown = false;
        activeTimer = 0f;
        cooldownTimer = 0f;
    }

    public void Activate()
    {
        if (IsActive || IsOnCooldown) return; // Prevent activation if already active or on cooldown

        IsActive = true;
        activeTimer = 0f;
        OnActivate?.Invoke();
    }

    public void Update(float deltaTime)
    {
        if (IsActive)
        {
            // Update the active timer
            activeTimer += deltaTime;
            if (activeTimer >= Duration)
            {
                Deactivate();
            }
        }
        else if (IsOnCooldown)
        {
            // Update the cooldown timer
            cooldownTimer += deltaTime;
            if (cooldownTimer >= CooldownDuration)
            {
                EndCooldown();
            }
        }
    }

    public void Deactivate()
    {
        if (!IsActive) return;

        IsActive = false;
        IsOnCooldown = true;
        cooldownTimer = 0f;
        OnDeactivate?.Invoke();
    }

    private void EndCooldown()
    {
        IsOnCooldown = false;
        OnCooldownEnd?.Invoke();
    }
}
