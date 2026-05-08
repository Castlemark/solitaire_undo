using UnityEngine;
using UnityEngine.InputSystem;

public class CardController : MonoBehaviour, IClickable
{
    private bool isDragging;
    private Camera mainCamera;
    [SerializeField] private CardMover mover;
    [SerializeField] private Vector3 dragOffset;

    private void Awake()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("CardController: No main camera found. Dragging will not work until a camera is assigned.");
        }
    }

    public void OnClick()
    {
        if (mainCamera == null)
        {
            return;
        }

        if (isDragging)
        {
            StopDragging();
        }
        else
        {
            StartDragging();
        }
    }

    public void TravelTo(Vector2 position)
    {
        isDragging = false;
        mover.TravelTo(position);
    }

    private void Update()
    {
        if (mainCamera == null || mover == null)
        {
            return;
        }

        if (isDragging)
        {
            var mouseScreenPosition = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mover.UpdateTravelTarget(mouseScreenPosition - dragOffset);
        }

        mover.UpdateMovement();
    }

    private void StartDragging()
    {
        isDragging = true;
        EventBus.RaiseDragStarted(this);
    }

    private void StopDragging()
    {
        isDragging = false;
        EventBus.RaiseDragStopped(this);
    }
}
