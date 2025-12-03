// File last updated: 09/09/2025
// Author: Lucien Treville
// File Created: 09/08/2025
// Description: This file contains the class implementation for Plants, inheriting from LivingEntity.

using UnityEngine;
using Dist = MathNet.Numerics.Distributions; // For normal distribution sampling
using UnityEngine.AI;
using System.Collections.Generic;

public struct PlantGenes
{
    public GameObject plantPrefab;
    public double[] nourishmentValueGene;
    public double[] fruitingChanceGene;   // (0-1], percent chance of producing fruit each update
    public double[] sproutingChanceGene;  // (0-1], percent chance of sprouting a new sapling when fruit is present)
    public double[] heightGene;
    public double[] healthGene;
    public string specieName;
}

public class Plant : LivingEntity
{
    MapLoader mapLoader;

    // Growth stages of the plant
    public enum GrowthStage { Sapling, Young, Fruiting }

    // Species-level data (ScriptableObject)
    public PlantSpecies speciesGeneData; // reference to ScriptableObject defining species attributes

    // Genetic data for this individual
    public PlantGenes genes;
    public PlantGenes? parentGenes;

    // Instance attributes
    public GrowthStage currentStage;
    public double fruitingChance;
    public double sproutingChance;
    public bool hasFruit;

    // apples for apple tree
    private List<GameObject> apple = new List<GameObject>();

    protected override void Start()
    {
        mapLoader = GameObject.FindObjectOfType<MapLoader>();
        base.Start();

        // Initialize genes either from parent or from species data
        if (parentGenes.HasValue)
        {
            // Slightly mutate parent genes for child
            genes = PlantGenesMutation(parentGenes.Value);
        }
        else
        {
            // First generation: use ScriptableObject
            genes.nourishmentValueGene = speciesGeneData.nourishmentValueGene;
            genes.fruitingChanceGene = speciesGeneData.fruitingChanceGene;
            genes.sproutingChanceGene = speciesGeneData.sproutingChanceGene;
            genes.heightGene = speciesGeneData.heightGene;
            genes.healthGene = speciesGeneData.healthGene;
            genes.specieName = speciesGeneData.specieName;
            genes.plantPrefab = speciesGeneData.plantPrefab;
        }

        specieName = speciesGeneData.specieName;
        nourishmentValue = Dist.Normal.Sample(genes.nourishmentValueGene[0], genes.nourishmentValueGene[1]);
        fruitingChance = Dist.Normal.Sample(genes.fruitingChanceGene[0], genes.fruitingChanceGene[1]);
        sproutingChance = Dist.Normal.Sample(genes.sproutingChanceGene[0], genes.sproutingChanceGene[1]);
        height = Dist.Normal.Sample(genes.heightGene[0], genes.heightGene[1]);
        health = Dist.Normal.Sample(genes.healthGene[0], genes.healthGene[1]);

        currentStage = GrowthStage.Sapling;
        hasFruit = false;

        // Cache apple child objects for "Apple Tree"
        if (specieName == "Apple Tree")
        {
            Transform[] allChildren = GetComponentsInChildren<Transform>(true);

            foreach (Transform child in allChildren)
            {
                if (child.CompareTag("Apple"))
                {
                    apple.Add(child.gameObject);
                    child.gameObject.SetActive(false);
                }
            }
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        Grow();
    }

    // override base class Grow method
    public override void Grow()
    {
        // Logic to transition between stages
        switch (currentStage)
        {
            case GrowthStage.Sapling:
                // Check conditions to become a Young plant
                if (age > 15)
                {
                    currentStage = GrowthStage.Young;
                    transform.localScale *= 1.5f;
                }
                break;

            case GrowthStage.Young:
                // Check conditions to become a Fruiting plant
                if (age > 30)
                {
                    currentStage = GrowthStage.Fruiting;
                    transform.localScale *= 1.5f;
                }
                break;

            case GrowthStage.Fruiting:
                // Fruiting/sprouting logic
                if (hasFruit)
                {
                    // Sprout a new sapling nearby
                    if (Random.Range(0f, 1f) < sproutingChance)
                    {
                        GameObject babyPrefab = genes.plantPrefab;
                        bool foundSpot = false;
                        Vector3 spawnPos = Vector3.zero;

                        for (int i = 0; i < 10; i++)
                        {
                            Vector2 randomCircle = Random.insideUnitCircle * 3f;
                            spawnPos = transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);
                            spawnPos = mapLoader.GetValidSpawnPoint(spawnPos.x, spawnPos.z, babyPrefab, 2);

                            NavMeshHit hit;
                            // small (1.0) radius so it fails if in lake
                            if (NavMesh.SamplePosition(spawnPos, out hit, 1.0f, NavMesh.AllAreas))
                            {
                                spawnPos.x = hit.position.x;
                                spawnPos.z = hit.position.z;

                                int mask = LayerMask.GetMask("LivingEntity");
                                if (!Physics.CheckSphere(spawnPos, 3f, mask))
                                {
                                    foundSpot = true;
                                    break;
                                }
                            }
                        }

                        if (foundSpot)
                        {
                            // spawn baby plant and give it parent genes
                            GameObject babyObj = Instantiate(babyPrefab, spawnPos, Quaternion.identity);
                            Plant babyPlant = babyObj.GetComponent<Plant>();
                            if (babyPlant != null)
                            {
                                babyPlant.parentGenes = this.genes;
                            }

                            if (PopulationManager.Instance != null)
                                PopulationManager.Instance.UpdateCount(genes.specieName, 1);
                        }
                    }
                }
                else
                {
                    // Try to produce fruit
                    if (Random.Range(0f, 1f) < fruitingChance)
                    {
                        hasFruit = true;

                        if (genes.specieName == "Apple Tree")
                        {
                            foreach (var ap in apple)
                            {
                                ap.SetActive(true);
                            }
                        }
                    }
                }
                break;
        }
    }

    /// <summary>
    /// Called when an animal has fully consumed this plant's nourishment.
    /// </summary>
    public void Eaten()
    {
<<<<<<< HEAD
        if (isDead) return;
=======
        this.health -= nourishmentValue;
        if (health <= 0)
        {
            Die();
        }
    }
>>>>>>> 6b5eb0e5ea159e5a3dd1a0a2b717dc2b5acb68ba

        Debug.Log($"Plant, {specieName}, (ID: {instanceID}) has been eaten and is now dead.");
        Die();  // marks isDead/isCorpse and updates PopulationManager
    }

    public PlantGenes PlantGenesMutation(PlantGenes parent)
    {
        PlantGenes mutatedGenes = parent;

        // Apply small random mutations to each gene and keep them positive
        mutatedGenes.nourishmentValueGene[0] *= (double)Random.Range(0.85f, 1.15f);
        mutatedGenes.fruitingChanceGene[0]   *= (double)Random.Range(0.85f, 1.15f);
        mutatedGenes.sproutingChanceGene[0]  *= (double)Random.Range(0.85f, 1.15f);
        mutatedGenes.heightGene[0]           *= (double)Random.Range(0.85f, 1.15f);
        mutatedGenes.healthGene[0]           *= (double)Random.Range(0.85f, 1.15f);

        mutatedGenes.nourishmentValueGene[1] *= (double)Random.Range(0.85f, 1.15f);
        mutatedGenes.fruitingChanceGene[1]   *= (double)Random.Range(0.85f, 1.15f);
        mutatedGenes.sproutingChanceGene[1]  *= (double)Random.Range(0.85f, 1.15f);
        mutatedGenes.heightGene[1]           *= (double)Random.Range(0.85f, 1.15f);
        mutatedGenes.healthGene[1]           *= (double)Random.Range(0.85f, 1.15f);

        mutatedGenes.plantPrefab = parent.plantPrefab;

        return mutatedGenes;
    }

    void PrintPlantGenes(PlantGenes genes)
    {
        Debug.LogWarning($"Specie: {genes.specieName}");
        Debug.LogWarning($"Fruiting Chance: {genes.fruitingChanceGene[0]}, {genes.fruitingChanceGene[1]}");
        Debug.LogWarning($"Sprouting Chance: {genes.sproutingChanceGene[0]}, {genes.sproutingChanceGene[1]}");
    }
}
