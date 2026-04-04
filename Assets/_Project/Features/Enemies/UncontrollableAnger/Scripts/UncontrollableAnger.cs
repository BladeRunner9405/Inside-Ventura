using InsideVentura.AI;
using UnityEngine;

[RequireComponent(typeof(EnemyBrain))]
public class UncontrollableAnger : Enemy
{
  [Header("Distances")]
  public float playerDistance = 2.7f;
  public float retreatDistance = 2f;

  [Header("Attack Settings")]
  public float cooldownDuration = 3f;

  [SerializeField]
  private AttackObject waveAttackPrefab;

  private float _curCooldown;
  public bool IsAttackReady => _curCooldown <= 0;

  private Vector2 _attackDir;

  protected override void Awake()
  {
    base.Awake();
  }

  protected override void Start()
  {
    base.Start();
    Brain.Init(this, new UA_ChaseState());
  }

  private void Update()
  {
    if (IsDead)
      return;
    if (_curCooldown > 0)
      _curCooldown -= Time.deltaTime;
  }

  // Метод начала атаки, который вызовет наше состояние
  // Замени StartAttackSequence на это:
  public override void Attack(Vector2 direction)
  {
    // Врагу направление пока не особо нужно (он сам смотрит на target в UADoDamage),
    // но мы соблюдаем контракт Entity.
    _attackDir = (target.position - transform.position).normalized;
    _view.PlayAttack(UADoDamage, UAStartCooldown);
  }

  private void UADoDamage()
  {
    if (target == null)
      return;

    // СТАВИМ Quaternion.identity! Никаких Atan2!
    var attackObj = GamePools.Hitboxes.Get(
      waveAttackPrefab,
      transform.position,
      Quaternion.identity
    );
    attackObj.gameObject.SetActive(true);

    // Вызываем новый Initialize, передавая dir
    attackObj.Initialize(damage, LayerMask.GetMask("Player"), _attackDir);
  }

  private void UAStartCooldown()
  {
    _curCooldown = cooldownDuration;
    Brain.ChangeState(new UA_ChaseState());
  }
}
