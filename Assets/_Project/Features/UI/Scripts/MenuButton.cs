using UnityEngine;
using UnityEngine.InputSystem;

public class MenuButton : MonoBehaviour
{
  private ObjectToggler _objectToggler;
  private InputToggler _inputToggler;

  private InputMaster controls;

  private void Awake()
  {
    controls = new InputMaster();

    _objectToggler = GetComponent<ObjectToggler>();
    _inputToggler = GetComponent<InputToggler>();
  }

  private void OnEnable()
  {
    controls.Player.Menu.performed += OnInteractPerformed;
    controls.Enable();
  }

  private void OnDisable()
  {
    controls.Player.Menu.performed -= OnInteractPerformed;
    controls.Disable();
  }

  public void OnInteract()
  {
    _objectToggler.ToggleObjects();
    _inputToggler.TogglePanel();
  }

  private void OnInteractPerformed(InputAction.CallbackContext ctx)
  {
    OnInteract();
  }
}
