// File last updated: 09/09/2025
// Author: Lucien Treville
// File Created: 09/08/2025
// Description: This file contains the class implementation for Plants, inheriting from LivingEntity.

// Note: Originally the growth stages were going to be separate subclasses.
//       However, Gemini highlighted doing this would create an anti-pattern know as a "type-based state machine".
//       So, upon its advice, I am implementing the growth stages as an enum with growth logic in the Grow() method.


using UnityEngine;
using Dist = MathNet.Numerics.Distributions; // For normal distribution sampling

public struct PlantGenes
{
    public double[] nourishmentValueGene;
    public double[] fruitingChanceGene; // (0-1], percent chance of producing fruit each update
    public double[] sproutingChanceGene; // (0-1], percent chance of sprouting a new sapling when fruit is present)
    public double[] heightGene;
    public double[] healthGene;
    public string specieName;
}



public class Plant : LivingEntity
{

    // enum of growth stages
    public enum GrowthStage { Sapling, Young, Fruiting }

    // plant attributes
    // genes
    public PlantSpecies speciesGeneData; // reference to ScriptableObject defining species attributes
    public PlantGenes genes;
    public PlantGenes? parentGenes;

    // instance attributes
    public GrowthStage currentStage;
    // public double nourishmentValue; animals are nourishing too
    public double fruitingChance;
    public double sproutingChance;
    public bool hasFruit;

    protected override void Start()
    {
        base.Start();

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
            }

        specieName = speciesGeneData.specieName;
        nourishmentValue = Dist.Normal.Sample(genes.nourishmentValueGene[0], genes.nourishmentValueGene[1]);
        fruitingChance = Dist.Normal.Sample(genes.fruitingChanceGene[0], genes.fruitingChanceGene[1]);
        sproutingChance = Dist.Normal.Sample(genes.sproutingChanceGene[0], genes.sproutingChanceGene[1]);
        height = Dist.Normal.Sample(genes.heightGene[0], genes.heightGene[1]);
        health = Dist.Normal.Sample(genes.healthGene[0], genes.healthGene[1]);
        currentStage = GrowthStage.Sapling;
        hasFruit = false;
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
                    height *= 1.5f; // random growth factor
                    Debug.Log($"Plant, {specieName}, (ID: {instanceID}) has grown to the Young stage.");
                }
                break;
            case GrowthStage.Young:
                // Check conditions to become a Fruiting plant
                if (age > 30)
                {
                    currentStage = GrowthStage.Fruiting;
                    height *= 1.5f;
                    Debug.Log($"Plant, {specieName}, (ID: {instanceID}) is now in the Fruiting stage.");
                }
                break;
            case GrowthStage.Fruiting:
                // Implement logic for creating a new sapling
                if (hasFruit)
                {
                    // Sprout a new sapling nearby
                    if (Random.Range(0f, 1f) < sproutingChance)
                    {
                        // Instantiate a new Plant GameObject
                        Debug.Log($"A new sapling has sprouted nearby from {specieName}, ID:{instanceID}!");
                    }

                    // A method to instantiate a new Plant GameObject
                }
                else
                {
                    if (Random.Range(0f, 1f) < fruitingChance)
                    {
                        hasFruit = true;
                        Debug.Log($"Plant, {specieName}, (ID: {instanceID}) has produced fruit.");
                    }
                }
                break;
        }
    }

    public void Eaten()
    {
        Debug.Log($"Plant, {specieName}, (ID: {instanceID}) has been eaten and is now dead.");
        Die();
    }


    public PlantGenes PlantGenesMutation(PlantGenes parent)
    {
        PlantGenes mutatedGenes = parent;

        // Apply small random mutations to each gene
        mutatedGenes.nourishmentValueGene[0] += Random.Range(-0.5f, 0.5f);
        mutatedGenes.fruitingChanceGene[0] += Random.Range(-0.05f, 0.05f);
        mutatedGenes.sproutingChanceGene[0] += Random.Range(-0.05f, 0.05f);
        mutatedGenes.heightGene[0] += Random.Range(-0.5f, 0.5f);
        mutatedGenes.healthGene[0] += Random.Range(-0.5f, 0.5f);

        mutatedGenes.nourishmentValueGene[1] += Random.Range(-0.05f, 0.05f);
        mutatedGenes.fruitingChanceGene[1] += Random.Range(-0.05f, 0.05f);
        mutatedGenes.sproutingChanceGene[1] += Random.Range(-0.05f, 0.05f);
        mutatedGenes.heightGene[1] += Random.Range(-0.05f, 0.05f);
        mutatedGenes.healthGene[1] += Random.Range(-0.05f, 0.05f);

        return mutatedGenes;
    }

}
