using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private Camera mainCamera;

    void Awake()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("InputHandler: No main camera found. Input handling will not work until a camera is assigned.");
        }
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        var rayCastHit = Physics2D.GetRayIntersection(mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue()));
        if (!rayCastHit.collider) return;

        var clickable = rayCastHit.collider.gameObject.GetComponent<IClickable>();

        if (clickable != null)
        {
            clickable.OnClick();
        }
    }
}
