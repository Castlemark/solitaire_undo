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
    }

    public void UndoLastMove()
    {
        if (pastMoves.Count == 0) return;

        Move lastMove = pastMoves.Pop();
        lastMove.Undo();
    }
}

public class Move
{
    CardController card;
    CardController currentTargetCard;
    CardController originalTargetCard;
    Vector2 targetPosition;

    public Move(CardController card, CardController currentTargetCard, CardController originalTargetCard = null, Vector2 targetPosition = default)
    {
        this.card = card;
        this.currentTargetCard = currentTargetCard;
        this.targetPosition = targetPosition;
        this.originalTargetCard = originalTargetCard;
    }

    public void Undo()
    {
        if (originalTargetCard != null)
        {
            card.StackAndTravelTo(originalTargetCard, targetPosition);
        }
        else
        {
            if (currentTargetCard != null)
            {
                currentTargetCard.Unstack();
            }

            card.TravelTo(targetPosition);
        }
    }
}
