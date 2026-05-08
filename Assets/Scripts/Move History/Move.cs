using UnityEngine;

public class Move
{
    CardController card;
    CardController currentBelowCard;
    CardController originalBelowCard;
    Vector2 initialPosition;



    public Move(CardController card, CardController currentBelowCard = null, CardController originalBelowCard = null, Vector2 initialPosition = default)
    {
        this.card = card;
        this.currentBelowCard = currentBelowCard;
        this.initialPosition = initialPosition;
        this.originalBelowCard = originalBelowCard;
    }

    public void Undo()
    {
        if (originalBelowCard != null)
        {
            card.StackAndTravelTo(originalBelowCard, initialPosition, false);
        }
        else
        {
            if (currentBelowCard != null)
            {
                currentBelowCard.Unstack();
            }

            card.TravelTo(initialPosition, false);
        }
    }

    public override string ToString()
    {
        return $"Move(card={card.name}, currentTargetCard={currentBelowCard?.name}, originalTargetCard={originalBelowCard?.name}, initialPosition={initialPosition})";
    }
}
