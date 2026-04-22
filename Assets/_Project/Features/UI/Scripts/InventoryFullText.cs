using CherryFramework.DependencyManager;
using TMPro;
using DG.Tweening;
using UnityEngine;

public class InventoryFullText : InjectMonoBehaviour
{
  [Inject]
  private PlayerAccessor _playerAccessor;

  private TextMeshProUGUI _text;
  private Tween _tween;

  private void Start() {
    _playerAccessor.OnPlayerRegistered += Subscribe;

    _text = GetComponent<TextMeshProUGUI>();
    _text.enabled = false;
  }

  private void Subscribe(Player player) {
    _playerAccessor.Inventory.OnCantAddThought += ShowMessage;
  }

  private void ShowMessage() {
    Debug.Log("message");
    _tween?.Kill();

    _text.enabled = true;

    _tween = DOVirtual.DelayedCall(1f, () => {
      _text.enabled = false;
    });
  }
}
