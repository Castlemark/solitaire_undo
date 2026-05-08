using DG.Tweening;
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
    [SerializeField] private CardVisuals visuals;
    [SerializeField] private SortingGroup sortingGroup;
    [SerializeField] private Collider2D cardCollider;

    [Header("Movement Settings")]
    [SerializeField] private float dragScaleFactor;
    [SerializeField] private float dragScaleDuration;
    [SerializeField] private Vector3 dragOffset;
    [SerializeField] private Vector3 stackedCardOffset;

    private Vector3 stackedCardTargetPosition => transform.position - stackedCardOffset;

    private CardController stackedCardBelow;
    private CardController stackedCardAbove;

    private Vector3 initialPosition;
    private CardController previousBelowCard;

    private void Awake()
    {
        mainCamera = Camera.main;

        visuals.SetCardVisuals();
    }

    public void OnClick(CardController currentlyDraggedCard = null)
    {
        if (currentlyDraggedCard != null)
        {
            if (stackedCardAbove == null) // Handle stacking
            {
                currentlyDraggedCard.StackAndTravelTo(this, stackedCardTargetPosition);
            }

            return;
        }

        if (isDragging)
        {
            Drop();
            EventBus.RaiseMovePerformed(new Move(this, null, previousBelowCard, transform.position, initialPosition));

            return;
        }

        Drag();
    }

    public void TravelTo(Vector2 position, bool recordMove = true)
    {
        Drop();

        mover.TravelTo(position);

        if (recordMove)
        {
            EventBus.RaiseMovePerformed(new Move(this, null, previousBelowCard, position, initialPosition));
        }
    }

    public void StackAndTravelTo(CardController baseCard, Vector2 position, bool recordMove = true)
    {
        Drop();

        stackedCardBelow = baseCard;
        baseCard.stackedCardAbove = this;

        mover.TravelTo(position);

        if (recordMove)
        {
            EventBus.RaiseMovePerformed(new Move(this, stackedCardBelow, previousBelowCard, position, initialPosition));
        }
    }

    // To unstack, we simply clear the references of the below card. This way, it will no longer try to control the above card's position.
    public void Unstack()
    {
        if (stackedCardAbove != null)
        {
            stackedCardAbove.stackedCardBelow = null;
            stackedCardAbove = null;
        }
    }

    private void Update()
    {
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

    private void Drag()
    {
        previousBelowCard = stackedCardBelow;
        if (stackedCardBelow != null)
        {
            stackedCardBelow.Unstack();
        }

        isDragging = true;
        initialPosition = transform.position;
        PrepareForStackedDragging();
        EventBus.RaiseDragStarted(this);
    }

    private void PrepareForStackedDragging()
    {
        sortingGroup.sortingLayerName = DRAGGING_SORTING_LAYER;
        cardCollider.enabled = false;
        transform.DOScale(Vector3.one * dragScaleFactor, dragScaleDuration);

        if (stackedCardAbove != null)
        {
            stackedCardAbove.PrepareForStackedDragging();
        }
    }

    private void Drop()
    {
        isDragging = false;
        PrepareForStackedDropping();
        EventBus.RaiseDragStopped(this);
    }

    private void PrepareForStackedDropping()
    {
        sortingGroup.sortingLayerName = DEFAULT_SORTING_LAYER;
        cardCollider.enabled = true;
        transform.DOScale(Vector3.one, dragScaleDuration);

        if (stackedCardAbove != null)
        {
            stackedCardAbove.PrepareForStackedDropping();
        }
    }
}
