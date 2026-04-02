using CherryFramework.BaseClasses;
using DG.Tweening;
using UnityEngine;

public class EntityVisuals : BehaviourBase
{
  [SerializeField]
  private Entity entity;

  [SerializeField]
  private SpriteRenderer spriteRenderer;

  [SerializeField]
  private float damageFlashDuration = 0.1f;

  private Color _originalColor;
  private Sequence _damageSequence;

  protected override void OnEnable()
  {
    base.OnEnable();
    _originalColor = spriteRenderer.color;

    // Подписываемся на события логики
    entity.OnTakeDamage += PlayDamageEffect;
    entity.OnDeath += PlayDeathEffect;
  }

  private void PlayDamageEffect(float damage)
  {
    if (damage <= 0)
      return; // Можно добавить эффект уворота (Miss!)

    _damageSequence?.Kill();
    _damageSequence = DOTween.Sequence();
    _damageSequence.Append(spriteRenderer.DOColor(Color.red, damageFlashDuration));
    _damageSequence.Append(spriteRenderer.DOColor(_originalColor, damageFlashDuration));
  }

  private void PlayDeathEffect()
  {
    spriteRenderer.DOColor(Color.gray, 0.5f);
    // Тут же можно спавнить частицы смерти
  }
}
