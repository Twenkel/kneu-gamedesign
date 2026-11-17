using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    [Header("Дані")]
    [SerializeField] private EnemyStatsSO stats;

    [Header("Точки")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform edgeCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Transform attackPoint;

    [Header("Шари")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask damageableLayers;

    [Header("Звук")]
    [SerializeField] private AudioSource audioSource;

    [Header("Стрільба")]
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField, Range(5, 50)] private int poolCapacity = 20;

    private const float FacingDeadZone = 0.1f;
    private const float ProbeRadius = 0.1f;
    private static readonly int SpeedHash = Animator.StringToHash("Speed");

    private ObjectPool<Projectile> pool;

    public EnemyStatsSO Stats => stats;
    public Rigidbody2D Body { get; private set; }
    public Animator Animator { get; private set; }
    public StateMachine Machine { get; private set; }
    public Transform Target { get; private set; }

    public PatrolState PatrolState { get; private set; }
    public ChaseState ChaseState { get; private set; }
    public HurtState HurtState { get; private set; }
    public DeadState DeadState { get; private set; }
    public IState AttackState { get; private set; }

    public int Facing { get; private set; } = 1;

    private void Awake()
    {
        Body = GetComponent<Rigidbody2D>();
        Animator = GetComponentInChildren<Animator>();
        Machine = new StateMachine();
        Target = GameObject.FindGameObjectWithTag("Player").transform;

        GetComponent<Health>().Configure(stats.maxHealth);
        GetComponentInChildren<SpriteRenderer>().color = stats.tint;
        ApplyScale();

        PatrolState = new PatrolState(this);
        ChaseState = new ChaseState(this);
        HurtState = new HurtState(this);
        DeadState = new DeadState(this);
        AttackState = stats.ranged
            ? (IState)new RangedAttackState(this)
            : new MeleeAttackState(this);

        pool = new ObjectPool<Projectile>(
            createFunc: () => Instantiate(projectilePrefab),
            actionOnGet: p => p.gameObject.SetActive(true),
            actionOnRelease: p => p.gameObject.SetActive(false),
            actionOnDestroy: p => Destroy(p.gameObject),
            defaultCapacity: poolCapacity);
    }

    private void Start() => Machine.ChangeState(PatrolState);
    private void Update() => Machine.Tick();

    public float DistanceToTarget => Mathf.Abs(Target.position.x - transform.position.x);

    public bool TargetOnSameLevel =>
        Mathf.Abs(Target.position.y - groundCheck.position.y) <= stats.verticalTolerance;

    public bool IsGrounded => Physics2D.OverlapCircle(groundCheck.position, ProbeRadius, groundLayer);
    public bool AtEdge() => !Physics2D.OverlapCircle(edgeCheck.position, ProbeRadius, groundLayer);
    public bool AtWall() => Physics2D.OverlapCircle(wallCheck.position, ProbeRadius, groundLayer);

    public void Move(float speed)
    {
        Body.linearVelocity = new Vector2(Facing * speed, Body.linearVelocity.y);
        Animator.SetFloat(SpeedHash, Mathf.Abs(Body.linearVelocity.x));
    }

    public void Stop()
    {
        Body.linearVelocity = new Vector2(0f, Body.linearVelocity.y);
        Animator.SetFloat(SpeedHash, 0f);
    }

    public void Flip()
    {
        Facing = -Facing;
        ApplyScale();
    }

    private void ApplyScale() =>
        transform.localScale = new Vector3(stats.sizeScale * Facing, stats.sizeScale, 1f);

    public void FaceTarget()
    {
        float dx = Target.position.x - transform.position.x;
        if (Mathf.Abs(dx) < FacingDeadZone) return;
        int dir = dx > 0f ? 1 : -1;
        if (dir != Facing) Flip();
    }

    public void PlayTelegraph()
    {
        if (stats.telegraphSfx != null)
            audioSource.PlayOneShot(stats.telegraphSfx);
    }

    public void DealDamage()
    {
        Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, stats.attackRadius, damageableLayers);
        if (hit != null && hit.TryGetComponent(out IDamageable target))
            target.TakeDamage(stats.damage, transform.position);
    }

    public void Shoot()
    {
        Projectile p = pool.Get();
        p.transform.position = attackPoint.position;
        p.OnFinished = Release;
        p.Launch(new Vector2(Facing, 0f), stats.damage);
    }

    private void Release(Projectile p)
    {
        p.OnFinished = null;
        pool.Release(p);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (attackPoint != null)
            Gizmos.DrawWireSphere(attackPoint.position, stats != null ? stats.attackRadius : 0.5f);

        Gizmos.color = Color.cyan;
        if (groundCheck != null) Gizmos.DrawWireSphere(groundCheck.position, ProbeRadius);
        if (edgeCheck != null) Gizmos.DrawWireSphere(edgeCheck.position, ProbeRadius);
        if (wallCheck != null) Gizmos.DrawWireSphere(wallCheck.position, ProbeRadius);
    }
}
