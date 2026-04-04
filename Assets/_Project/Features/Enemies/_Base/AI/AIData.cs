using UnityEngine;

public class AIData
{
  public float[] Interest = new float[8];
  public float[] Danger = new float[8];

  // Направления: Вверх, Вверх-Вправо, Вправо и т.д.
  public static readonly Vector2[] Directions =
  {
    Vector2.up,
    new Vector2(1, 1).normalized,
    Vector2.right,
    new Vector2(1, -1).normalized,
    Vector2.down,
    new Vector2(-1, -1).normalized,
    Vector2.left,
    new Vector2(-1, 1).normalized,
  };
}
