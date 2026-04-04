using UnityEngine;
using System;

public class AttackAnimator : MonoBehaviour
{
    [SerializeField] private AttackVisualData data;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Direction Settings")]
    [Tooltip("Нужно ли физически поворачивать спрайт по вектору атаки? (Да - для меча, Нет - для волн/вспышек)")]
    [SerializeField] private bool rotateToDirection = false;
    
    [Tooltip("Отзеркаливать ли спрайт, если атака идет влево?")]
    [SerializeField] private bool flipOnLeftDirection = true;

    private float _timer;
    private int _currentFrame;
    private bool _isPlaying;
    
    private Action _onDamageFrame;
    private Action _onEnd;

    public void Play(Action onDamage, Action onEnd, Vector2 direction)
    {
        if (data == null || data.frames.Length == 0)
        {
            onDamage?.Invoke();
            onEnd?.Invoke();
            return;
        }

        _onDamageFrame = onDamage;
        _onEnd = onEnd;
        
        _currentFrame = 0;
        _timer = 0;
        _isPlaying = true;

        if (spriteRenderer != null)
        {
            if (rotateToDirection)
            {
                // Вычисляем угол в градусах из вектора направления
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                
                // Вращаем ТОЛЬКО SpriteRenderer, а не весь AttackObject
                spriteRenderer.transform.rotation = Quaternion.Euler(0, 0, angle);

                // Если мы повернули спрайт влево (угол ~180), он окажется вверх ногами. 
                // Поэтому при повороте мы флипаем его по оси Y, а не X!
                if (flipOnLeftDirection)
                {
                    spriteRenderer.flipY = direction.x < 0;
                }
            }
            else
            {
                // Если не вращаем (например, Круговая атака), то просто отзеркаливаем по X
                if (flipOnLeftDirection)
                {
                    spriteRenderer.flipX = direction.x < 0;
                }
            }

            spriteRenderer.sprite = data.frames[0];
        }
    }

    private void Update()
    {
        if (!_isPlaying) return;

        _timer += Time.deltaTime;
        if (_timer >= data.animationSpeed)
        {
            _timer -= data.animationSpeed;
            _currentFrame++;

            if (_currentFrame == data.damageFrame)
            {
                _onDamageFrame?.Invoke();
            }

            if (_currentFrame >= data.frames.Length)
            {
                _isPlaying = false;
                _onEnd?.Invoke();
                return;
            }

            spriteRenderer.sprite = data.frames[_currentFrame];
        }
    }
}