using System.Collections.Generic;
using UnityEngine;

public class MoveHistoryManager : MonoBehaviour
{
    [SerializeField] private UndoButtonController undoButtonController;

    private Stack<Move> pastMoves = new();

    private void Awake()
    {
        EventBus.MovePerformed += RecordMove;
    }

    public void RecordMove(Move move)
    {
        pastMoves.Push(move);
        undoButtonController.OnMoveRecorded(move);
    }

    public void UndoLastMove()
    {
        if (pastMoves.Count == 0) return;

        Move lastMove = pastMoves.Pop();
        lastMove.Undo();

        if (pastMoves.Count == 0)
        {
            undoButtonController.OnMoveHistoryEmptied();
        }
    }

    private void OnDestroy()
    {
        EventBus.MovePerformed -= RecordMove;
    }
}
