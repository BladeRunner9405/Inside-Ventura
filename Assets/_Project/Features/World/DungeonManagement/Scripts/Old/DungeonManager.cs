using System.Collections;
using System.Diagnostics;
using Edgar.Unity;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using CherryFramework.BaseClasses;
using CherryFramework.DependencyManager;
using Unity.Cinemachine;

public class DungeonManager : BehaviourBase {
  public System.Random Random { get; private set; }

  [SerializeField] private CinemachineCamera playerCamera;
  [SerializeField] private DungeonGeneratorGrid2D generator;

  public void Awake() {
    Random = new();

    // Start the generator coroutine
    StartCoroutine(GeneratorCoroutine(generator));
  }

  public void RestartLevel() {
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex,LoadSceneMode.Single);
  }

  /// <summary>
  /// Coroutine that generates the level.
  /// We need to yield return before the generator starts because we want to show the loading screen
  /// and it cannot happen in the same frame.
  /// It is also sometimes useful to yield return before we hide the loading screen to make sure that
  /// all the scripts that were possibly created during the process are properly initialized.
  /// </summary>
  private IEnumerator GeneratorCoroutine(DungeonGeneratorGrid2D generator) {
    var stopwatch = new Stopwatch();

    stopwatch.Start();

    yield return null;

    generator.Generate();

    yield return null;

    stopwatch.Stop();
  }
}

public class DungeonAccessor : IDungeon {
  private DungeonManager _instance;

  public void RegisterDungeon(DungeonManager dungeon) {
    _instance = dungeon;
  }

  public void Restart() {
    _instance.RestartLevel();
  }
}

public interface IDungeon {
  void Restart();
}
