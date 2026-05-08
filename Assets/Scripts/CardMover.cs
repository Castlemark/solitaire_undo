using UnityEngine;

public class CardMover : MonoBehaviour
{
    private const float MOVEMENT_THRESHOLD = 0.01f;
    private const float ROTATION_THRESHOLD = 0.1f;


    [SerializeField] private SpriteRenderer cardImage;

    [Header("Movement Settings")]
    [SerializeField] private float followSmoothTime = 0.1f;
    [SerializeField] private float rotationSmoothTime = 0.1f;
    [SerializeField] private float maxRotationDegrees = 25f;
    [SerializeField] private float rotationMultiplier = 5f;

    private Vector3 velocity = Vector3.zero;
    private Vector3 prevPosition;
    private float currentRotationZ;
    private float targetRotationZ;
    private Vector3 travelTargetPosition;

    private void Awake()
    {
        currentRotationZ = transform.eulerAngles.z;
        targetRotationZ = currentRotationZ;
        travelTargetPosition = Vector3.forward;
        prevPosition = transform.position;
    }

    public void TravelTo(Vector2 targetPosition)
    {
        travelTargetPosition = targetPosition;
        prevPosition = transform.position;
        velocity = Vector3.zero;
        currentRotationZ = transform.eulerAngles.z;
        targetRotationZ = currentRotationZ;
    }

    public void UpdateTravelTarget(Vector2 targetPosition)
    {
        travelTargetPosition = targetPosition;
    }

    public void UpdateMovement()
    {
        if (travelTargetPosition != Vector3.forward) // we use Vector3.forward as the default value
        {
            LerpTo(travelTargetPosition);
            if (Vector3.Distance(transform.position, travelTargetPosition) < MOVEMENT_THRESHOLD)
            {
                transform.position = travelTargetPosition;
                travelTargetPosition = Vector3.forward;
            }
        }
        else
        {
            // When not dragging and not traveling, lerp rotation to default
            targetRotationZ = 0f;
        }

        LerpCardRotation();
    }

    private void LerpTo(Vector3 targetPos)
    {
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, followSmoothTime);

        // Calculate targetrotation based on horizontal movement
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

        if (Mathf.Abs(currentRotationZ - targetRotationZ) < ROTATION_THRESHOLD)
        {
            currentRotationZ = targetRotationZ;
        }

        transform.rotation = Quaternion.Euler(0f, 0f, currentRotationZ);
    }
}
