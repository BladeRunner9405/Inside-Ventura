using UnityEngine;
using UnityEngine.UI; // Добавляем для работы с UI кнопкой

public class ButtonExit : MonoBehaviour
{
    [SerializeField] private GameObject panelToToggle; // Панель, которую будем переключать
    private Button button; // Ссылка на компонент Button
    
    void Start()
    {
        // Получаем компонент Button на этом объекте
        button = GetComponent<Button>();
        
        // Добавляем обработчик события нажатия кнопки
        if (button != null)
        {
            button.onClick.AddListener(TogglePanel);
        }
        
        // Проверяем, назначена ли панель в инспекторе
        if (panelToToggle == null)
        {
            Debug.LogWarning("Панель не назначена в инспекторе!", this);
        }
    }
    
    void TogglePanel()
    {
        // Проверяем, существует ли панель
        if (panelToToggle != null)
        {
            // Переключаем состояние панели на противоположное
            bool isActive = panelToToggle.activeSelf;
            panelToToggle.SetActive(!isActive);
            
          
            Debug.Log($"Панель {(isActive ? "деактивирована" : "активирована")}");
        }
    }
    
    
    void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(TogglePanel);
        }
    }
}