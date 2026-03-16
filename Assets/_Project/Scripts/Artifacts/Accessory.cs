using UnityEngine;

public abstract class Accessory : Artifact {
  [SerializeField] private float cooldown;

  private float lastUseTime;

  public float Cooldown => cooldown;

  public override void Initialize() {
    base.Initialize();
    lastUseTime = -cooldown;
  }

  protected bool CanUse() {
    return Time.time >= lastUseTime + cooldown;
  }

  public void TryUseAbility(Vector2 direction) {
    if (!CanUse()) return;

    UseAbility(direction);
    lastUseTime = Time.time; // фиксируем время использования
  }

  protected abstract void UseAbility(Vector2 direction);
}
