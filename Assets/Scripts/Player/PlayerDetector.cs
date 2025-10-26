using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    [Header("Detection Settings")]
    public float detectionRange = 5f;
    public LayerMask interactableMask;
    public Camera cam;

    [Header("Debug")]
    public bool showRay = true;

    private IInteractable currentTarget;

    void Update()
    {
        DetectObject();
    }

    private void DetectObject()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, detectionRange, interactableMask))
        {
            if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                if (currentTarget != interactable)
                {
                    currentTarget?.OnLoseFocus();
                    currentTarget = interactable;
                    currentTarget.OnFocus();
                }
                return;
            }
        }

        if (currentTarget != null)
        {
            currentTarget.OnLoseFocus();
            currentTarget = null;
        }
    }

    public bool HasTarget()
    {
        return currentTarget != null;
    }

    public void TryInteract(GameObject player)
    {
        currentTarget?.Interact(player);
    }

    private void OnDrawGizmos()
    {
        if (!showRay || cam == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(cam.transform.position, cam.transform.forward * detectionRange);
    }
}
