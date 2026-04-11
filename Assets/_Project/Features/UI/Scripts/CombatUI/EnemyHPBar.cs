using System;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHPBar : MonoBehaviour {
  [SerializeField] private Enemy enemy;
  [SerializeField] private Slider slider;

  private void Awake() {
    if (enemy == null) {
      Debug.Log("EnemyHPBar enemy is null!");
    }

    if (slider == null) {
      Debug.Log("EnemyHPBar slider is null!");
    }

    enemy.OnTakeDamage += _ => {
      Debug.Log("something");
      slider.value = enemy.Health / enemy.MaxHealth;

      if (enemy.Health == 0) {
        gameObject.SetActive(false);
      }
    };
  }
}
