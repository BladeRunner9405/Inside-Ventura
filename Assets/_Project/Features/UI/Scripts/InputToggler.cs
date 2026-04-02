using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputToggler : MonoBehaviour
{
  [SerializeField]
  private InputActionAsset inputActionAsset;

  [SerializeField]
  private string actionMapName = "Player";

  private Button _button;
  private InputActionMap _playerActionMap;

  private bool _isActive = true;

  private void Start()
  {
    _button = GetComponent<Button>();
    if (_button != null)
      _button.onClick.AddListener(TogglePanel);

    if (inputActionAsset != null)
      _playerActionMap = inputActionAsset.FindActionMap(actionMapName);
  }

  private void OnDestroy()
  {
    if (_button != null)
      _button.onClick.RemoveListener(TogglePanel);
  }

  private void TogglePanel()
  {
    if (_playerActionMap != null)
    {
      if (_isActive)
        _playerActionMap.Disable();
      else
        _playerActionMap.Enable();
      _isActive = !_isActive;
    }
  }
}
