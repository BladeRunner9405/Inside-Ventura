using UnityEngine;
using UnityEngine.Rendering;

public class SpritesSorting : MonoBehaviour
{
  [SerializeField]
  private Transform pivot;

  private SortingGroup sortingGroup;
  private SpriteRenderer spriteRenderer;

  private void Start()
  {
    if (pivot == null) {
      pivot = gameObject.transform;
    }

    sortingGroup = GetComponent<SortingGroup>();
    spriteRenderer = GetComponent<SpriteRenderer>();
  }

  private void Update()
  {
    var order = Mathf.RoundToInt(-pivot.position.y * 100f);
    if (sortingGroup)
      sortingGroup.sortingOrder = order;
    else
      spriteRenderer.sortingOrder = order;
  }
}
