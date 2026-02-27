using UnityEngine;

public abstract class Accessory : Artifact
{
  [SerializeField] private float cooldown = 3f;

  public abstract void UseAbility();
}
