using System;

public static class EventBus
{
    public static event Action<CardController> DragStarted;
    public static event Action<CardController> DragStopped;

    public static event Action<Move> MovePerformed;

    public static void RaiseDragStarted(CardController draggable)
    {
        DragStarted?.Invoke(draggable);
    }

    public static void RaiseDragStopped(CardController draggable)
    {
        DragStopped?.Invoke(draggable);
    }

    public static void RaiseMovePerformed(Move move)
    {
        MovePerformed?.Invoke(move);
    }
}
