using CherryFramework.DependencyManager;

public class Enemy : Entity {
  public int damage;
  public bool isBoss;

  [Inject] private PlayerAccessor _playerAccessor;

  protected void Start() {
    var player = _playerAccessor.Player.transform;

    TargetTo(player);
  }

  private void FixedUpdate() {
    if (target && !IsDead) Move((target.position - transform.position).normalized);
  }
}
