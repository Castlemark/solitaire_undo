using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CardHolder : MonoBehaviour, IClickable
{
    [SerializeField] private Transform holdedCardTargetPosition;
    [SerializeField] private Collider2D cardHolderCollider;

    private CardController stackedCardAbove;

    public void OnClick(CardController currentlyDraggedCard = null)
    {
        if (currentlyDraggedCard == null || stackedCardAbove != null) return; // Only allow clicks from cards if we don't already have a card stacked above

        currentlyDraggedCard.TravelTo(holdedCardTargetPosition.position);

    }
}
