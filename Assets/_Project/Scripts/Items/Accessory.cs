using UnityEngine;

[CreateAssetMenu(fileName = "HeartName", menuName = "Inside-Ventura/Artifacts/Accessory")]
public abstract class Accessory : Artifact
{
  [SerializeField] private float cooldown = 3f;

  public abstract void UseAbility(IPlayer player);
}
