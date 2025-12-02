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
    public GameObject animalPrefab;

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
    public string currentState;
    // 
    // public GameObject[] offspring;
    // private GameObject newOffspring;
    MapLoader mapLoader;


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
    public bool isBreeding = false; // true if currently in breeding process
    public bool hasLaunchedOffspring = false;    // true after this animal initiated offspring spawn recently
    public bool isHungry = false;
    public bool isThirsty = false;
    public bool isSleepy = false;
    public bool isScared = false;
    public bool isAggro = false;


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

    // Add near other fields
    public LayerMask livingEntityMask = 0;
    public LayerMask waterMask = 0;
    public float defaultVisionRange = 25f;
    public float defaultVisionAngle = 140f;

    protected override void Start()
    {
        base.Start(); // assigns instanceID
                      // PopulationManager.Instance.UpdateCount(genes.specieName, 1);

        mapLoader = GameObject.FindObjectOfType<MapLoader>();
        agent = GetComponent<NavMeshAgent>(); // for pathfinding
        _machine = gameObject.AddComponent<AnimalStateMachine>();
        _machine.setAnimal(this);

        // Defaults
        if (visionRange <= 0f) visionRange = defaultVisionRange;
        if (visionAngle <= 0f) visionAngle = defaultVisionAngle;
        if (visionInterval <= 0f) visionInterval = 1.0f;

        // Masks
        if (livingEntityMask == 0)
        {
            int mask = LayerMask.GetMask("LivingEntity");
            livingEntityMask = mask == 0 ? ~0 : mask; // fallback to Everything for debug
        }
        if (waterMask == 0)
        {
            int wmask = LayerMask.GetMask("Water");
            waterMask = wmask == 0 ? ~0 : wmask;
        }

        StartCoroutine(VisionRoutine());
        watersources = BuildWaterList();

        // specieName = speciesData.specieName;
        currentStage = GrowthStage.Child;
        hungerLevel = 50; // start mostly nourished
        thirstLevel = 50; //


        // initialize a default state
        // setstate(new RoamingState());
        // currentstate = AnimalState.Roaming;
    }

    protected override void FixedUpdate()
    {
        if (isDead) return;
        base.FixedUpdate();
        Grow();
        if (hungerLevel < hungerThreshold) isHungry = true; else isHungry = false;
        if (thirstLevel < thirstThreshold) isThirsty = true; else isThirsty = false;
        if (fearLevel >= fleeThreshold) isScared = true; else isScared = false;
        if (fearLevel > 0 && fearLevel < fleeThreshold) isAggro = true; else isAggro = false;
        // sleep bool

        if (hungerLevel <= 0 || thirstLevel <= 0)
        {
            Die();
        }
        
       
        // need to decrement hungerLevel over time
        hungerLevel -= Time.fixedDeltaTime * 0.5; // Decrease hunger over time
        thirstLevel -= Time.fixedDeltaTime * 0.5; // Decrease thirst over time (thirst depletes faster)
    }

    public override void Grow()
    {
        // Logic to transition between stages
        switch (currentStage)
        {
            case GrowthStage.Child:
                // Check conditions to become a Teen animal
                if (this.age > 15)
                {
                    currentStage = GrowthStage.Teen;
                    // increase scale to represent growth
                    transform.localScale *= 1.5f;
                    Debug.Log($"Animal, {specieName}, (ID: {instanceID}) has grown to the Teen stage.");
                }
                break;
            case GrowthStage.Teen:
                // Check conditions to become an Adult animal
                if (this.age > 30)
                {
                    currentStage = GrowthStage.Adult;
                    transform.localScale *= 1.5f;
                    Debug.Log($"Animal, {specieName}, (ID: {instanceID}) is now in the Adult stage.");
                }
                break;
            case GrowthStage.Adult:

                if (hungerLevel > 80.0 || thirstLevel > 80.0 || !isBreedable)
                {
                    double chance = reproductionChance;
                    if (chance > 1.0) chance = chance / 100.0;

                    if (Random.value < (float)chance)
                    {
                        isBreedable = true;
                        Debug.Log($"Animal, {specieName} (ID:{instanceID}) became breedable (roll={chance}).");
                    }

                }

               // if (hungerLevel < hungerThreshold || thirstLevel < thirstThreshold)
                //{
                  //  isBreedable = false;
                // }
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
                Max(0, (p1.nourishmentValueGene[0] + p2.nourishmentValueGene[0]) / 2 + Dist.Normal.Sample(0, 0.1)),
                Max(0, (p1.nourishmentValueGene[1] + p2.nourishmentValueGene[1]) / 2 + Dist.Normal.Sample(0, 0.05))
        };

        childGenes.movementSpeedGene = new double[]
        {
                Max(0, (p1.movementSpeedGene[0] + p2.movementSpeedGene[0]) / 2 + Dist.Normal.Sample(0, 0.1)),
                Max(0, (p1.movementSpeedGene[1] + p2.movementSpeedGene[1]) / 2 + Dist.Normal.Sample(0, 0.05))
        };

        childGenes.reproductionChanceGene = new double[]
        {
                Max(0, (p1.reproductionChanceGene[0] + p2.reproductionChanceGene[0]) / 2 + Dist.Normal.Sample(0, 0.1)),
                Max(0, (p1.reproductionChanceGene[1] + p2.reproductionChanceGene[1]) / 2 + Dist.Normal.Sample(0, 0.05))
        };

        childGenes.attackStrengthGene = new double[]
        {
                Max(0, (p1.attackStrengthGene[0] + p2.attackStrengthGene[0]) / 2 + Dist.Normal.Sample(0, 0.1)),
                Max(0, (p1.attackStrengthGene[1] + p2.attackStrengthGene[1]) / 2 + Dist.Normal.Sample(0, 0.05))
        };

        childGenes.heightGene = new double[]
        {
                Max(0, (p1.heightGene[0] + p2.heightGene[0]) / 2 + Dist.Normal.Sample(0, 0.1)),
                Max(0, (p1.heightGene[1] + p2.heightGene[1]) / 2 + Dist.Normal.Sample(0, 0.05))
        };

        childGenes.healthGene = new double[]
        {
                Max(0, (p1.healthGene[0] + p2.healthGene[0]) / 2 + Dist.Normal.Sample(0, 0.1)),
                Max(0, (p1.healthGene[1] + p2.healthGene[1]) / 2 + Dist.Normal.Sample(0, 0.05))
        };

        childGenes.specieName = p1.specieName; // inherit species name

        childGenes.hungerThresholdGene = new double[]
             {
                Max(0, (p1.hungerThresholdGene[0] + p2.hungerThresholdGene[0]) / 2 + Dist.Normal.Sample(0, 0.1)),
                Max(0, (p1.hungerThresholdGene[1] + p2.hungerThresholdGene[1]) / 2 + Dist.Normal.Sample(0, 0.05))
             };

        childGenes.thirstThresholdGene = new double[]
             {
                Max(0, (p1.thirstThresholdGene[0] + p2.thirstThresholdGene[0]) / 2 + Dist.Normal.Sample(0, 0.1)),
                Max(0, (p1.thirstThresholdGene[1] + p2.thirstThresholdGene[1]) / 2 + Dist.Normal.Sample(0, 0.05))
             };

        childGenes.fleeThresholdGene = new double[]
             {
                Max(0, (p1.fleeThresholdGene[0] + p2.fleeThresholdGene[0]) / 2 + Dist.Normal.Sample(0, 0.1)),
                Max(0, (p1.fleeThresholdGene[1] + p2.fleeThresholdGene[1]) / 2 + Dist.Normal.Sample(0, 0.05))
             };

        childGenes.sleepThresholdGene = new double[]
             {
                Max(0, (p1.sleepThresholdGene[0] + p2.sleepThresholdGene[0]) / 2 + Dist.Normal.Sample(0, 0.1)),
                Max(0, (p1.sleepThresholdGene[1] + p2.sleepThresholdGene[1]) / 2 + Dist.Normal.Sample(0, 0.05))
             };


        return childGenes;
    }

    IEnumerator VisionRoutine()
    {
        yield return new WaitForSeconds(Random.Range(0f, visionInterval));

        while (true)
        {
            visibleEntities = GetVisibleEntities();
            UpdateFear();
            yield return new WaitForSeconds(visionInterval);
        }
    }

    // this is called at start to build a list of water sources in the map
    // unlike living entities, this list is constant so that the animal "memorizes" water sources
    public List<GameObject> BuildWaterList()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 1000f, waterMask);
        List<GameObject> waterSources = new List<GameObject>();
        foreach (var hit in hits)
        {
            waterSources.Add(hit.gameObject);
        }
        return waterSources;
    }

    public void FindWaterSource()
    {
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
        if (this.waterTarget != null) return this.waterTarget;
        return null;
    }

    // Replace GetVisibleEntities with a more robust implementation
    public List<LivingEntity> GetVisibleEntities()
    {
        List<LivingEntity> visible = new List<LivingEntity>();

        // Broad phase: find potential targets on the livingEntityMask
        Collider[] hits = Physics.OverlapSphere(transform.position, visionRange, livingEntityMask);
        if (hits == null || hits.Length == 0) return visible;

        // Use the animal's height to place the "eye" origin sensibly:
        // - If height is known (>0), use a fraction of it (e.g., 40%) clamped to [0.5, 2.0] meters.
        // - Otherwise, fall back to 0.5m above ground.
        float eyeHeight;
        if (height > 0.0)
        {
            eyeHeight = Mathf.Clamp((float)(height * 0.4), 0.5f, 2.0f);
        }
        else
        {
            eyeHeight = 0.5f;
        }
        Vector3 eyeOrigin = transform.position + Vector3.up * eyeHeight;

        foreach (var hit in hits)
        {
            LivingEntity e = hit.GetComponent<LivingEntity>();
            if (e == null || e == this) continue;

            // Accept collider in child hierarchy as the same living entity
            var entity = hit.GetComponentInParent<LivingEntity>();
            if (entity != null) e = entity;

            // Distance check (optional: skip too-close self overlaps)
            float dist = Vector3.Distance(transform.position, e.transform.position);
            if (dist > visionRange) continue;

            // Field of view
            Vector3 dir = (e.transform.position - transform.position).normalized;
            if (visionAngle > 0f && Vector3.Angle(transform.forward, dir) > visionAngle * 0.5f)
                continue;

            // Line of sight check: allow hitting any collider in the entity's hierarchy
            if (Physics.Raycast(eyeOrigin, dir, out RaycastHit hitInfo, visionRange, ~0, QueryTriggerInteraction.Ignore))
            {
                // Try to resolve the LivingEntity from the collider hierarchy
                var hitEntity = hitInfo.collider.GetComponentInParent<LivingEntity>();
                if (hitEntity != null && hitEntity == e)
                {
                    visible.Add(e);
                    continue;
                }
            }

            // Fallback: if LOS fails but target is very near, still consider visible
            if (dist <= 1.5f)
            {
                visible.Add(e);
            }
        }

        return visible;
    }


    public void UpdateFear()
    {

        threat = DetectThreats();

        if (threat == null) { fearLevel = 0;  return; }
        


        // closer distance means higher fear, 100 max
        double fearValue = ((threat.attackStrength - this.attackStrength) / this.attackStrength) * 50.0;
        fearValue = Max(0.0, Min(100.0, fearValue));

        fearLevel = fearValue;

        if (fearLevel < fleeThreshold) SetTargetEntity(threat);

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
            .Where(a => a != this && a.isPredator && a.specieName != this.specieName && a.currentTarget == this)    // must be predator and not self
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
        // if (animal.mate == null || animal == this) return;
        if (!this.isBreedable || !animal.isBreedable) return;
        if (this.mate != null || animal.mate != null) return;
        this.SetMate(animal); animal.SetMate(this);


    }

    public void StartBreed(Animal mate)
    {
        if (mate == null) return;

        // Must be mutual pairing
        if (this.mate != mate || mate.mate != this) return;

        // Determine single initiator deterministically to avoid double-spawn
        Animal initiator = (this.instanceID <= mate.instanceID) ? this : mate;
        Animal other = (initiator == this) ? mate : this;

        // Quick guard: if already reserved or in cooldown, bail
        if (initiator.hasLaunchedOffspring) return;

        // Reserve the initiator atomically to prevent race-starting multiple coroutines
        if (initiator.isBreeding) return; // someone else already reserved
        initiator.isBreeding = true; // reserve immediately before starting coroutine

        // Start mating coroutine on the initiator; pass (initiator, other) so the coroutine sees correct roles
        initiator.StartCoroutine(initiator.BreedCoroutine(initiator, other));
    }

    public void SetMate(Animal animal)
    {
        this.mate = animal;
    }

    private IEnumerator BreedCoroutine(Animal requester, Animal partner)
    {
        Debug.LogWarning("starting breed routine");

        // mark both as in mating process
        this.isBreeding = true;
        if (partner != null) partner.isBreeding = true;

        // small handshake/animation window
        yield return new WaitForSeconds(0.5f);

        // verify both still valid and breedable
        if (partner == null || this == null)
        {
            CleanupAfterFailedBreed(partner);
            yield break;
        }

        if (this.mate != partner || partner.mate != this)
        {
            CleanupAfterFailedBreed(partner);
            yield break;
        }

        // Only the initiator (this) performs the spawn action
        if (!this.hasLaunchedOffspring)
        {
            // mark launched for both so partner won't also spawn
            this.hasLaunchedOffspring = true;
            partner.hasLaunchedOffspring = true;



            // Optionally set breedable false until cooldown / gestation complete
           // this.isBreedable = false;
           // partner.isBreedable = false;

            // Call Breed() � override this to perform actual instantiation and gene assignment
            Breed(this, partner);
        }

        // cleanup flags after short delay to ensure no immediate re-breeding
        yield return new WaitForSeconds(0.1f);
        this.isBreeding = false;
        if (partner != null) partner.isBreeding = false;
        this.ClearMate();
        if (partner != null) partner.ClearMate();

    }

    private void CleanupAfterFailedBreed(Animal partner)
    {
        this.isBreeding = false;
        if (partner != null) partner.isBreeding = false;
        if (partner != null && partner.mate == this) partner.mate = null;
        if (this.mate == partner) this.mate = null;
        Debug.LogWarning("Failed breed");

    }


    public void Breed(Animal parent1, Animal parent2)
    {
        if (parent1 == null || parent2 == null) return;
        if (parent1.specieName != parent2.specieName)
        {
            Debug.LogWarning("Breed aborted: parents are different species.");
            return;
        }

        GameObject prefab = genes.animalPrefab;
        if (prefab == null)
        {
            Debug.LogError($"Breed failed: genes.animalPrefab is null for species '{parent1.specieName}'. Assign prefab in species data.");
            return;
        }

        if (mapLoader == null)
            mapLoader = GameObject.FindObjectOfType<MapLoader>();
        if (mapLoader == null)
        {
            Debug.LogError("Breed failed: MapLoader not found in scene.");
            return;
        }

        bool foundSpot = false;
        Vector3 spawnPos = Vector3.zero;

        // Try multiple random nearby candidates (same approach Plant uses)
        for (int i = 0; i < 10; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * 3f;
            Vector3 candidate = transform.position + new Vector3(randomCircle.x, 0f, randomCircle.y);
            candidate = mapLoader.GetValidSpawnPoint(candidate.x, candidate.z, prefab, 2);

            NavMeshHit hit;
            if (!NavMesh.SamplePosition(candidate, out hit, 1.0f, NavMesh.AllAreas))
            {
                Debug.Log($"Breed() candidate #{i} NavMesh.SamplePosition failed at {candidate}");
                continue;
            }

            Vector3 candidateOnNav = hit.position;
            int mask = LayerMask.GetMask("LivingEntity");
            bool overlap = Physics.CheckSphere(candidateOnNav, 3f, mask);

            Debug.Log($"Breed() candidate #{i} pos={candidateOnNav} overlap={overlap}");
            if (!overlap)
            {
                spawnPos = candidateOnNav;
                foundSpot = true;
                break;
            }
        }

        // Fallback to midpoint between parents if no free spot found
        if (!foundSpot)
        {
            Vector3 midpoint = (parent1.transform.position + parent2.transform.position) * 0.5f;
            NavMeshHit midHit;
            if (NavMesh.SamplePosition(midpoint, out midHit, 5f, NavMesh.AllAreas))
            {
                spawnPos = midHit.position;
                Debug.LogWarning($"Breed(): fallback to parents' midpoint at {spawnPos}");
                foundSpot = true;
            }
            else
            {
                Debug.LogError("Breed(): no valid spawn point found; aborting spawn.");
                return;
            }
        }

        // Instantiate inactive so we can configure before child's Start/logic runs
        GameObject childGO = Instantiate(prefab, spawnPos, Quaternion.identity);
        childGO.SetActive(false);

        // Optional: give unique name to track in logs/hierarchy
        childGO.name = $"{prefab.name}_child_{System.DateTime.UtcNow.Ticks % 100000}_{parent1.instanceID}_{parent2.instanceID}";

        // Configure Animal component
        Animal child = childGO.GetComponent<Animal>();
        if (child == null)
        {
            Debug.LogError($"Breed failed: instantiated prefab '{childGO.name}' has no Animal component.");
            Destroy(childGO);
            return;
        }

        // Assign parent genes so child's Start() can average them if needed
        child.parent1Genes = parent1.genes;
        child.parent2Genes = parent2.genes;

        // Ensure sensible runtime defaults (prevent immediate death / invalid state)


        // Remove any accidental parenting (prevents container cleanup from destroying children)
        if (childGO.transform.parent != null)
            childGO.transform.SetParent(null);

        // Remove any embedded AnimalStateMachine on prefab instance (we will let child's Start create it)
        var existingSM = childGO.GetComponent<AnimalStateMachine>();
        if (existingSM != null) Destroy(existingSM);

        childGO.SetActive(true);

        if (PopulationManager.Instance != null)
            PopulationManager.Instance.UpdateCount(this.specieName, 1);

        Debug.Log($"Breed: spawned {child.specieName} (ID:{child.instanceID}, name:{childGO.name}) at {spawnPos}");
    }

    public void ClearMate()
    {
        this.mate = null;
        this.hasLaunchedOffspring = false;
        
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
        this.currentTargetPos = (target != null) ? target.transform : null;
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
        // Defensive: clear and return null if the currentTarget reference is gone, dead, or a corpse
        if (currentTarget == null) return null;

        // Unity destroyed check (UnityEngine.Object overload). Cast to Object to detect native destruction.
        if ((UnityEngine.Object)currentTarget == null)
        {
            currentTarget = null;
            currentTargetPos = null;
            return null;
        }

        // If target died or became a corpse, consider it no longer a valid active target


        return currentTarget;
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
        Vector3 fleeTarget = transform.position + fleeDir * 10f; // large number to just move away

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
        thirstLevel = Min(100.0, thirstLevel + 15.0); // arbitrary value for now
        Debug.Log("Successful Drink");

    }




}