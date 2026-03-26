using UnityEngine;
using UnityEngine.Rendering;

public class SpritesSorting : MonoBehaviour {
  private SortingGroup sortingGroup;
  private SpriteRenderer spriteRenderer;

  private void Start() {
    sortingGroup = GetComponent<SortingGroup>();
    spriteRenderer = GetComponent<SpriteRenderer>();
  }

  private void Update() {
    var order = Mathf.RoundToInt(-transform.position.y * 100f);
    if (sortingGroup)
      sortingGroup.sortingOrder = order;
    else
      spriteRenderer.sortingOrder = order;
  }
}
