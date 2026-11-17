using UnityEngine;

public class DeadState : IState
{
    private static readonly int DieHash = Animator.StringToHash("Die");

    private readonly EnemyController ctx;

    public DeadState(EnemyController ctx) => this.ctx = ctx;

    public void Enter()
    {
        ctx.Stop();
        ctx.Animator.SetTrigger(DieHash);
    }

    public void Tick() { }
    public void Exit() { }
}
