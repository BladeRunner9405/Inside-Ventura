using System.Collections;
using System.Diagnostics;
using Edgar.Unity;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class DungeonManager : MonoBehaviour {
  public System.Random Random { get; private set; }
  public static DungeonManager Instance;

  [SerializeField]
  private InputActionAsset inputActions;
  private InputAction restartAction;

  private DungeonGeneratorGrid2D generator;

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

    restartAction = InputSystem.actions.FindAction("RestartLevel");

    // Find the generator runner
    generator = GameObject.Find("Dungeon Generator").GetComponent<DungeonGeneratorGrid2D>();

    // Start the generator coroutine
    StartCoroutine(GeneratorCoroutine(generator));
  }

  private void Update() {
    if (restartAction.triggered) {
      SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex,LoadSceneMode.Single);
    }
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
