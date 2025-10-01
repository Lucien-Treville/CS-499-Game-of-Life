// File last updated: 09/09/2025
// Author: Lucien Treville
// File Created: 09/08/2025
// Description: This file contains the class implementation for Plants, inheriting from LivingEntity.

// Note: Originally the growth stages were going to be separate subclasses.
//       However, Gemini highlighted doing this would create an anti-pattern know as a "type-based state machine".
//       So, upon its advice, I am implementing the growth stages as an enum with growth logic in the Grow() method.


using UnityEngine;
using Dist = MathNet.Numerics.Distributions; // For normal distribution sampling

public class Plant : LivingEntity
{

    // enum of growth stages
    public enum GrowthStage { Sapling, Young, Fruiting }

    // plant attributes
    // genes
    public PlantSpecies speciesGeneData; // reference to ScriptableObject defining species attributes
    public double[] nourishmentValueGene; 
    public double[] fruitingChanceGene; // (0-1], percent chance of producing fruit each update
    public double[] sproutingChanceGene; // (0-1], percent chance of sprouting a new sapling when fruit is present)
    public double[] heightGene;
    public double[] healthGene;
    // instance attributes
    public GrowthStage currentStage;
    public double nourishmentValue;
    public double fruitingChance;
    public double sproutingChance;
    public bool hasFruit;

    protected override void Start()
    {
        base.Start();
        nourishmentValueGene   = speciesGeneData.nourishmentValueGene;
        fruitingChanceGene     = speciesGeneData.fruitingChanceGene;
        sproutingChanceGene    = speciesGeneData.sproutingChanceGene;
        heightGene             = speciesGeneData.heightGene;
        healthGene             = speciesGeneData.healthGene;

        specieName          = speciesGeneData.specieName;
        nourishmentValue    = Dist.Normal.Sample(nourishmentValueGene[0], nourishmentValueGene[1]);
        fruitingChance      = Dist.Normal.Sample(fruitingChanceGene[0], fruitingChanceGene[1]);
        sproutingChance     = Dist.Normal.Sample(sproutingChanceGene[0], sproutingChanceGene[1]);
        height              = Dist.Normal.Sample(heightGene[0], heightGene[1]);
        health              = Dist.Normal.Sample(healthGene[0], healthGene[1]);
        currentStage        = GrowthStage.Sapling;
        hasFruit            = false;
    }

    public override void SimulateStep(float timeStep)
    {
        base.SimulateStep(timeStep);
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
                if (age > 10)
                {
                    currentStage = GrowthStage.Young;
                    height *= 1.5f; // random growth factor
                    Debug.Log($"Plant, {specieName}, (ID: {instanceID}) has grown to the Young stage.");
                }
                break;
            case GrowthStage.Young:
                // Check conditions to become a Fruiting plant
                if (age > 20)
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



}
