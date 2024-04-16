using UnityEngine;

public class PathPredictionUsingVelocity : MonoBehaviour
{
    public Rigidbody rb;
    public int segments = 10;
    public float predictionTime = 2.0f; // Total prediction time
    public float updateInterval;

    private Vector3 previousVelocity;
    private float lastUpdateTime;

    void Start()
    {
        updateInterval = predictionTime / segments;
        previousVelocity = rb.velocity.normalized; // Normalize initial velocity
        lastUpdateTime = Time.time;
    }

    void Update()
    {
        if (Time.time - lastUpdateTime > updateInterval)
        {
            Vector3 currentVelocity = rb.velocity.normalized; // Normalize to ensure only direction is considered
            Vector3 angularVelocity = CalculateAngularVelocity(previousVelocity, currentVelocity, Time.time - lastUpdateTime);

            PredictPath(angularVelocity);
            previousVelocity = currentVelocity;
            lastUpdateTime = Time.time;
        }
    }

    Vector3 CalculateAngularVelocity(Vector3 oldVelocity, Vector3 newVelocity, float deltaTime)
    {
        if (deltaTime == 0 || oldVelocity == newVelocity) return Vector3.zero; // Prevent division by zero or unnecessary calculations

        Vector3 axisOfRotation = Vector3.Cross(oldVelocity, newVelocity).normalized;
        float angle = Vector3.Angle(oldVelocity, newVelocity);
        if (angle == 0) return Vector3.zero; // If no angle change, angular velocity is zero

        return axisOfRotation * (angle / deltaTime);
    }

    void PredictPath(Vector3 estimatedAngularVelocity)
    {
        Vector3 previousPosition = rb.position;
        Quaternion rotation = Quaternion.LookRotation(rb.velocity.normalized);
        float deltaTime = predictionTime / segments;

        for (int i = 0; i < segments; i++)
        {
            Quaternion angularDisplacement = Quaternion.Euler(estimatedAngularVelocity * Mathf.Rad2Deg * deltaTime);
            rotation *= angularDisplacement;
            Vector3 direction = rotation * Vector3.forward; // Get the forward direction from the rotation
            Vector3 displacement = direction * (rb.velocity.magnitude * deltaTime); // Correct displacement calculation
            Vector3 nextPosition = previousPosition + displacement;

            Debug.DrawLine(previousPosition, nextPosition, Color.red, deltaTime);
            previousPosition = nextPosition;
        }
    }
}
