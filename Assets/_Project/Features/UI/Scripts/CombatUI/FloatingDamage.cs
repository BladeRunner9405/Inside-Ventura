using System.Globalization;
using DG.Tweening;
using TMPro;
using UnityEngine;


public class FloatingDamage : MonoBehaviour {
  [SerializeField] private TMP_Text text;
  [SerializeField] private float duration = 0.3F;
  [SerializeField] private float upMoveAmount = 1;

  public void Initialize(float damage) {
    text.text = damage.ToString(CultureInfo.InvariantCulture);
    text.alpha = 1;

    transform.DOMoveY(upMoveAmount, duration);
    DOTween.To(() => transform.position, p => transform.position = p, transform.position + Vector3.up * upMoveAmount,
      duration);
    DOTween.To(() => text.alpha, x => text.alpha = x, 0, duration);

    Invoke(nameof(Disappear), duration);
  }

  private void Disappear() {
    gameObject.SetActive(false);
  }
}
