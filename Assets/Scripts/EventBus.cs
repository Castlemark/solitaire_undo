using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class EventBus
{
    public static event Action<CardController> DragStarted;
    public static event Action<CardController> DragStopped;

    public static MoveHistory MoveHistory = new();

    public static void RaiseDragStarted(CardController draggable)
    {
        DragStarted?.Invoke(draggable);
    }

    public static void RaiseDragStopped(CardController draggable)
    {
        DragStopped?.Invoke(draggable);
    }
}

public class MoveHistory
{
    public Stack<Move> pastMoves = new();

    public void RecordMove(Move move)
    {
        pastMoves.Push(move);

        Debug.Log("All moves:");
        foreach (var item in pastMoves)
        {
            Debug.Log(item);
        }
    }

    public void UndoLastMove()
    {
        if (pastMoves.Count == 0) return;

        Move lastMove = pastMoves.Pop();
        lastMove.Undo();

        Debug.Log($"Undone move: {lastMove}");
        Debug.Log("Remaining moves:");
        foreach (var item in pastMoves)
        {
            Debug.Log(item);
        }
    }
}

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
