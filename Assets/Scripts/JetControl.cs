using System.Collections;
using UnityEngine;

public class JetControl : MonoBehaviour
{
    [SerializeField] float aerodynamicEfficiency = 1f;
    [SerializeField] float aerodynamicDrag = 1f;
    [SerializeField] float wingspan = 10;
    [SerializeField] float negativeGLimit = -20;

    public void ResetPosition(float duration) {
        StartCoroutine(ResetPosition_async(duration));
    }
    IEnumerator ResetPosition_async(float duration) {
        Vector3 target = Vector3.zero;
        float elapsedTime = 0;
        while(elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            float completion = elapsedTime / duration;
            virtualCursor = Vector2.Lerp(virtualCursor, new Vector2(0, 0), completion);
            transform.localPosition = Vector3.Lerp(transform.localPosition, target, completion);
            yield return null;
        }
    }

    public float turnMultiplier = 0.2f;
    public float thrustMultiplier = 0.1f;

    [SerializeField] Transform viewport;
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
        Debug.DrawRay(transform.localPosition, targetDirection);

        Vector3 velocity = bakedVelocity + rb.velocity;

        //split the inputs (not used)
        float axisInput = Vector3.Dot(targetDirection.normalized, bakedVelocity.normalized);  //-1=reverseThrust, 1=thrusts, 0=turning
        Vector3 sideAxis = Vector3.Cross(-bakedVelocity.normalized, -viewport.forward);
        float axialInput = Vector3.Dot(targetDirection.normalized, sideAxis); //-1=left, 1,right, 0=neutral


        //rotation
        targetDirection.x = Mathf.Clamp(targetDirection.x, 1, float.PositiveInfinity);
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
        Quaternion rotation = Quaternion.Slerp(rb.rotation, targetRotation, turnMultiplier);

        //for calculating aerodynamic forces
        float slipAngle = Quaternion.Angle(rb.rotation, rotation);
        float referenceArea = Mathf.Sin(slipAngle) * wingspan;

        //aerodynamic forces
        float drag = velocity.sqrMagnitude * referenceArea * aerodynamicDrag; //simplified drag equation
        float normal = velocity.sqrMagnitude * referenceArea * aerodynamicEfficiency; //simplified lift equation

        var forwardAxis = rb.rotation * Vector3.forward;
        var sidewardAxis = Vector3.Cross(forwardAxis, viewport.forward);
        Vector3 dragAccel = -velocity * drag;
        Vector3 normalAccel = sidewardAxis * Mathf.Sign(axialInput) * normal;
        Vector3 thrustAccel = rb.rotation * Vector3.forward * thrustMultiplier;
        Debug.DrawRay(transform.localPosition, forwardAxis * 10, Color.blue);
        Debug.DrawRay(transform.localPosition, sidewardAxis * 10, Color.red);
        Debug.DrawRay(transform.localPosition, normalAccel * 10, Color.blue);
        Debug.DrawRay(transform.localPosition, sidewardAxis * 10, Color.red);



        //Could enforce g force limits - e.g. 10G up (KO), 2G down (pop!)
#region
        if(normalAccel.sqrMagnitude < negativeGLimit) {
            Quaternion upsideDown = Quaternion.LookRotation(transform.forward, -transform.up);
            //rb.rotation = Quaternion.Slerp(rb.rotation, upsideDown, turn);
        }
        else if (normalAccel.sqrMagnitude > 0){
            Quaternion upside = Quaternion.LookRotation(transform.forward, transform.up);
            //rb.rotation = Quaternion.Slerp(rb.rotation, upside, turn);
        }
#endregion

        //apply rotation here in case we clamp based on normalAccel or slipAngle
        rb.MoveRotation(rotation);

        Vector3 totalAccel = normalAccel + thrustAccel;
        rb.AddForce(totalAccel);
    }
}
