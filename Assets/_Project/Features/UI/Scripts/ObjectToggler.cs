using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic; // Для работы со списками

public class ObjectToggler : MonoBehaviour
{
  [SerializeField] private List<GameObject> objectsToToggle; // Список объектов, активность которых будем переключать
  private Button button; // Ссылка на компонент Button

  private void Start()
  {
    // Получаем компонент Button на этом объекте
    button = GetComponent<Button>();

    // Добавляем обработчик события нажатия кнопки
    if (button != null)
      button.onClick.AddListener(ToggleObjects);

    // Проверяем, назначен ли список в инспекторе
    if (objectsToToggle == null || objectsToToggle.Count == 0)
      Debug.LogWarning("Список объектов для переключения пуст или не назначен в инспекторе!", this);
  }

  private void OnDestroy()
  {
    // Удаляем обработчик при уничтожении объекта, чтобы избежать утечек
    if (button != null)
      button.onClick.RemoveListener(ToggleObjects);
  }

  private void ToggleObjects()
  {
    // Если список не задан, ничего не делаем
    if (objectsToToggle == null) return;

    // Перебираем все объекты в списке и переключаем их активность
    foreach (GameObject obj in objectsToToggle)
    {
      if (obj != null) // Проверяем, что объект существует
      {
        bool isActive = obj.activeSelf;
        obj.SetActive(!isActive);

        // Debug.Log($"Объект {obj.name} {(isActive ? "деактивирован" : "активирован")}");
      }
    }
  }
}
