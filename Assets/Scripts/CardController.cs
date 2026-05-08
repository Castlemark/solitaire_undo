using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SpriteRenderer))]
public class CardController : MonoBehaviour, IClickable
{

    [SerializeField] private float cardFollowPositionOffset;
    [SerializeField] private float followSmoothTime;
    [SerializeField] private float rotationSmoothTime;
    [SerializeField] private float maxRotationDegrees;
    [SerializeField] private float rotationMultiplier;

    private bool isTraveling;
    private Vector3 travelTargetPosition;
    private bool isDragging;
    private Vector3 dragOffset;
    private Camera mainCamera;

    private Vector3 velocity = Vector3.zero;
    private Vector3 motionDelta;
    private Vector3 prevPosition;
    private float currentRotationZ;
    private float targetRotationZ;

    private void Awake()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("CardController: No main camera found. Dragging will not work until a camera is assigned.");
        }
    }

    public void OnClick()
    {
        if (mainCamera == null)
        {
            return;
        }

        if (isDragging)
        {
            StopDragging();
        }
        else
        {
            StartDragging();
        }
    }

    public void TravelTo(Vector3 position)
    {
        isTraveling = true;
        isDragging = false;

        travelTargetPosition = position;
        prevPosition = transform.position;
        velocity = Vector3.zero;
        currentRotationZ = transform.eulerAngles.z;
        targetRotationZ = currentRotationZ;
    }


    private void Update()
    {
        if (mainCamera == null)
        {
            return;
        }

        if (isDragging)
        {
            LerpFollow(Mouse.current.position.ReadValue());
        }
        else if (isTraveling)
        {
            LerpToWorldPoint(travelTargetPosition);
            if (Vector3.Distance(transform.position, travelTargetPosition + dragOffset) < 0.01f)
            {
                Debug.Log("CardController: Reached travel target position.");
                isTraveling = false;
                transform.position = travelTargetPosition + dragOffset; // Snap to target to avoid small discrepancies
            }
        }

        LerpCardRotation();
    }

    private void StartDragging()
    {
        isDragging = true;
        Vector3 mousePos = Mouse.current.position.ReadValue();
        Vector3 screenPoint = new(mousePos.x, mousePos.y, mainCamera.WorldToScreenPoint(transform.position).z);
        Vector3 worldPoint = mainCamera.ScreenToWorldPoint(screenPoint);

        Vector3 localTopCenter = Vector3.up * cardFollowPositionOffset * GetHalfHeight(); // Adjust the offset to be slightly below the top center of the card
        Vector3 topCenterOffset = transform.TransformVector(localTopCenter);
        Vector3 desiredPosition = worldPoint - topCenterOffset;
        dragOffset = desiredPosition - worldPoint;

        prevPosition = transform.position;
        velocity = Vector3.zero;
        currentRotationZ = transform.eulerAngles.z;
        targetRotationZ = currentRotationZ;

        EventBus.RaiseDragStarted(this);
    }

    private void StopDragging()
    {
        isDragging = false;
        targetRotationZ = 0f;

        EventBus.RaiseDragStopped(this);
    }

    // We lerp rotation separately to ensure it returns to 0 smoothly even when the card stops being dragged
    private void LerpCardRotation()
    {
        if (currentRotationZ != targetRotationZ)
        {
            // Always lerp rotation towards target
            currentRotationZ = Mathf.LerpAngle(currentRotationZ, targetRotationZ, Time.deltaTime / rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, 0f, currentRotationZ);
        }
    }

    private void LerpFollow(Vector2 target)
    {
        Vector3 screenPoint = new(target.x, target.y, mainCamera.WorldToScreenPoint(transform.position).z);
        Vector3 worldPoint = mainCamera.ScreenToWorldPoint(screenPoint);
        LerpToWorldPoint(worldPoint);
    }

    private void LerpToWorldPoint(Vector3 worldPoint)
    {
        Vector3 targetPos = worldPoint + dragOffset;

        // Smooth position following with delay
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, followSmoothTime);

        // Calculate desired rotation
        Vector3 motionDelta = prevPosition - transform.position;
        prevPosition = transform.position;

        targetRotationZ = Mathf.Clamp(rotationMultiplier * motionDelta.x, -maxRotationDegrees, maxRotationDegrees);
    }

    private float GetHalfHeight()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer.sprite != null)
        {
            return spriteRenderer.sprite.bounds.extents.y;
        }

        Debug.LogWarning("CardController: SpriteRenderer has no sprite assigned. Defaulting half height to 0.5f.");
        return 0.5f;
    }
}
