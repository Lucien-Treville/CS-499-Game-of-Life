// Fi// File last updated: 09/11/2025
// Author: Lucien Treville
// File Created: 09/11/2025
// Description: This file contains the class implementation for Animals, inheriting from LivingEntity.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static System.Math;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngineInternal;
using Dist = MathNet.Numerics.Distributions; // For normal distribution sampling

public struct AnimalGenes
{
    public double[] nourishmentValueGene;
    public double[] movementSpeedGene;
    public double[] reproductionChanceGene;
    public double[] attackStrengthGene;
    public double[] heightGene;
    public double[] healthGene;
    public string specieName;

    public double[] hungerThresholdGene; // determines stage of hunger in which animal will look for food
    public double[] thirstThresholdGene; // determines stage of thirst in which animal will look for drink
    public double[] fleeThresholdGene; // determines flee threshold
    public double[] sleepThresholdGene; // determines when animal sleeps
}


public class Animal : LivingEntity
{
    // enum of growth stages
    public enum GrowthStage { Child, Teen, Adult }

    // animal attributes
    // genes
    public AnimalGenes genes;
    public AnimalGenes? parent1Genes;
    public AnimalGenes? parent2Genes;

    // instance attributes
    public double movementSpeed; // how many tiles can move
    public double hungerLevel; // 0-100 scale (0 = starving, 100 = full)
    public double thirstLevel; // 0-100 scale (0 = dying of thirst, 100 = quenched)
    public double sleepLevel; // 0-100 scale (0 = sleep deprived, 100 = rested)
    public double fearLevel; //0-100 scale (0 = safe, 100 = terrified) 
    public double reproductionChance; // 0-1 scale
    public double attackStrength;
    public bool isPredator; // true if predator
    public bool isBreedable = false; // true if looking to breed
   

    public double hungerThreshold; // determines stage of hunger in which animal will look for food
    public double thirstThreshold; // determines stage of thirst in which animal will look for drink
    public double fleeThreshold; // determines flee threshold
    public double sleepThreshold; // determines when animal sleeps

    public float visionRange; // how far the animal can see
    public float visionAngle; // the angle the animal can see
    public float visionInterval = 5f; // how often the animal checks it vision
    public List<LivingEntity> visibleEntities = new List<LivingEntity>(); // the list of the visible entities 
    public List<GameObject> watersources = new List<GameObject>(); // list of visible water sources, instantiated at start

    public NavMeshAgent agent; // for navigation
    public LivingEntity currentTarget;
    public GameObject waterTarget;
    public Transform currentTargetPos; // food/prey target
    public AnimalStateMachine _machine;
    public Animal mate = null;
    public Animal threat = null; // flee "target"

    public GrowthStage currentStage;
    // public AnimalState currentState; // not implemented yet, but public variable for other entities to read

    protected override void Start()
    {
        base.Start(); // assigns instanceID
        agent = GetComponent<NavMeshAgent>(); // for pathfinding
        _machine = gameObject.AddComponent<AnimalStateMachine>();
        _machine.setAnimal(this);
        StartCoroutine(VisionRoutine()); 
        watersources = BuildWaterList();

        // specieName = speciesData.specieName;
        currentStage = GrowthStage.Child;
        hungerLevel = 70; // start mostly nourished


        // initialize a default state
        // setstate(new RoamingState());
        // currentstate = AnimalState.Roaming;
    }

    protected override void FixedUpdate()
    {
        if (isDead) return;
        base.FixedUpdate();
        Grow();
        if (hungerLevel <= 0)
        {
            Die();
        }
        // need to decrement hungerLevel over time
        hungerLevel -= Time.fixedDeltaTime; // Decrease hunger over time
    }

    public override void Grow()
    {
        // Logic to transition between stages
        switch (currentStage)
        {
            case GrowthStage.Child:
                // Check conditions to become a Teen animal
                if (age > 15)
                {
                    currentStage = GrowthStage.Teen;
                    // increase scale to represent growth
                    transform.localScale *= 1.5f;
                    Debug.Log($"Animal, {specieName}, (ID: {instanceID}) has grown to the Teen stage.");
                }
                break;
            case GrowthStage.Teen:
                // Check conditions to become an Adult animal
                if (age > 30)
                {
                    currentStage = GrowthStage.Adult;
                    transform.localScale *= 1.5f;
                    Debug.Log($"Animal, {specieName}, (ID: {instanceID}) is now in the Adult stage.");
                }
                break;
            case GrowthStage.Adult:
                // Implement logic for reproduction
                // if (hungerLevel > 80)
                // {
                //    setState( roaming/searching for mate); 
                // }
                break;
        }
    }

    public AnimalGenes AverageGenes(AnimalGenes p1, AnimalGenes p2)
    {
        AnimalGenes childGenes = new AnimalGenes();
        // for each gene field, average mean and stddev with slight mutation

        childGenes.nourishmentValueGene = new double[]
        {
                (p1.nourishmentValueGene[0] + p2.nourishmentValueGene[0]) / 2 + Dist.Normal.Sample(0, 0.1),
                (p1.nourishmentValueGene[1] + p2.nourishmentValueGene[1]) / 2 + Dist.Normal.Sample(0, 0.05)
        };

        childGenes.movementSpeedGene = new double[]
        {
                (p1.movementSpeedGene[0] + p2.movementSpeedGene[0]) / 2 + Dist.Normal.Sample(0, 0.1),
                (p1.movementSpeedGene[1] + p2.movementSpeedGene[1]) / 2 + Dist.Normal.Sample(0, 0.05)
        };

        childGenes.reproductionChanceGene = new double[]
        {
                (p1.reproductionChanceGene[0] + p2.reproductionChanceGene[0]) / 2 + Dist.Normal.Sample(0, 0.1),
                (p1.reproductionChanceGene[1] + p2.reproductionChanceGene[1]) / 2 + Dist.Normal.Sample(0, 0.05)
        };

        childGenes.attackStrengthGene = new double[]
        {
                (p1.attackStrengthGene[0] + p2.attackStrengthGene[0]) / 2 + Dist.Normal.Sample(0, 0.1),
                (p1.attackStrengthGene[1] + p2.attackStrengthGene[1]) / 2 + Dist.Normal.Sample(0, 0.05)
        };

        childGenes.heightGene = new double[]
        {
                (p1.heightGene[0] + p2.heightGene[0]) / 2 + Dist.Normal.Sample(0, 0.1),
                (p1.heightGene[1] + p2.heightGene[1]) / 2 + Dist.Normal.Sample(0, 0.05)
        };

        childGenes.healthGene = new double[]
        {
                (p1.healthGene[0] + p2.healthGene[0]) / 2 + Dist.Normal.Sample(0, 0.1),
                (p1.healthGene[1] + p2.healthGene[1]) / 2 + Dist.Normal.Sample(0, 0.05)
        };

        childGenes.specieName = p1.specieName; // inherit species name

        childGenes.hungerThresholdGene = new double[]
             {
                (p1.hungerThresholdGene[0] + p2.hungerThresholdGene[0]) / 2 + Dist.Normal.Sample(0, 0.1),
                (p1.hungerThresholdGene[1] + p2.hungerThresholdGene[1]) / 2 + Dist.Normal.Sample(0, 0.05)
             };

        childGenes.thirstThresholdGene = new double[]
             {
                (p1.thirstThresholdGene[0] + p2.thirstThresholdGene[0]) / 2 + Dist.Normal.Sample(0, 0.1),
                (p1.thirstThresholdGene[1] + p2.thirstThresholdGene[1]) / 2 + Dist.Normal.Sample(0, 0.05)
             };

        childGenes.fleeThresholdGene = new double[]
             {
                (p1.fleeThresholdGene[0] + p2.fleeThresholdGene[0]) / 2 + Dist.Normal.Sample(0, 0.1),
                (p1.fleeThresholdGene[1] + p2.fleeThresholdGene[1]) / 2 + Dist.Normal.Sample(0, 0.05)
             };

        childGenes.sleepThresholdGene = new double[]
             {
                (p1.sleepThresholdGene[0] + p2.sleepThresholdGene[0]) / 2 + Dist.Normal.Sample(0, 0.1),
                (p1.sleepThresholdGene[1] + p2.sleepThresholdGene[1]) / 2 + Dist.Normal.Sample(0, 0.05)
             };


        return childGenes;
    }

    IEnumerator VisionRoutine()
    {
        yield return new WaitForSeconds(Random.Range(0f, visionInterval));

        while (true)
        {
            visibleEntities = GetVisibleEntities();
            yield return new WaitForSeconds(visionInterval);
        }
    }

    // this is called at start to build a list of water sources in the map
    // unlike living entities, this list is constant so that the animal "memorizes" water sources
    public List<GameObject> BuildWaterList()
        {
        Collider[] hits = Physics.OverlapSphere(transform.position, 1000f, LayerMask.GetMask("Water"));
        List<GameObject> waterSources = new List<GameObject>();
        foreach (var hit in hits)
        {
            waterSources.Add(hit.gameObject);
        }
        return waterSources;
    }

    public void FindWaterSource() {            
        if (watersources.Count == 0) return;
        GameObject closestWater = watersources
            .OrderBy(w => Vector3.Distance(transform.position, w.transform.position))
            .FirstOrDefault();
        if (closestWater != null)
        {
            SetTargetWaterSource(closestWater);
        }
    }

    private void SetTargetWaterSource(GameObject water)
    {
        this.waterTarget = water;
    }

    public GameObject GetWaterTarget()
    {
        if(this.waterTarget != null) return this.waterTarget;
        return null;
    }

    // This builds a list of visible plants and animals by first creating a spherical collider, and then using raycasts to filter out occluded entites
    public List<LivingEntity> GetVisibleEntities()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, visionRange, LayerMask.GetMask("LivingEntity"));
        List<LivingEntity> visible = new List<LivingEntity>();

        foreach (var hit in hits)
        {
            LivingEntity e = hit.GetComponent<LivingEntity>();
            if (e == null) continue;

            Vector3 dir = (e.transform.position - transform.position).normalized;

            // 1) Field of view test
            if (Vector3.Angle(transform.forward, dir) > visionAngle * 0.5f)
                continue;

            // 2) Line of sight check (raycast)
            if (Physics.Raycast(transform.position, dir, out RaycastHit hitInfo, visionRange))
            {
                // Check if the thing we hit IS the entity
                if (hitInfo.collider.transform == e.transform)
                {
                    visible.Add(e);
                }
            }
        }

        return visible;
    }


    public void UpdateFear()
    {

        threat = DetectThreats();

        if (threat == null) { 
            fearLevel = Max(0, fearLevel - 10.0 * Time.deltaTime); 
            return; 
        }

        // distance to predator
        double distance = Vector3.Distance(transform.position, threat.transform.position);

        // closer distance means higher fear, 100 max
        double fearValue = ((visionRange - distance) / visionRange) * 100.0;
        fearValue = Max(0.0, Min(100.0, fearValue));

        fearLevel = Max(fearLevel, fearValue);

    }

    // looks for the closest predator to determine if fleeing is necessary
    public Animal DetectThreats()
    {
        // if (isPredator) return null;

        List<LivingEntity> visible = visibleEntities;

        if (visible.Count == 0) return null;

        // Filter for predators only
        List<Animal> predators = visible
            .Where(e => e != null)
            .OfType<Animal>()                         // only animals
            .Where(a => a != this && a.isPredator && a.specieName != this.specieName)    // must be predator and not self
            .ToList();
        if (predators.Count == 0) return null;

        // Return the closest predator
        Animal closestPredator = predators
            .OrderBy(a => Vector3.Distance(transform.position, a.transform.position))
            .First();

        return closestPredator;

    }

    public Animal FindBreedTarget()
    {

        if (this.mate != null) return this.mate;

        List<LivingEntity> visible = visibleEntities;

        if (visible.Count == 0)
        {
           return null;
        }

        // 2) Filter by type or species
        List<Animal> potentialTargets = visible
            .Where(e => e != null)
            .OfType<Animal>()
            .Where(e => e != this && e.specieName == this.specieName && e.isBreedable == true)  // filters for own species
            .ToList();

        if (potentialTargets.Count == 0)
        {
            
            return null;
        }

        // find the closest valid mate
        foreach (var candidate in potentialTargets)
        {
            RequestBreed(candidate);

            if (this.mate != null)
            {
                // Success: this animal and candidate agreed
                return this.mate;
            }
        }


        return null; 

    }

    public void RequestBreed(Animal animal)
    {
        if (animal.mate == null) { this.SetMate(animal); animal.SetMate(this); }
    }

    public void SetMate(Animal animal)
    {
        this.mate = animal;
    }

    public void Breed(Animal parent1, Animal parent2)
    {
        // insert breed logic
    }

    // the following methods look for the closest plant or animal entity for food purposes

    public void FindFood()
    {
        LivingEntity target = null;
        if (!isPredator)
        {
            target = FindClosestPlant();
        }
        if (isPredator)
        {
            target = FindClosestAnimal();
        }

        if (target != null)
        {
            SetTargetEntity(target);
        }
    }

    private void SetTargetEntity(LivingEntity target)
    {
        this.currentTarget = target;
    }

    private LivingEntity FindClosestAnimal()
    {
        return visibleEntities
            .Where(e => e != null)                // filter out destroyed UnityEngine.Objects early
            .OfType<Animal>()                     // cast to Animal safely
            .Where(a => a != this && a.specieName != this.specieName)
            .OrderBy(a => Vector3.Distance(transform.position, a.transform.position))
            .FirstOrDefault();

            
    }

    private LivingEntity FindClosestPlant()
    {
        return visibleEntities
            .Where(e => e != null)
            .OfType<Plant>()
            .OrderBy(e => Vector3.Distance(transform.position, e.transform.position))
            .FirstOrDefault();
    }

    public LivingEntity GetTargetEntity() 
    {
        if (this.currentTarget != null) return currentTarget;
        return null;
    }

    // returns the transform of the current target
    public Transform GetTarget()
    {
        if (this.currentTarget != null) return this.currentTarget.transform;
        if (this.waterTarget != null) return this.waterTarget.transform;
        return null;
    }



    public void ClearTarget()
    {
        this.currentTarget = null;
        this.waterTarget = null;
        this.currentTargetPos = null;
    }

    public float GetTargetDistance() 
        {
        Transform target = this.GetTarget();
        if (target != null)
        {
            return Vector3.Distance(this.transform.position, target.position);

        }
        return -1f;
    } 

    public void PursueTargetTransform(Transform t)
    {
        if (agent != null)
            agent.speed = (float)movementSpeed;
            agent.SetDestination(t.position);
    }

    public Vector3 GetWanderTarget()
    {
        Vector3 randomDirection = Random.insideUnitSphere * 10f;
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, 10f, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return transform.position; // fallback
    }

    // Usage
    public void Wander()
    {
        Vector3 wanderTarget = GetWanderTarget();
        MoveTo(wanderTarget);
    }


    // MoveTo accepting a Vector3 instead of Transform
    public void MoveTo(Vector3 targetPos)
    {
        if (agent == null) return;
        agent.speed = (float)movementSpeed;
        agent.SetDestination(targetPos);
    }

    public Transform GetThreat()
    {
        if (this.threat != null) return this.threat.transform;
        return null;
    }

    public void Flee(Transform threat)
    {
        if (agent == null || threat == null) return;

        // 1. Direction away from the threat
        Vector3 fleeDir = (transform.position - threat.position).normalized;

        // 2. Create a temporary target far in that direction
        Vector3 fleeTarget = transform.position + fleeDir * 100f; // large number to just move away

        // 3. Project target onto NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(fleeTarget, out hit, 100f, NavMesh.AllAreas))
            fleeTarget = hit.position;

        // 4. Set NavMeshAgent destination
        agent.speed = (float)movementSpeed;
        agent.SetDestination(fleeTarget);
    }


    public void AttackAnimal(LivingEntity animalTarget)
    {
        double damage = this.attackStrength;
        animalTarget.SufferAttack(damage);
    }

    public void Eat(LivingEntity food)
    {
        if (food is Plant plant)
        {
            hungerLevel = Min(100.0, hungerLevel + plant.nourishmentValue);
            thirstLevel = Min(100.0, thirstLevel + plant.nourishmentValue);
            plant.RemoveCorpse(); // consume the plant
            ClearTarget();
        }
        else if (food is Animal prey)
        {
            hungerLevel = Min(100.0, hungerLevel + prey.nourishmentValue);
            thirstLevel = Min(100.0, thirstLevel + prey.nourishmentValue);
            prey.RemoveCorpse(); // consume the prey
            ClearTarget();

        }
    }

    public void Drink()
    {
        thirstLevel = Min(100.0, thirstLevel + 1.0); // arbitrary value for now
    }

    


}