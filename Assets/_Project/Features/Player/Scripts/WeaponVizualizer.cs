using UnityEngine;

public class WeaponVizualizer : MonoBehaviour
{
  [SerializeField] private PlayerEquipment equipment;

  private Animator _animator;
  private readonly int _animAttack = Animator.StringToHash("Attack");
  private readonly int _animSpecialAttack = Animator.StringToHash("SpecialAttack");

  private void Awake()
  {
    _animator = GetComponent<Animator>();
    equipment.OnInstancesInitialized += OnWeaponInitialized;
  }

  private void OnDestroy() {
    equipment.OnInstancesInitialized -= OnWeaponInitialized;
    equipment.Weapon.OnAttack -= PlayAttackAnimation;
  }

  private void OnWeaponInitialized() {
    equipment.Weapon.OnAttack += PlayAttackAnimation;
  }

  private void PlayAttackAnimation(bool isCombo)
  {
    if (isCombo)
    {
      _animator.SetTrigger(_animSpecialAttack);
    }
    else
    {
      _animator.SetTrigger(_animAttack);
    }
  }
}
