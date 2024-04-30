using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    float damage;
    [SerializeField]
    float lifetime;
    [SerializeField]
    float speed;
    [SerializeField]
    LayerMask collisionMask;
    [SerializeField]
    float width;

    Plane owner;
    Rigidbody rb;
    Vector3 lastPosition;
    float startTime;

    public void Fire(Plane owner) {
        this.owner = owner;
        rb = GetComponent<Rigidbody>();
        startTime = Time.time;

        rb.AddRelativeForce(new Vector3(0, 0, speed), ForceMode.VelocityChange);
        rb.AddForce(owner.getRBVelocity(), ForceMode.VelocityChange);
        lastPosition = rb.position;
    }

    void FixedUpdate() {
        if (Time.time > startTime + lifetime) {
            Destroy(gameObject);
            return;
        }

        var diff = rb.position - lastPosition;
        lastPosition = rb.position;

        Ray ray = new Ray(lastPosition, diff.normalized);
        RaycastHit hit;

        if (Physics.SphereCast(ray, width, out hit, diff.magnitude, collisionMask.value)) {
            Plane other = hit.collider.GetComponent<Plane>();

            if (other != null && other != owner) {
                other.ApplyDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}
