using System;
using UnityEngine;

public class Wave : MonoBehaviour {
  private void Awake() {
    // The wave is started by room manager.
    gameObject.SetActive(false);
  }

  public void StartWave() {

  }
}
