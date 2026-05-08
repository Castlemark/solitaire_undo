using UnityEngine;
using UnityEngine.InputSystem;

public class CardController : MonoBehaviour, IClickable
{
    private bool isDragging;
    private Camera mainCamera;
    [SerializeField] private CardMover mover;

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

    public void TravelTo(Vector3 position)
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

        mover.UpdateMovement(mainCamera, isDragging, Mouse.current.position.ReadValue());
    }

    private void StartDragging()
    {
        isDragging = true;
        mover.StartDragging(Mouse.current.position.ReadValue(), mainCamera);
        EventBus.RaiseDragStarted(this);
    }

    private void StopDragging()
    {
        isDragging = false;
        mover.SetReturnToNeutralRotation();
        EventBus.RaiseDragStopped(this);
    }
}
