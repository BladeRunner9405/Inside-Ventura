using System;
using System.Collections.Generic;
using UnityEngine;
using CherryFramework.SimplePool; // Подключаем пулы

namespace InsideVentura.World
{
    public class EncounterManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private GameObject chestPrefab;

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

        /// <summary>
        /// Позволяет узнать, идет ли сейчас бой
        /// </summary>
        public bool IsEncounterActive => _activeEnemies.Count > 0 || _enemiesToSpawn.Count > 0;

        /// <summary>
        /// Запускает бой в новой комнате
        /// </summary>
        public void StartEncounter(EncounterData encounter, List<Transform> spawnPoints, Action onCleared)
        {
            _currentEncounter = encounter;
            _spawnPoints = spawnPoints;
            _onEncounterCleared = onCleared;

            _currentWaveIndex = 0;
            _activeEnemies.Clear();
            _enemiesToSpawn.Clear();
            _allSpawnedEnemies.Clear(); // Очищаем список перед новым боем

            if (_currentEncounter == null || _currentEncounter.waves.Count == 0 || _spawnPoints.Count == 0)
            {
                Debug.LogWarning("<color=yellow>[Encounter]</color> Нет данных для боя или точек спавна. Авто-зачистка.");
                FinishEncounter();
                return;
            }

            Debug.Log($"<color=red>[Encounter]</color> БОЙ НАЧАЛСЯ! Волн: {_currentEncounter.waves.Count}");
            StartWave(_currentWaveIndex);
        }

        private void StartWave(int waveIndex)
        {
            Debug.Log($"<color=orange>[Encounter]</color> Запуск волны {waveIndex + 1}");
            
            Wave currentWave = _currentEncounter.waves[waveIndex];
            _enemiesToSpawn.Clear();

            // Заполняем очередь префабами из настроек волны
            foreach (var waveEnemy in currentWave.enemies)
            {
                for (int i = 0; i < waveEnemy.count; i++)
                {
                    _enemiesToSpawn.Enqueue(waveEnemy.enemyPrefab);
                }
            }

            TrySpawnEnemies();
        }

        private void TrySpawnEnemies()
        {
            while (_activeEnemies.Count < _currentEncounter.maxConcurrentEnemies && _enemiesToSpawn.Count > 0)
            {
                GameObject enemyPrefabObj = _enemiesToSpawn.Dequeue();
                Enemy enemySample = enemyPrefabObj.GetComponent<Enemy>();
                
                Transform randomSpawnPoint = _spawnPoints[UnityEngine.Random.Range(0, _spawnPoints.Count)];
                
                // ИСПОЛЬЗУЕМ ПУЛ ВМЕСТО INSTANTIATE
                Enemy enemyInstance = _enemyPool.Get(enemySample, randomSpawnPoint.position, Quaternion.identity);
                enemyInstance.gameObject.SetActive(true); 
                
                _activeEnemies.Add(enemyInstance);
                _allSpawnedEnemies.Add(enemyInstance); // Запоминаем для очистки трупов позже
                
                // Подписываемся на смерть
                // Важно: если Enemy используется повторно, убедитесь, что в самом Enemy событие OnDeath очищается при деактивации
                enemyInstance.OnDeath += () => OnEnemyDied(enemyInstance);
            }
        }

        private void OnEnemyDied(Enemy enemy)
        {
            if (!_activeEnemies.Contains(enemy)) return;

            _activeEnemies.Remove(enemy);
            
            // Проверяем состояние волны
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
                // Враг умер, место освободилось — доспавниваем следующих
                TrySpawnEnemies();
            }
        }

        private void FinishEncounter()
        {
            Debug.Log("<color=cyan>[Encounter]</color> БОЙ ОКОНЧЕН!");
            
            SpawnReward();
            _onEncounterCleared?.Invoke();
        }

        private void SpawnReward()
        {
            // Ищем точку награды через RoomBuilder
            var builder = FindObjectOfType<RoomBuilder>();
            if (builder != null && builder.RewardPoint != null && chestPrefab != null)
            {
                Instantiate(chestPrefab, builder.RewardPoint.position, Quaternion.identity);
                Debug.Log("<color=yellow>[Encounter]</color> Сундук появился!");
            }
        }

        /// <summary>
        /// Вызывается при переходе в новую комнату, чтобы убрать старые трупы в пул
        /// </summary>
        public void ClearCorpses()
        {
            foreach (var enemy in _allSpawnedEnemies)
            {
                if (enemy != null && enemy.gameObject.activeSelf)
                {
                    // В SimplePool возврат в пул обычно происходит через деактивацию объекта
                    enemy.gameObject.SetActive(false); 
                }
            }
            _allSpawnedEnemies.Clear();
            Debug.Log("<color=gray>[Encounter]</color> Трупы убраны в пул.");
        }
    }
}