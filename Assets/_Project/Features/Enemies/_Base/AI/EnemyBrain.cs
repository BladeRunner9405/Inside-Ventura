using UnityEngine;

namespace InsideVentura.AI
{
    public class EnemyBrain : MonoBehaviour
    {
        private EnemyState _currentState;
        private Enemy _enemy;

        public void Init(Enemy enemy, EnemyState initialState)
        {
            _enemy = enemy;
            ChangeState(initialState);
        }

        public void ChangeState(EnemyState newState)
        {
            _currentState?.Exit();
            _currentState = newState;
            _currentState.Init(this, _enemy);
            _currentState.Enter();
        }

        private void Update() => _currentState?.Update();
        private void FixedUpdate() => _currentState?.FixedUpdate();
    }
}