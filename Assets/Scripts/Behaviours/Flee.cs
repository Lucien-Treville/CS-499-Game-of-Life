using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Flee : MonoBehaviour
{
    public Transform pursuer;       // The object to flee from
    public float speed = 5f;        // Movement speed
    public float safeDistance = 10f; // Distance at which it starts fleeing

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation; // prevent tipping over
    }

    void FixedUpdate()
    {
        if (pursuer == null) return;

        Vector3 toPursuer = pursuer.position - transform.position;
        float distance = toPursuer.magnitude;

        if (distance < safeDistance)
        {
            // Calculate flee direction
            Vector3 fleeDirection = (-toPursuer).normalized;

            // Apply velocity
            rb.MovePosition(rb.position + fleeDirection * speed * Time.fixedDeltaTime);

            // Optional: face movement direction
            if (fleeDirection != Vector3.zero)
                rb.MoveRotation(Quaternion.LookRotation(fleeDirection));
        }
        else
        {
            // Stop moving when safe
            rb.velocity = Vector3.zero;
        }
    }
}