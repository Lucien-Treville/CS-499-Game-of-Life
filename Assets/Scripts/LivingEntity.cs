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


        PopulationManager.Instance.UpdateCount(specieName, -1);
        // Destroy(gameObject);
    }

    private void OnDeath()
    {
        // Placeholder for additional death handling logic
        var animator = GetComponent<Animator>();
        if (animator != null)
            animator.enabled = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        bool addedRb = false;
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.mass = 1f;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            addedRb = true;
        }

        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.None;

        if (addedRb)
        {
            // rotate 90 degrees on Z so the model visibly lies on its side
            transform.rotation = Quaternion.Euler(transform.eulerAngles + new Vector3(90f, 0f, 0f));
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