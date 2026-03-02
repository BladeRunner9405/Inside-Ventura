using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private float radius;
    [SerializeField] private Player owner;

    private List<InteractableObject> _interactables = new List<InteractableObject>();
    private InteractableObject _closestInterObj;

    public void Awake() {
        transform.localScale = new Vector3(radius, radius, 0);
    }

    private void FixedUpdate() {
        InteractableObject nearest = GetNearest();
        if (nearest) {
            if (_closestInterObj != nearest) {
                if (_closestInterObj) {
                    _closestInterObj.SetFocused(false);
                }
                
                _closestInterObj = nearest;
                _closestInterObj.SetFocused(true);
            }
        } else {
            if (_closestInterObj) {
                _closestInterObj.SetFocused(false);
                _closestInterObj = null;
            }
        }
    }

    public InteractableObject GetNearest()
    {
        if (_interactables.Count == 0) return null;

        return _interactables
            .OrderBy(obj => Vector3.Distance(transform.position, obj.transform.position))
            .FirstOrDefault();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision!");
        if (collision.CompareTag("InteractableObject"))
        {
            InteractableObject interactableObject = collision.GetComponent<InteractableObject>();
            interactableObject.PlayerIsNearby(true);
            _interactables.Add(interactableObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("InteractableObject"))
        {
            InteractableObject interactableObject = collision.GetComponent<InteractableObject>();
            interactableObject.PlayerIsNearby(false);
            _interactables.Remove(interactableObject);
        }
    }

    public bool TryToInteract()
    {
        if (_closestInterObj) {
            _closestInterObj.Interact(owner);

            if (!_closestInterObj.IsActive()) {
                _interactables.Remove(_closestInterObj);
            }
            return true;
        } 
        return false;
    }
}
