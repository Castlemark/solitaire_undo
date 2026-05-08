using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CardHolder : MonoBehaviour, IClickable
{
    [SerializeField] private Transform holdedCardTargetPosition;
    [SerializeField] private Collider2D cardHolderCollider;

    private CardController stackedCardAbove;

    public void OnClick(CardController cardController = null)
    {
        if (cardController == null || stackedCardAbove != null) return; // Only allow clicks from cards if we don't already have a card stacked above

        cardController.TravelTo(holdedCardTargetPosition.position);

    }
}
