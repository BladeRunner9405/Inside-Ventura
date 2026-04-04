using UnityEngine;

[CreateAssetMenu(fileName = "NewAttackVisual", menuName = "Inside-Ventura/AttackVisual")]
public class AttackVisualData : ScriptableObject
{
  public Sprite[] frames;
  public float animationSpeed = 0.05f;

  [Tooltip("На каком кадре нанести урон (начиная с 0)?")]
  public int damageFrame = 2;
}
