using UnityEngine;

public abstract class Accessory : Artifact {
  [SerializeField] private float cooldown;

  private float lastUseTime;

  public float Cooldown => cooldown;

  public override void Initialize(GameObject player) {
    base.Initialize(player);
    lastUseTime = -cooldown;
  }

  protected bool CanUse() {
    return Time.time >= lastUseTime + cooldown;
  }

  public void TryUseAbility(GameObject player, Vector2 direction) {
    if (!CanUse()) return;

    UseAbility(player, direction);
    lastUseTime = Time.time; // фиксируем время использования
  }

  protected abstract void UseAbility(GameObject player, Vector2 direction);
}
