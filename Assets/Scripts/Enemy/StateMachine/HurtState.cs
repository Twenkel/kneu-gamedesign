using UnityEngine;

public class HurtState : IState
{
    private static readonly int HurtHash = Animator.StringToHash("Hurt");

    private readonly EnemyController ctx;
    private float timer;

    public HurtState(EnemyController ctx) => this.ctx = ctx;

    public void Enter()
    {
        timer = ctx.Stats.hurtTime;
        ctx.Animator.SetTrigger(HurtHash);
    }

    public void Tick()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f) ctx.Machine.ChangeState(ctx.ChaseState);
    }

    public void Exit() => ctx.Animator.ResetTrigger(HurtHash);
}
