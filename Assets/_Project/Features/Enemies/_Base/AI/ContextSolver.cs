using UnityEngine;

public class ContextSolver : MonoBehaviour
{
  public Vector2 GetDirection(AIData aiData, Vector2 targetDir)
  {
    // 1. Заполняем Интересы (насколько направление совпадает с целью)
    for (int i = 0; i < AIData.Directions.Length; i++)
    {
      float dot = Vector2.Dot(targetDir, AIData.Directions[i]);
      aiData.Interest[i] = Mathf.Max(0, dot);
    }

    // 2. Вычитаем Опасности
    for (int i = 0; i < 8; i++)
    {
      aiData.Interest[i] = Mathf.Clamp01(aiData.Interest[i] - aiData.Danger[i]);
    }

    // 3. Вычисляем средний вектор
    Vector2 finalDir = Vector2.zero;
    for (int i = 0; i < 8; i++)
    {
      finalDir += AIData.Directions[i] * aiData.Interest[i];
    }

    return finalDir.normalized;
  }
}
