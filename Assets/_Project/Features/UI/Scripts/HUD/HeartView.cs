using UnityEngine;
using UnityEngine.UI;

public enum HeartState {
  Empty,
  Half,
  Full
}

public class HeartView : MonoBehaviour {
  [SerializeField] private Sprite fullSprite;
  [SerializeField] private Sprite halfSprite;
  [SerializeField] private Sprite emptySprite;
  private Image _image;

  private void Awake() {
    _image = GetComponent<Image>();
  }

  public void SetState(HeartState state) {
    if (_image == null) return;
    switch (state) {
      case HeartState.Full:
        _image.sprite = fullSprite;
        break;
      case HeartState.Half:
        _image.sprite = halfSprite;
        break;
      case HeartState.Empty:
        _image.sprite = emptySprite;
        break;
    }
  }
}
