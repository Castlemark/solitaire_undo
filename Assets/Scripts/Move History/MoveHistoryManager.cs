using System.Collections.Generic;
using UnityEngine;

public class MoveHistoryManager : MonoBehaviour
{
    [SerializeField] private MoveHistoryButtonController undoButtonController;
    [SerializeField] private MoveHistoryButtonController redoButtonController;

    private Stack<Move> undoMoves = new();
    private Stack<Move> redoMoves = new();

    private void Awake()
    {
        EventBus.MovePerformed += RecordMove;
    }

    public void RecordMove(Move move)
    {
        undoMoves.Push(move);
        redoMoves.Clear(); // Clear redo stack on new move

        undoButtonController.OnMoveRecorded(move);
        redoButtonController.OnMoveHistoryEmptied();
    }

    public void UndoLastMove()
    {
        if (undoMoves.Count == 0) return;

        Move lastMove = undoMoves.Pop();
        lastMove.Undo();
        redoMoves.Push(lastMove);

        redoButtonController.OnMoveRecorded(lastMove);
        if (undoMoves.Count == 0)
        {
            undoButtonController.OnMoveHistoryEmptied();
        }
    }

    public void RedoLastMove()
    {
        if (redoMoves.Count == 0) return;

        Move lastMove = redoMoves.Pop();
        lastMove.Redo();
        undoMoves.Push(lastMove);

        undoButtonController.OnMoveRecorded(lastMove);
        if (redoMoves.Count == 0)
        {
            redoButtonController.OnMoveHistoryEmptied();
        }
    }

    private void OnDestroy()
    {
        EventBus.MovePerformed -= RecordMove;
    }
}
