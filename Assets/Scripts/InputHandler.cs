using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private Camera mainCamera;

    private CardController draggingCard;

    private void Awake()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("InputHandler: No main camera found. Input handling will not work until a camera is assigned.");
        }

        EventBus.DragStarted += OnDragStarted;
        EventBus.DragStopped += OnDragStopped;
    }

    private void OnDragStarted(CardController controller) { draggingCard = controller; }

    private void OnDragStopped(CardController controller) { draggingCard = null; }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (!context.started) return;


        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction, 100f);

        if (hits.Length == 0)
        {
            if (draggingCard != null)
            {
                draggingCard.OnClick();
            }

            return;
        }

        // Find the topmost collider based on Y position (Y-sort)
        RaycastHit2D topHit = hits[0];
        foreach (var hit in hits)
        {
            if (hit.collider == null) continue;

            if (hit.transform.position.y < topHit.transform.position.y)
            {
                topHit = hit;
            }
        }

        var clickable = topHit.collider.gameObject.GetComponent<IClickable>();
        if (clickable != null)
        {
            clickable.OnClick(draggingCard);
        }
    }

    private void OnDestroy()
    {
        EventBus.DragStarted -= OnDragStarted;
        EventBus.DragStopped -= OnDragStopped;
    }
}
