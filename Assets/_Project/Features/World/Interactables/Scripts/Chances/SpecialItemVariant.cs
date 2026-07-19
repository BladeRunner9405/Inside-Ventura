using UnityEngine;

[CreateAssetMenu(fileName = "NewSpecialItemVariant", menuName = "InsideVentura/Chances/SpecialItemVariant")]
public class SpecialItemVariant : ScriptableObject
{
  public GameObject prefab;
  [Range(0f, 1f)] public float chance;
}
