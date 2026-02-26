using UnityEngine;

[CreateAssetMenu(fileName = "WeaponName", menuName = "Inside-Ventura/Artifacts/Weapon")]
public class Weapon : Artifact
{
  [SerializeField] private float attackSpeed = 1f;
  [SerializeField] private float damage = 10f;

  public virtual void Attack(IPlayer player)
  {
    Debug.Log($"Weapon {artifactName} attacks with damage {damage}");
  }
}
