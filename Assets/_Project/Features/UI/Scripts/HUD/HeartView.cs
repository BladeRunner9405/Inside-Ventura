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
  [SerializeField] private Image image;

  public void SetState(HeartState state) {
    if (image == null)
      return;
    switch (state) {
      case HeartState.Full:
        image.sprite = fullSprite;
        break;
      case HeartState.Half:
        image.sprite = halfSprite;
        break;
      case HeartState.Empty:
        image.sprite = emptySprite;
        break;
    }
  }
}
