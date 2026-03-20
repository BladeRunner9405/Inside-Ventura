using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemPickup : MonoBehaviour {
  [SerializeField] private float radius;

  private readonly List<InteractableObject> _interactables = new();
  private InteractableObject _closestInterObj;

  public void Awake() {
    transform.localScale = new Vector3(radius, radius, 0);
  }

  private void FixedUpdate() {
    var nearest = GetNearest();
    if (nearest) {
      if (_closestInterObj != nearest) {
        if (_closestInterObj) _closestInterObj.SetFocused(false);

        _closestInterObj = nearest;
        _closestInterObj.SetFocused(true);
      }
    }
    else {
      if (_closestInterObj) {
        _closestInterObj.SetFocused(false);
        _closestInterObj = null;
      }
    }
  }

  private void OnTriggerEnter2D(Collider2D collision) {
    if (collision.CompareTag("InteractableObject")) {
      var interactableObject = collision.GetComponent<InteractableObject>();
      interactableObject.PlayerIsNearby(true);
      _interactables.Add(interactableObject);
    }
  }

  private void OnTriggerExit2D(Collider2D collision) {
    if (collision.CompareTag("InteractableObject")) {
      var interactableObject = collision.GetComponent<InteractableObject>();
      interactableObject.PlayerIsNearby(false);
      _interactables.Remove(interactableObject);
    }
  }

  public InteractableObject GetNearest() {
    if (_interactables.Count == 0) return null;

    return _interactables
      .OrderBy(obj => Vector3.Distance(transform.position, obj.transform.position))
      .FirstOrDefault();
  }

  public bool TryToInteract() {
    if (_closestInterObj) {
      _closestInterObj.OnInteract();

      if (!_closestInterObj.IsActive()) _interactables.Remove(_closestInterObj);
      return true;
    }

    return false;
  }
}
