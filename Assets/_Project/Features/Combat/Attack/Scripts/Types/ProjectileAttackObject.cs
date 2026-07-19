using UnityEngine;

public class ProjectileAttackObject : AttackObject
{
    [Header("Projectile Settings")]
    public float speed = 0.7f;
    private Vector2 _moveDirection;

    public override void Initialize(float damage, LayerMask layer, Vector2 direction)
    {
        base.Initialize(damage, layer, direction);
        _moveDirection = direction.normalized;
    }

    protected override void Update()
    {
        base.Update();
        // Двигаем шар каждый кадр
        transform.Translate(_moveDirection * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        // Проверяем, попали ли мы в цель (Игрока)
        if (((1 << col.gameObject.layer) & targetLayer) != 0)
        {
            TryDealDamage(col);
            Despawn(); // Прячем шар обратно в пул
        }
        // Если попали в стену (Слой Obstacle)
        else if (col.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            Despawn(); // Просто исчезаем
        }
    }
}