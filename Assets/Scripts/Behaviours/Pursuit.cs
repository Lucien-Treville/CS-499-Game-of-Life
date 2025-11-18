using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Pursuit : MonoBehaviour
{
    public Transform target;          // The object to chase
    public float speed = 5f;          // Movement speed
    public float predictionFactor = 1.5f; // How far ahead to predict

    private Rigidbody rb;
    private Rigidbody targetRb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation; // prevent tipping over
    }

    void Start()
    {
        if (target != null)
            targetRb = target.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (target == null) return;

        // Predict target’s future position
        Vector3 targetVelocity = targetRb != null ? targetRb.velocity : Vector3.zero;
        Vector3 predictedPosition = target.position + targetVelocity * predictionFactor;

        // Move toward predicted position
        Vector3 direction = (predictedPosition - transform.position).normalized;

        rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);

        // Optional: rotate to face target
        if (direction != Vector3.zero)
            rb.MoveRotation(Quaternion.LookRotation(direction));
    }
}
