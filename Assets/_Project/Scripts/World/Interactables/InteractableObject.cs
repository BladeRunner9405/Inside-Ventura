using CherryFramework.DependencyManager;
using UnityEngine;

public abstract class InteractableObject : InjectMonoBehaviour {
  [SerializeField] private GameObject outline;
  private bool _isPlayerNearby;

  private bool isActive = true;

  [Inject] protected PlayerAccessor PlayerAccessor;

  public void PlayerIsNearby(bool nearby) {
    // мб этот метод нам не понадобится, но все равно. Срабатываеит, если рядом игрок
    _isPlayerNearby = nearby;
  }

  public virtual void OnInteract() {
    SetActive(false);
    Destroy(this);
  }

  protected void SetActive(bool active) {
    if (isActive != active) isActive = active;
    Destroy(gameObject);
  }

  public bool IsActive() {
    return isActive;
  }

  public void SetFocused(bool active) {
    // срабатывает, если игрок стоит рядом с предметом и готов его взять.
    if (outline.activeSelf != active) outline.SetActive(active);
    // ... тут логика для появления UI с инфой об предмете
  }
}
