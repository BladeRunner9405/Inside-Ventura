using System.Globalization;
using TMPro;
using UnityEngine;

public class FloatingDamage : MonoBehaviour {
  [SerializeField] private TMP_Text text;

  public void Initialize(float damage) {
    text.text = damage.ToString(CultureInfo.InvariantCulture);
    Invoke(nameof(Disappear), 0.3F);
  }

  private void Disappear() {
    gameObject.SetActive(false);
  }
}
