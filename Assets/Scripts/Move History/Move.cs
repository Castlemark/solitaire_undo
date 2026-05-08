using UnityEngine;

public class Move
{
    CardController card;
    CardController currentBelowCard;
    CardController originalBelowCard;
    Vector2 currentPosition;
    Vector2 originalPosition;



    public Move(CardController card, CardController currentBelowCard, CardController originalBelowCard, Vector2 currentPosition, Vector2 originalPosition)
    {
        this.card = card;
        this.currentBelowCard = currentBelowCard;
        this.originalBelowCard = originalBelowCard;
        this.currentPosition = currentPosition;
        this.originalPosition = originalPosition;
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

    public void Redo()
    {
        if (originalBelowCard != null)
        {
            originalBelowCard.Unstack();
        }

        if (currentBelowCard != null)
        {
            card.StackAndTravelTo(currentBelowCard, currentPosition, false);
        }
        else
        {
            card.TravelTo(currentPosition, false);
        }
    }

    public override string ToString()
    {
        return $"Move(card: {card.name}, currentTargetCard: {currentBelowCard?.name}, originalTargetCard: {originalBelowCard?.name}, currentPosition: {currentPosition}, originalPosition: {originalPosition})";
    }
}
