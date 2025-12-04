// File last updated: 09/08/2025
// Author: Lucien Treville
// File Created: 09/08/2025
// Description: This file contains the base class for all living entities (Plants, Grazers, Predators).


using UnityEngine;

public class LivingEntity : MonoBehaviour
{

    // Common Attributes
    public string specieName;
    public double age;
    public double health;
    public double height; // in meters
    public double nourishmentValue;
    public int instanceID; // unique ID assigned by Unity
    public double corpseHealth;

    // Death booleans
    public bool isDead = false;
    public bool isCorpse = false;




    // On our variable simulation time step
    protected virtual void FixedUpdate()
    {
        if (isDead) return;
        age += Time.fixedDeltaTime; // Increment age by the fixed time step

    }

    // After reaching a certain age, the entity will grow to its next stage
    public virtual void Grow()
    {
        // Placeholder for growth logic
    }

    // Method to handle death of the entity
    public virtual void Die()
    {

        // Placeholder for death logic
        // Need to implement logging of death
        // remove from GUI

        if (isDead) return;

        isDead = true;

        OnDeath(); // this might be used if we want to change the appearance of the dead plant/animal


        // stop vision routines
        try
        {
            StopAllCoroutines();
        }
        catch { }


        // stop movement
        var nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (nav != null)
        {
            nav.isStopped = true;
            nav.enabled = false;
        }

        isCorpse = true;


        if (PopulationManager.Instance != null)
            PopulationManager.Instance.UpdateCount(specieName, -1);

        Invoke(nameof(RemoveCorpse), 15f);
        // Destroy(gameObject);
    }

    private void OnDeath()
    {
        // Placeholder for additional death handling logic
        var animator = GetComponent<Animator>();
        if (animator != null)
            animator.enabled = false;

        // Freeze any rigidbodies (on root or child ragdoll bones) so the corpse stays put.
        var rbs = GetComponentsInChildren<Rigidbody>();
        foreach (var rb in rbs)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        
        
            // rotate 90 degrees on Z so the model visibly lies on its side
        transform.rotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0f, 0f, 90f));

        try
        {
            var obstacle = GetComponent<UnityEngine.AI.NavMeshObstacle>();
            if (obstacle == null)
                obstacle = gameObject.AddComponent<UnityEngine.AI.NavMeshObstacle>();

            if (obstacle != null)
            {
                // Try to match obstacle to available collider
                var box = GetComponent<BoxCollider>();
                var capsule = GetComponent<CapsuleCollider>();
                var anyCol = GetComponent<Collider>();

                if (box != null)
                {
                    obstacle.shape = UnityEngine.AI.NavMeshObstacleShape.Box;
                    obstacle.center = box.center;
                    obstacle.size = box.size;
                }
                else if (capsule != null)
                {
                    obstacle.shape = UnityEngine.AI.NavMeshObstacleShape.Capsule;
                    obstacle.center = capsule.center;
                    obstacle.radius = capsule.radius;
                    obstacle.height = capsule.height;
                }
                else if (anyCol != null)
                {
                    // Fallback: approximate from bounds
                    var b = anyCol.bounds;
                    obstacle.shape = UnityEngine.AI.NavMeshObstacleShape.Box;
                    // Convert world center to local space
                    obstacle.center = transform.InverseTransformPoint(b.center);
                    obstacle.size = b.size;
                }
                else
                {
                    // Last resort: reasonable default
                    obstacle.shape = UnityEngine.AI.NavMeshObstacleShape.Box;
                    obstacle.center = Vector3.zero;
                    obstacle.size = new Vector3(1f, 0.5f, 1f);
                }

                obstacle.carving = true;
                obstacle.carveOnlyStationary = true;
            }

        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"OnDeath obstacle setup failed for {specieName} (ID:{instanceID}): {ex.Message}");
        }


    }

    public virtual void RemoveCorpse()
    {
        if (!isCorpse) return;
        Debug.Log($"Corpse of {specieName} (ID: {instanceID}) is being removed from the simulation.");

        Destroy(gameObject);
    }

    // method to suffer attack damage
    public virtual void SufferAttack(double damage)
    {
        this.health -= damage;

        if (this.health <= 0)
        {
            Die();
        }

    }

    public virtual void SufferCorpseDamage(double damage)
    {
        if (!isCorpse) return;

        this.corpseHealth -= damage;

        if (this.corpseHealth <= 0)
        {
            RemoveCorpse();
        }
    }

    void OnDestroy()
    {
        // Log stack trace so we can see who called Destroy
        Debug.LogWarning($"OnDestroy(): {specieName} (ID:{instanceID}, name:{name}) destroyed. StackTrace:\n{new System.Diagnostics.StackTrace()}");
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        instanceID = gameObject.GetInstanceID();
    }

    // Update is called once per frame
    void Update()
    {

    }
}