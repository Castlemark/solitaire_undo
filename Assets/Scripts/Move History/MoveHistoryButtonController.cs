using System;
using UnityEngine;
using UnityEngine.UI;

public class MoveHistoryButtonController : MonoBehaviour
{
    [SerializeField] private Button button;

    private void Awake()
    {
        button.interactable = false;
    }

    public void OnMoveHistoryEmptied()
    {
        button.interactable = false;
    }

    public void OnMoveRecorded(Move move)
    {
        button.interactable = true;
    }
}
