using System;
using UnityEngine;
using UnityEngine.UI;

public class NewMonoBehaviourScript : MonoBehaviour {
  [SerializeField] private Enemy enemy;
  [SerializeField] private Slider slider;

  private void Awake() {
    enemy.OnTakeDamage += _ => { slider.value = enemy.Health / enemy.MaxHealth; };
  }
}
