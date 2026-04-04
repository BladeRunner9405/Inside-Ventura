using UnityEngine;
using System;

namespace InsideVentura.Player
{
    public class PlayerAnimator : MonoBehaviour
    {
        [SerializeField] private PlayerVisualData data;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private global::Player _player; 
        private float _timer;
        private int _currentFrame;
        private Sprite[] _activeSprites;
        
        private bool _isPlayingAction;
        private Action _onActionEnded;

        private void Awake()
        {
            _player = GetComponentInParent<global::Player>();
            _activeSprites = data.idleSprites;
        }

        /// <summary>
        /// Вызов жесткой анимации (например, атака мечом), которая перебивает ходьбу
        /// </summary>
        public void PlayAction(Sprite[] actionSprites, Action onEnd = null)
        {
            if (actionSprites == null || actionSprites.Length == 0)
            {
                onEnd?.Invoke();
                return;
            }
            
            _activeSprites = actionSprites;
            _currentFrame = 0;
            _timer = 0;
            _isPlayingAction = true;
            _onActionEnded = onEnd;
        }

        private void Update()
        {
            if (_player.IsDead)
            {
                spriteRenderer.sprite = data.deathSprite;
                return;
            }

            // Игрок всегда смотрит туда, куда целится
            HandleFlip();

            // Автоматическое переключение ходьбы/айдла/дэша
            if (!_isPlayingAction) 
            {
                UpdateMovementState();
            }

            PlayFrames();
        }

        private void UpdateMovementState()
        {
            Sprite[] targetSprites = data.idleSprites;

            if (_player.IsDashing && data.dashSprites.Length > 0)
            {
                targetSprites = data.dashSprites;
            }
            else if (_player.CurrentMoveDirection.sqrMagnitude > 0.001f)
            {
                targetSprites = data.walkSprites;
            }

            // Меняем массив только если состояние реально изменилось
            if (_activeSprites != targetSprites)
            {
                _activeSprites = targetSprites;
                _currentFrame = 0;
                _timer = 0;
            }
        }

        private void PlayFrames()
        {
            if (_activeSprites == null || _activeSprites.Length == 0) return;

            _timer += Time.deltaTime;
            if (_timer >= data.animationSpeed)
            {
                // Вычитаем скорость вместо обнуления для более точного тайминга
                _timer -= data.animationSpeed; 
                _currentFrame++;

                // Если анимация закончилась
                if (_currentFrame >= _activeSprites.Length)
                {
                    if (_isPlayingAction)
                    {
                        _isPlayingAction = false;
                        _onActionEnded?.Invoke();
                    }
                    _currentFrame = 0; // Зацикливаем или сбрасываем
                }

                // Защита от выхода за границы
                if (_currentFrame < _activeSprites.Length)
                {
                    spriteRenderer.sprite = _activeSprites[_currentFrame];
                }
            }
        }

        private void HandleFlip()
        {
            // У игрока есть поле 'target' унаследованное от Entity.
            // Туда смотрит AimTarget. Мы разворачиваем спрайт относительно этого таргета!
            if (_player.target != null)
            {
                spriteRenderer.flipX = _player.target.position.x < transform.position.x;
            }
            // Запасной вариант, если таргета нет - флип по движению
            else if (_player.CurrentMoveDirection.x != 0)
            {
                spriteRenderer.flipX = _player.CurrentMoveDirection.x < 0;
            }
        }
    }
}