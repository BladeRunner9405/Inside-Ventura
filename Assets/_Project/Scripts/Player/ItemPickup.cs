using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public float radius;
    public Player owner;

    private List<InteractableObject> interactables = new List<InteractableObject>();
    private InteractableObject closestInterObj;

    public void Awake() {
        transform.localScale = new Vector3(radius, radius, 0);
    }

    private void FixedUpdate() {
        InteractableObject nearest = GetNearest();
        if (nearest) {
            if (closestInterObj != nearest) {
                if (closestInterObj) {
                    closestInterObj.SetFocused(false);
                }
                
                closestInterObj = nearest;
                closestInterObj.SetFocused(true);
            }
        } else {
            if (closestInterObj) {
                closestInterObj.SetFocused(false);
                closestInterObj = null;
            }
        }
    }

    public InteractableObject GetNearest()
    {
        if (interactables.Count == 0) return null;

        return interactables
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
            interactables.Add(interactableObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("InteractableObject"))
        {
            InteractableObject interactableObject = collision.GetComponent<InteractableObject>();
            interactableObject.PlayerIsNearby(false);
            interactables.Remove(interactableObject);
        }
    }

    public bool TryToInteract()
    {
        if (closestInterObj) {
            closestInterObj.Interact(owner);

            if (!closestInterObj.IsActive()) {
                interactables.Remove(closestInterObj);
            }
            return true;
        } 
        return false;
    }
}
