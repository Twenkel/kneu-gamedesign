using UnityEngine;

public class MeleeAttackState : IState
{
    private static readonly int TelegraphHash = Animator.StringToHash("Telegraph");

    private readonly EnemyController ctx;
    private float timer;
    private bool struck;

    public MeleeAttackState(EnemyController ctx) => this.ctx = ctx;

    public void Enter()
    {
        ctx.Stop();
        ctx.FaceTarget();
        struck = false;
        timer = ctx.Stats.telegraphTime;
        ctx.Animator.SetTrigger(TelegraphHash);
        ctx.PlayTelegraph();
    }

    public void Tick()
    {
        timer -= Time.deltaTime;
        if (timer > 0f) return;

        if (!struck)
        {
            ctx.DealDamage();
            struck = true;
            timer = ctx.Stats.recoveryTime;
            return;
        }

        ctx.Machine.ChangeState(ctx.ChaseState);
    }

    public void Exit() => ctx.Animator.ResetTrigger(TelegraphHash);
}
