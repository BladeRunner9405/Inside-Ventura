using UnityEngine;

namespace InsideVentura.AI
{
  public class EnemyBrain : MonoBehaviour
  {
    private EnemyState _currentState;
    private EnemyState _initialState; // Запоминаем стартовое состояние!
    private Enemy _enemy;

    public void Init(Enemy enemy, EnemyState initialState)
    {
      _enemy = enemy;
      _initialState = initialState; // Сохраняем для будущих воскрешений
      ChangeState(initialState);
    }

    public void ChangeState(EnemyState newState)
    {
      _currentState?.Exit();
      _currentState = newState;

      if (_currentState != null)
      {
        _currentState.Init(this, _enemy);
        _currentState.Enter();
      }
    }

    private void Update() => _currentState?.Update();

    private void FixedUpdate() => _currentState?.FixedUpdate();

    public void ResetBrain()
    {
      this.enabled = true;
      // Возвращаемся к изначальному состоянию (например, Погоне), а не к тому, на котором умерли
      if (_initialState != null)
      {
        ChangeState(_initialState);
      }
    }
  }
}
