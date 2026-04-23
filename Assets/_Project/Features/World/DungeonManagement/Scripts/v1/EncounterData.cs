using System;
using System.Collections.Generic;
using UnityEngine;

namespace InsideVentura.World.v1
{
  // Настройки конкретного типа врага в волне
  [Serializable]
  public class WaveEnemy
  {
    public GameObject enemyPrefab; // Префаб (Гнев или Зависть)
    public int count; // Сколько штук нужно заспавнить
  }

  // Настройки одной волны
  [Serializable]
  public class Wave
  {
    public List<WaveEnemy> enemies;
  }

  [CreateAssetMenu(fileName = "NewEncounter", menuName = "InsideVentura/Dungeon/EncounterData")]
  public class EncounterData : ScriptableObject
  {
    [Header("Wave Settings")]
    public List<Wave> waves; // Список всех волн в этой комнате

    [Header("Rules")]
    [Tooltip("Максимальное количество живых врагов на арене одновременно")]
    public int maxConcurrentEnemies = 5;
  }
}
