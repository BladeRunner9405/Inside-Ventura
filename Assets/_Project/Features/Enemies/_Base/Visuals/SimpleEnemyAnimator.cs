using InsideVentura.AI;
using UnityEngine;

public class SimpleEnemyAnimator : MonoBehaviour
{
  [SerializeField]
  private EnemyVisualData data;

  [SerializeField]
  private SpriteRenderer spriteRenderer;

  private Enemy _enemy;
  private float _timer;
  private int _currentFrame;
  private Sprite[] _activeSprites;

  private bool _isPlayingAction;
  private System.Action _onDamageFrameReached;
  private System.Action _onAnimationEnded;

  private void Awake()
  {
    _enemy = GetComponentInParent<Enemy>();
    _activeSprites = data.idleSprites;
  }

  public void PlayAction(Sprite[] actionSprites, System.Action onDamage, System.Action onEnd)
  {
    _activeSprites = actionSprites;
    _currentFrame = 0;
    _timer = 0;
    _isPlayingAction = true;
    _onDamageFrameReached = onDamage;
    _onAnimationEnded = onEnd;
  }

  private void Update()
  {
    if (_enemy.IsDead)
    {
      spriteRenderer.sprite = data.deathSprite;
      return;
    }

    HandleFlip();
    if (!_isPlayingAction)
      UpdateMovementState();
    PlayFrames();
  }

  private void UpdateMovementState()
  {
    // Увеличим порог до 0.1, чтобы анимация ходьбы включалась только при реальном движении
    bool isMoving = _enemy.CurrentMoveDirection.magnitude > 0.1f || _enemy.IsDashing;

    SetSprites(isMoving ? data.walkSprites : data.idleSprites);
  }

  private void SetSprites(Sprite[] newSprites)
  {
    if (_activeSprites == newSprites)
      return;
    _activeSprites = newSprites;
    _currentFrame = 0;
    _timer = 0;
  }

  private void PlayFrames()
  {
    if (_activeSprites == null || _activeSprites.Length == 0)
      return;

    _timer += Time.deltaTime;
    if (_timer >= data.animationSpeed)
    {
      _timer = 0;
      _currentFrame++;
      // Проверка кадра урона
      if (_isPlayingAction && _currentFrame == data.attackDamageFrame)
        _onDamageFrameReached?.Invoke();

      // Конец анимации
      if (_currentFrame >= _activeSprites.Length)
      {
        if (_isPlayingAction)
        {
          _isPlayingAction = false;
          _onAnimationEnded?.Invoke();
        }
        _currentFrame = 0;
      }

      spriteRenderer.sprite = _activeSprites[_currentFrame];
    }
  }

  public void PlayAttack(System.Action onDamage, System.Action onEnd)
  {
    // Теперь аниматор сам знает, что брать data.attackSprites
    PlayAction(data.attackSprites, onDamage, onEnd);
  }

  private void HandleFlip()
  {
    // Опционально: можно флипать спрайт не только на игрока,
    // но и по направлению движения, если игрока нет в зоне видимости
    if (_enemy.target != null)
    {
      spriteRenderer.flipX = _enemy.target.position.x < transform.position.x;
    }
    else if (_enemy.CurrentMoveDirection.x != 0)
    {
      spriteRenderer.flipX = _enemy.CurrentMoveDirection.x < 0;
    }
  }

  public void ResetToIdle()
  {
    _isPlayingAction = false;
    _onDamageFrameReached = null;
    _onAnimationEnded = null;

    _timer = 0f;
    _currentFrame = 0;

    // Возвращаем дефолтные спрайты
    if (data != null && data.idleSprites.Length > 0)
    {
      _activeSprites = data.idleSprites;
      spriteRenderer.sprite = _activeSprites[0];
    }
  }
}
