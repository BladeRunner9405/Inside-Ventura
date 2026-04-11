using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewHeart", menuName = "InsideVentura/Artifacts/Heart")]
public class Heart : Artifact
{
  // Здесь могут быть только БАЗОВЫЕ настройки Сердца (если они есть)
  // Например, базовое увеличение макс. здоровья, которое дает само сердце без мыслей

  /*[SerializeField]
  public float baseMaxHealthBonus = 0f;*/

  [SerializeReference] private Effect[] traits;

  public IReadOnlyList<Effect> Traits => traits;
}
