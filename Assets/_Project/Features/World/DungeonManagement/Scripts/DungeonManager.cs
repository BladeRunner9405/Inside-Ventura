using System.Collections;
using System.Diagnostics;
using Edgar.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;
using CherryFramework.BaseClasses;
using Debug = UnityEngine.Debug;

namespace InsideVentura.World {
  public class DungeonManager : BehaviourBase {
    public System.Random Random { get; private set; }

    [SerializeField] private DungeonGeneratorGrid2D generator;

    public RoomManagerBase CurrentRoom { get; set; }

    public void Awake() {
      Random = new();

      // Start the generator coroutine
      StartCoroutine(GeneratorCoroutine(generator));
    }

    public void RestartLevel() {
      SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
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

    public void RegisterRoomObject(GameObject obj) {
      Debug.Log("Registering room object " + obj.name);
      obj.transform.SetParent(CurrentRoom.transform);
    }
  }
}
