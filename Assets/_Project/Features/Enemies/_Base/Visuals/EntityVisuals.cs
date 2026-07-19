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

  // 1. Запоминаем цвет один раз при создании префаба
  protected virtual void Awake()
  {
    // Проверка на null, если spriteRenderer не назначен в инспекторе
    if (spriteRenderer != null)
    {
      _originalColor = spriteRenderer.color;
    }
  }

  protected override void OnEnable()
  {
    base.OnEnable();

    // 2. Мгновенно возвращаем нормальный цвет при доставании из пула
    spriteRenderer.color = _originalColor;

    // Подписываемся на события
    entity.OnTakeDamage += PlayDamageEffect;
    entity.OnDeath += PlayDeathEffect;
    entity.OnAppear += PlayAppearEffect;
  }

  // 3. ОТПИСКА! Критически важно для пулов
  protected virtual void OnDisable()
  {
    entity.OnTakeDamage -= PlayDamageEffect;
    entity.OnDeath -= PlayDeathEffect;
    entity.OnAppear -= PlayAppearEffect;

    // Убиваем анимацию, если объект выключили во время получения урона
    _damageSequence?.Kill();
  }

  private void PlayDamageEffect(float damage)
  {
    if (damage <= 0)
      return;

    _damageSequence?.Kill();
    _damageSequence = DOTween.Sequence();
    _damageSequence.Append(spriteRenderer.DOColor(Color.red, damageFlashDuration));
    _damageSequence.Append(spriteRenderer.DOColor(_originalColor, damageFlashDuration));
  }

  private void PlayDeathEffect()
  {
    _damageSequence?.Kill(); // Останавливаем мигание урона, если оно шло
    spriteRenderer.DOColor(Color.gray, 0.5f);
  }

  private void PlayAppearEffect()
  {
    // Если хочешь плавное появление из прозрачности, можно сделать так:
    // spriteRenderer.color = new Color(_originalColor.r, _originalColor.g, _originalColor.b, 0f);
    // spriteRenderer.DOColor(_originalColor, 1f);
  }
}
