using System.Collections;
using System.Diagnostics;
using Edgar.Unity;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;

public class DungeonManager : MonoBehaviour {
  public System.Random Random { get; private set; }

  [SerializeField]
  private InputActionAsset inputActions;

  public static DungeonManager Instance;

  private void OnEnable() {
    Debug.Log("Enabling input actions...");
    inputActions.Enable();
  }

  private void OnDisable() {
    Debug.Log("Disabling input actions...");
    inputActions.Disable();
  }

  public void Awake() {
    Random = new();

    if (Instance == null) {
      Instance = this;
    }

    // Find the generator runner
    var generator = GameObject.Find("Dungeon Generator").GetComponent<DungeonGeneratorGrid2D>();

    // Start the generator coroutine
    StartCoroutine(GeneratorCoroutine(generator));
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
