using UnityEngine;

public class EnemyHealth : Health
{
    [SerializeField] private EnemyController controller;
    [SerializeField] private Collider2D hitbox;
    [SerializeField, Range(0.2f, 3f)] private float corpseLifetime = 1.2f;

    protected override void OnDamaged()
    {
        controller.Machine.ChangeState(controller.HurtState);
    }

    protected override void Die()
    {
        controller.Machine.ChangeState(controller.DeadState);
        hitbox.enabled = false;
        controller.enabled = false;
        Destroy(gameObject, corpseLifetime);
    }
}
