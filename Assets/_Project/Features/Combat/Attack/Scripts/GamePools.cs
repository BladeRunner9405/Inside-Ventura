using CherryFramework.SimplePool;

public static class GamePools {
  public static readonly SimplePool<AttackObject> Hitboxes = new();
  public static readonly SimplePool<FloatingDamage> FloatingDamages = new();
}
