using System;
using UnityEngine;
using UnityEngine.UI;

public class UndoButtonController : MonoBehaviour
{
    [SerializeField] private Button undoButton;

    private void Awake()
    {
        undoButton.interactable = false;
    }

    public void OnMoveHistoryEmptied()
    {
        undoButton.interactable = false;
    }

    public void OnMoveRecorded(Move move)
    {
        undoButton.interactable = true;
    }
}
