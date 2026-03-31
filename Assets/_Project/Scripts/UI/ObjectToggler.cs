using UnityEngine;
using UnityEngine.UI;

// Добавляем для работы с UI кнопкой

public class ObjectToggler : MonoBehaviour {
  [SerializeField] private GameObject objectToToggle; // Панель, которую будем переключать
  private Button button; // Ссылка на компонент Button

  private void Start() {
    // Получаем компонент Button на этом объекте
    button = GetComponent<Button>();

    // Добавляем обработчик события нажатия кнопки
    if (button != null) button.onClick.AddListener(TogglePanel);

    // Проверяем, назначена ли панель в инспекторе
    if (objectToToggle == null) Debug.LogWarning("Панель не назначена в инспекторе!", this);
  }


  private void OnDestroy() {
    if (button != null) button.onClick.RemoveListener(TogglePanel);
  }

  private void TogglePanel() {
    // Проверяем, существует ли панель
    if (objectToToggle != null) {
      // Переключаем состояние панели на противоположное
      var isActive = objectToToggle.activeSelf;
      objectToToggle.SetActive(!isActive);

      // Debug.Log($"Панель {(isActive ? "деактивирована" : "активирована")}");
    }
  }
}
