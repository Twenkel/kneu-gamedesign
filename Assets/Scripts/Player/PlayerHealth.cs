using UnityEngine;

public class PlayerHealth : Health
{
    [SerializeField] private PlayerRespawn respawn;

    [Header("Канали подій")]
    [SerializeField] private IntEventSO healthChanged;
    [SerializeField] private IntEventSO maxHealthChanged;

    private void Start()
    {
        maxHealthChanged.Raise(Max);
        healthChanged.Raise(Current);
    }

    protected override void OnHealthChanged()
    {
        if (healthChanged != null) healthChanged.Raise(Current);
    }

    protected override void Die()
    {
        respawn.Respawn();
        ResetHealth();
    }
}
