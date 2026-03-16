using UnityEngine;

public abstract class Accessory : Artifact {
  [SerializeField] private ModifiableStat cooldown = new(3f);

  private float _lastUseTime;

  public float Cooldown => cooldown.Value;

  public override ModifiableStat GetStat(StatName statName) {
    if (statName == StatName.Cooldown)
      return cooldown;
    return base.GetStat(statName);
  }

  public override void Initialize() {
    base.Initialize();
    _lastUseTime = -Cooldown;
  }

  protected bool CanUse() {
    return Time.time >= _lastUseTime + Cooldown;
  }

  public void TryUseAbility(Vector2 direction) {
    if (!CanUse()) return;

    UseAbility(direction);
    _lastUseTime = Time.time;
  }

  protected abstract void UseAbility(Vector2 direction);
}
