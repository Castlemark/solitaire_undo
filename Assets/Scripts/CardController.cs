using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class CardController : MonoBehaviour, IClickable
{
    private const string DRAGGING_SORTING_LAYER = "Dragging";
    private const string DEFAULT_SORTING_LAYER = "Default";

    private bool isDragging;
    private Camera mainCamera;
    [SerializeField] private CardMover mover;
    [SerializeField] private SortingGroup sortingGroup;
    [SerializeField] private Collider2D cardCollider;
    [SerializeField] private Vector3 dragOffset;
    [SerializeField] private Vector3 stackedCardOffset;

    private Vector3 stackedCardTargetPosition => transform.position - stackedCardOffset;

    private CardController stackedCardBelow;
    private CardController stackedCardAbove;

    private void Awake()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("CardController: No main camera found. Dragging will not work until a camera is assigned.");
        }
    }

    public void OnClick(CardController cardController = null)
    {
        if (mainCamera == null)
        {
            return;
        }

        if (cardController != null)
        {
            if (stackedCardAbove == null) // Handle stacking
            {
                stackedCardAbove = cardController;
                stackedCardAbove.TravelToBaseStack(this, stackedCardTargetPosition);
                return;
            }

            return; // Ignore clicks from other cards if already stacked
        }

        if (isDragging)
        {
            StopDragging();
        }
        else
        {
            if (stackedCardBelow != null) // Handle unstacking
            {
                stackedCardBelow.ReleaseStackedCard();
            }

            StartDragging();
        }
    }

    public void ReleaseStackedCard()
    {
        if (stackedCardAbove != null)
        {
            stackedCardAbove = null;
        }
    }

    public void TravelToBaseStack(CardController baseCard, Vector2 position)
    {
        StopDragging();
        stackedCardBelow = baseCard;
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

        if (stackedCardAbove != null)
        {
            stackedCardAbove.mover.UpdateTravelTarget(stackedCardTargetPosition);
        }
    }

    private void StartDragging()
    {
        isDragging = true;
        sortingGroup.sortingLayerName = DRAGGING_SORTING_LAYER;
        cardCollider.enabled = false;
        EventBus.RaiseDragStarted(this);
    }

    private void StopDragging()
    {
        isDragging = false;
        sortingGroup.sortingLayerName = DEFAULT_SORTING_LAYER;
        cardCollider.enabled = true;
        EventBus.RaiseDragStopped(this);
    }
}
