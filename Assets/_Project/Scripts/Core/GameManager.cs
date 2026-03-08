using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour {
  public InputActionAsset inputActions;

  private void OnEnable() {
    Debug.Log("Enabling input actions...");
    inputActions.Enable();
  }

  private void OnDisable() {
    Debug.Log("Disabling input actions...");
    inputActions.Disable();
  }
}
