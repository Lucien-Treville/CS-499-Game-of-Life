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
    public int instanceID; // unique ID assigned by Unity


    // On our variable simulation time step
    protected virtual void FixedUpdate()
    {
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
        Destroy(gameObject);
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
