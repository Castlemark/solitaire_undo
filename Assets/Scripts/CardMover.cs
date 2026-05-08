using UnityEngine;

public class CardMover : MonoBehaviour
{
    [SerializeField] private SpriteRenderer cardImage;

    [Header("Movement Settings")]
    [SerializeField] private float cardFollowPositionOffset = 0.8f;
    [SerializeField] private float followSmoothTime = 0.1f;
    [SerializeField] private float rotationSmoothTime = 0.1f;
    [SerializeField] private float maxRotationDegrees = 25f;
    [SerializeField] private float rotationMultiplier = 5f;

    private Vector3 dragOffset;
    private Vector3 velocity = Vector3.zero;
    private Vector3 prevPosition;
    private float currentRotationZ;
    private float targetRotationZ;
    private bool isTraveling;
    private Vector3 travelTargetPosition;

    private void Awake()
    {
        currentRotationZ = transform.eulerAngles.z;
        targetRotationZ = currentRotationZ;
        prevPosition = transform.position;
    }

    public void StartDragging(Vector2 screenPosition, Camera camera)
    {
        if (camera == null)
        {
            return;
        }

        isTraveling = false;
        CalculateDragOffset(screenPosition, camera);
        prevPosition = transform.position;
        velocity = Vector3.zero;
        currentRotationZ = transform.eulerAngles.z;
        targetRotationZ = currentRotationZ;
    }

    public void TravelTo(Vector3 worldPosition)
    {
        isTraveling = true;
        travelTargetPosition = worldPosition;
        prevPosition = transform.position;
        velocity = Vector3.zero;
        currentRotationZ = transform.eulerAngles.z;
        targetRotationZ = currentRotationZ;
    }

    public void StopTravel()
    {
        isTraveling = false;
    }

    public void SetReturnToNeutralRotation()
    {
        targetRotationZ = 0f;
    }

    public void UpdateMovement(Camera camera, bool isDragging, Vector2 mouseScreenPosition)
    {
        if (camera == null)
        {
            return;
        }

        if (isDragging)
        {
            LerpFollow(mouseScreenPosition, camera);
        }
        else if (isTraveling)
        {
            LerpToWorldPoint(travelTargetPosition);
            if (Vector3.Distance(transform.position, travelTargetPosition + dragOffset) < 0.01f)
            {
                transform.position = travelTargetPosition + dragOffset;
                isTraveling = false;
            }
        }

        LerpCardRotation();
    }

    private void LerpFollow(Vector2 screenPosition, Camera camera)
    {
        Vector3 screenPoint = new(screenPosition.x, screenPosition.y, camera.WorldToScreenPoint(transform.position).z);
        Vector3 worldPoint = camera.ScreenToWorldPoint(screenPoint);
        LerpToWorldPoint(worldPoint);
    }

    private void LerpToWorldPoint(Vector3 worldPoint)
    {
        Vector3 targetPos = worldPoint + dragOffset;

        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, followSmoothTime);

        Vector3 motionDelta = prevPosition - transform.position;
        prevPosition = transform.position;

        targetRotationZ = Mathf.Clamp(rotationMultiplier * motionDelta.x, -maxRotationDegrees, maxRotationDegrees);
    }

    private void LerpCardRotation()
    {
        if (currentRotationZ == targetRotationZ)
        {
            return;
        }

        currentRotationZ = Mathf.LerpAngle(currentRotationZ, targetRotationZ, Time.deltaTime / rotationSmoothTime);
        transform.rotation = Quaternion.Euler(0f, 0f, currentRotationZ);
    }

    private void CalculateDragOffset(Vector2 screenPosition, Camera camera)
    {
        Vector3 screenPoint = new(screenPosition.x, screenPosition.y, camera.WorldToScreenPoint(transform.position).z);
        Vector3 worldPoint = camera.ScreenToWorldPoint(screenPoint);

        Vector3 localTopCenter = Vector3.up * cardFollowPositionOffset * cardImage.sprite.bounds.extents.y;
        Vector3 topCenterOffset = transform.TransformVector(localTopCenter);
        Vector3 desiredPosition = worldPoint - topCenterOffset;
        dragOffset = desiredPosition - worldPoint;
    }
}
