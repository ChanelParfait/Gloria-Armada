using System.Collections;
using UnityEngine;

public class JetControl : MonoBehaviour
{
    [SerializeField] float aerodynamicEfficiency = 1f;
    [SerializeField] float aerodynamicDrag = 1f;
    [SerializeField] float wingspan = 10;
    [SerializeField] float accelLimit = 50;
    //[SerializeField] float positiveGLimit = 100;

    public void ResetPosition(float duration) {
        StartCoroutine(ResetPosition_async(duration));
    }
    IEnumerator ResetPosition_async(float duration) {
        Vector3 target = Vector3.zero;
        float elapsedTime = 0;
        while(elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            float completion = elapsedTime / duration;
            virtualCursor = Vector2.Lerp(virtualCursor, new Vector2(0, 0), completion * 10);
            transform.localPosition = Vector3.Lerp(transform.localPosition, target, completion * 10);
            yield return null;
        }
    }

    public float turnMultiplier = 0.2f;
    public float thrustMultiplier = 0.1f;

    [SerializeField] Transform viewport;
    [SerializeField] Transform target;
    Rigidbody rb;
    Vector3 lastViewportPosition;

    void Start() {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        lastViewportPosition = viewport.position;
    }

    Vector3 bakedVelocity = new Vector3(1, 0, 0) * 800;
    Vector2 virtualCursor = new Vector2(1, 0) * 5; //could implement better
    void FixedUpdate() {
        //just need to move the cursor in someway
        virtualCursor.x += Input.GetAxis("Mouse X");
        virtualCursor.y += Input.GetAxis("Mouse Y");

        //again, could implement better with perhaps a 3D cursor but this is fine for now
        Vector3 targetDirection = viewport.localRotation * virtualCursor;
        target.position = transform.position + targetDirection;
        Debug.DrawRay(transform.localPosition, targetDirection);

        Vector3 velocity = bakedVelocity + rb.velocity;

        //rotation
        targetDirection.x = Mathf.Clamp(targetDirection.x, 1, float.PositiveInfinity); // stop turn arounds
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
        Quaternion rotation = Quaternion.Slerp(rb.rotation, targetRotation, turnMultiplier);
        rb.MoveRotation(rotation);

        //for calculating aerodynamic forces
        Quaternion velocityOrientation = Quaternion.LookRotation(velocity, Vector3.up);
        float slipAngle = Quaternion.Angle(rb.rotation, velocityOrientation); //angle between wings and velocity
        float referenceArea = Mathf.Abs(Mathf.Sin(slipAngle) * wingspan); //frontal area

        var forwardAxis = rb.rotation * Vector3.forward; //from nose of plane
        var liftAxis = Vector3.Cross(velocity.normalized, viewport.forward);

        float drag = velocity.sqrMagnitude * referenceArea * aerodynamicDrag; //simplified drag equation

        var normalDirection = Mathf.Sign(Vector3.Dot(forwardAxis, liftAxis)); //1=lift, -1=downforce
        Debug.Log(forwardAxis);
        float normal = normalDirection * velocity.sqrMagnitude * referenceArea * aerodynamicEfficiency; //simplified lift equation
        //normal = Mathf.Clamp(normal, -accelLimit, accelLimit); //simulated stall

        Vector3 dragAccel = -velocity * drag;
        Vector3 normalAccel = liftAxis * normal;
        Vector3 thrustAccel = rb.rotation * Vector3.forward * thrustMultiplier;

        Debug.DrawRay(transform.localPosition, forwardAxis * 10, Color.blue);
        Debug.DrawRay(transform.localPosition, liftAxis * 10, Color.red);
        Debug.DrawRay(transform.localPosition, normalAccel, Color.cyan);
        Debug.DrawRay(transform.localPosition, thrustAccel, Color.magenta);
        Debug.DrawRay(transform.localPosition, velocity.normalized * 20, Color.green);



        Vector3 totalAccel = normalAccel + thrustAccel + dragAccel;
        rb.AddForce(totalAccel);
    }
}
