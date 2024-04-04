using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewJetControl : MonoBehaviour
{

    //!!! replace this with event-driven from new input system
    Vector2 viewportTarget;
    void ReplaceWithEventDriven() {
        bool mouseDebug = true;
        if (mouseDebug) {
            viewportTarget.x = Input.GetAxis("Mouse X");
            viewportTarget.y = Input.GetAxis("Mouse Y");
        } else {
            viewportTarget.x = Input.GetAxis("Horizontal");
            viewportTarget.y = Input.GetAxis("Vertical");
        }
    }



    [SerializeField] Transform viewport;
    [SerializeField] Rigidbody character;
    [SerializeField] Transform flightPath;
    [SerializeField] float liftCoefficient;

    public float flightPathSpeed = 800;

    void FixedUpdate() {
        {
            Vector3 target = viewport.position + viewport.rotation * viewportTarget;
            Vector3 characterToTarget = target - characterToTarget;
            characterToTarget = Vector3.ProjectOnPlane(characterToTarget, viewport.forward);

            //!!! replace with blend between Vector3.up and ProjectOnPlane(characterToTarget, character.forward)
            Vector3 characterUpDirection = character.rotation * Vector3.up;

            Quaternion targetRotation = Quaternion.LookRotation(characterToTarget, character.rotation * Vector3.up);


            Vector3 bakedVelocity = flightPath.forward * flightPathSpeed;
            Vector3 velocity = bakedVelocity + character.velocity;

            float liftAcceleration = velocity.sqrMagnitude * liftCoefficient;

        }
    }
}
