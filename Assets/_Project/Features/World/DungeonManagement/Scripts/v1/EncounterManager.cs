using System;
using System.Collections;
using System.Collections.Generic;
using CherryFramework.SimplePool; // Предполагается наличие этого пространства имен
using UnityEngine;

namespace InsideVentura.World.v1
{
    public class EncounterManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private GameObject chestPrefab;
        [SerializeField] private RoomBuilder builder;
        
        [Tooltip("Задержка между появлением отдельных врагов в секундах")]
        [SerializeField] private float spawnInterval = 0.5f;

        // Состояние боя
        private EncounterData _currentEncounter;
        private List<Transform> _spawnPoints;
        private Action _onEncounterCleared;

        private int _currentWaveIndex;
        private Queue<GameObject> _enemiesToSpawn = new Queue<GameObject>();
        private List<Enemy> _activeEnemies = new List<Enemy>();

        // Пулинг и учет объектов
        private SimplePool<Enemy> _enemyPool = new SimplePool<Enemy>();
        private List<Enemy> _allSpawnedEnemies = new List<Enemy>();

        // Ссылка на корутину спавна для контроля процесса
        private Coroutine _spawnCoroutine;

        /// <summary>
        /// Позволяет узнать, идет ли сейчас бой (есть враги на арене или в очереди на спавн)
        /// </summary>
        public bool IsEncounterActive => _activeEnemies.Count > 0 || _enemiesToSpawn.Count > 0;

        /// <summary>
        /// Запускает боевое событие в комнате
        /// </summary>
        public void StartEncounter(EncounterData encounter, List<Transform> spawnPoints, Action onCleared)
        {
            _currentEncounter = encounter;
            _spawnPoints = spawnPoints;
            _onEncounterCleared = onCleared;

            _currentWaveIndex = 0;
            _activeEnemies.Clear();
            _enemiesToSpawn.Clear();
            _allSpawnedEnemies.Clear();

            if (_currentEncounter == null || _currentEncounter.waves.Count == 0 || _spawnPoints.Count == 0)
            {
                Debug.LogWarning("<color=yellow>[Encounter]</color> Нет данных для боя или точек спавна. Завершение.");
                FinishEncounter();
                return;
            }

            Debug.Log($"<color=red>[Encounter]</color> Начало боя! Всего волн: {_currentEncounter.waves.Count}");
            StartWave(_currentWaveIndex);
        }

        private void StartWave(int waveIndex)
        {
            Debug.Log($"<color=orange>[Encounter]</color> Запуск волны {waveIndex + 1}");
            
            Wave currentWave = _currentEncounter.waves[waveIndex];
            _enemiesToSpawn.Clear();

            // Заполняем очередь префабами врагов из текущей волны
            foreach (var waveEnemy in currentWave.enemies)
            {
                for (int i = 0; i < waveEnemy.count; i++)
                {
                    _enemiesToSpawn.Enqueue(waveEnemy.enemyPrefab);
                }
            }

            // Запускаем процесс постепенного появления врагов
            RefreshSpawnProcess();
        }

        private void RefreshSpawnProcess()
        {
            // Если корутина уже работает, не запускаем вторую
            if (_spawnCoroutine != null) return;

            _spawnCoroutine = StartCoroutine(SpawnEnemiesRoutine());
        }

        private IEnumerator SpawnEnemiesRoutine()
        {
            // Пока есть кого спавнить И не превышен лимит врагов на арене
            while (_enemiesToSpawn.Count > 0 && _activeEnemies.Count < _currentEncounter.maxConcurrentEnemies)
            {
                SpawnSingleEnemy();

                // Делаем паузу перед следующим врагом для эффекта постепенного появления
                if (spawnInterval > 0)
                {
                    yield return new WaitForSeconds(spawnInterval);
                }
            }

            // Корутина завершила работу (либо очередь пуста, либо достигнут лимит)
            _spawnCoroutine = null;
        }

        private void SpawnSingleEnemy()
        {
            if (_enemiesToSpawn.Count == 0) return;

            GameObject enemyPrefabObj = _enemiesToSpawn.Dequeue();
            Enemy enemySample = enemyPrefabObj.GetComponent<Enemy>();
            Transform randomSpawnPoint = _spawnPoints[UnityEngine.Random.Range(0, _spawnPoints.Count)];

            // Достаем врага из пула
            Enemy enemyInstance = _enemyPool.Get(
                enemySample,
                randomSpawnPoint.position,
                Quaternion.identity
            );

            enemyInstance.gameObject.SetActive(true);
            
            // Важно: Сброс состояния врага (HP, анимации) перед использованием
            enemyInstance.ResetEntity();

            _activeEnemies.Add(enemyInstance);
            _allSpawnedEnemies.Add(enemyInstance);

            // Подписываемся на смерть. 
            // ВАЖНО: Мы используем метод напрямую. Если OnDeath — это Action, 
            // внутри самого Enemy событие ДОЛЖНО очищаться в OnDisable или ResetEntity, 
            // иначе при повторном использовании из пула возникнут утечки и двойные вызовы.
            enemyInstance.OnDeath += () => OnEnemyDied(enemyInstance);
        }

        private void OnEnemyDied(Enemy enemy)
        {
            if (!_activeEnemies.Contains(enemy)) return;

            _activeEnemies.Remove(enemy);

            // Если эта волна полностью зачищена
            if (_enemiesToSpawn.Count == 0 && _activeEnemies.Count == 0)
            {
                _currentWaveIndex++;

                if (_currentWaveIndex < _currentEncounter.waves.Count)
                {
                    StartWave(_currentWaveIndex);
                }
                else
                {
                    FinishEncounter();
                }
            }
            else
            {
                // Место освободилось — проверяем, нужно ли доспавнить оставшихся в очереди
                RefreshSpawnProcess();
            }
        }

        private void FinishEncounter()
        {
            // Останавливаем спавн, если он еще шел (на случай принудительного завершения)
            if (_spawnCoroutine != null)
            {
                StopCoroutine(_spawnCoroutine);
                _spawnCoroutine = null;
            }

            Debug.Log("<color=cyan>[Encounter]</color> Все волны зачищены!");

            SpawnReward();
            
            // Открываем двери в комнате через RoomBuilder
            if (builder != null)
            {
                builder.ApplyDoorState(true);
            }

            _onEncounterCleared?.Invoke();
        }

        private void SpawnReward()
        {
            if (builder != null && builder.RewardPoint != null && chestPrefab != null)
            {
                GameObject chest = Instantiate(chestPrefab, builder.RewardPoint.position, Quaternion.identity);

                // Регистрируем сундук в DungeonManager, чтобы он сохранялся/скрывался при переходах
                if (DungeonManager.Instance != null)
                {
                    DungeonManager.Instance.RegisterRoomObject(chest);
                }

                Debug.Log("<color=yellow>[Encounter]</color> Сундук заспавнен в точке награды.");
            }
        }

        /// <summary>
        /// Вызывается DungeonManager-ом при уходе из комнаты, чтобы вернуть трупы врагов в пул
        /// </summary>
        public void ClearCorpses()
        {
            // Останавливаем спавн принудительно
            if (_spawnCoroutine != null)
            {
                StopCoroutine(_spawnCoroutine);
                _spawnCoroutine = null;
            }

            foreach (var enemy in _allSpawnedEnemies)
            {
                if (enemy != null && enemy.gameObject.activeSelf)
                {
                    // В большинстве систем пулинга деактивация возвращает объект в пул
                    enemy.gameObject.SetActive(false);
                }
            }
            
            _allSpawnedEnemies.Clear();
            _activeEnemies.Clear();
            _enemiesToSpawn.Clear();
        }
    }
}