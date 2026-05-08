using UnityEngine;

public class Move
{
    CardController card;
    CardController currentBelowCard;
    CardController originalBelowCard;
    Vector2 currentPosition;
    Vector2 originalPosition;



    public Move(CardController card, CardController currentBelowCard = null, CardController originalBelowCard = null, Vector2 originalPosition = default)
    {
        this.card = card;
        this.currentBelowCard = currentBelowCard;
        this.originalPosition = originalPosition;
        this.originalBelowCard = originalBelowCard;
    }

    public void Undo()
    {
        if (currentBelowCard != null)
        {
            currentBelowCard.Unstack();
        }

        if (originalBelowCard != null)
        {
            card.StackAndTravelTo(originalBelowCard, originalPosition, false);
        }
        else
        {

            card.TravelTo(originalPosition, false);
        }
    }

    public override string ToString()
    {
        return $"Move(card={card.name}, currentTargetCard={currentBelowCard?.name}, originalTargetCard={originalBelowCard?.name}, originalPosition={originalPosition})";
    }
}
