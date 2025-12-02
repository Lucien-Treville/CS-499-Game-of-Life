// File last updated: 09/16/2025
// Author: Lucien Treville
// File Created: 09/16/2025
// Description: This file contains the class implementation for Grazers, inheriting from Animal.

using UnityEngine;
using UnityEngineInternal;
using Dist = MathNet.Numerics.Distributions; // For normal distribution sampling

public class Grazer : Animal
{
    // Additional attributes specific to Grazers can be added here
    public GrazerSpecies speciesGeneData; // reference to ScriptableObject defining species attributes



    protected override void Start()
    {
        base.Start(); // Calls Animal.Start which calls LivingEntity.Start which assigns instanceID
        // Animal.Start sets currentStage = GrowthStage.Child
        // Animal.Start sets hungerLevel = 70

        if (parent1Genes.HasValue && parent2Genes.HasValue)
        {
            // Average parent genes for child
            genes = AverageGenes(parent1Genes.Value, parent2Genes.Value);
        }
        else
        {
            // First generation: use ScriptableObject
            genes.attackStrengthGene = speciesGeneData.attackStrengthGene;
            genes.heightGene = speciesGeneData.heightGene;
            genes.healthGene = speciesGeneData.healthGene;
            genes.movementSpeedGene = speciesGeneData.movementSpeedGene;
            genes.reproductionChanceGene = speciesGeneData.reproductionChanceGene;
            genes.specieName = speciesGeneData.specieName;
            genes.hungerThresholdGene = speciesGeneData.hungerThresholdGene;
            genes.thirstThresholdGene = speciesGeneData.thirstThresholdGene;
            genes.fleeThresholdGene = speciesGeneData.fleeThresholdGene;
            genes.sleepThresholdGene = speciesGeneData.sleepThresholdGene;
            genes.nourishmentValueGene = speciesGeneData.nourishmentValueGene;
        }

        // assign instance attributes based on genes
        genes.animalPrefab = speciesGeneData.animalPrefab;

        specieName = genes.specieName;
        movementSpeed = Dist.Normal.Sample(genes.movementSpeedGene[0], genes.movementSpeedGene[1]);
        reproductionChance = Dist.Normal.Sample(genes.reproductionChanceGene[0], genes.reproductionChanceGene[1]);
        attackStrength = Dist.Normal.Sample(genes.attackStrengthGene[0], genes.attackStrengthGene[1]);
        height = Dist.Normal.Sample(genes.heightGene[0], genes.heightGene[1]);
        health = Dist.Normal.Sample(genes.healthGene[0], genes.healthGene[1]);
        nourishmentValue = Dist.Normal.Sample(genes.nourishmentValueGene[0], genes.nourishmentValueGene[1]);

        // NEW: initialize thresholds from genes (clamped to sensible ranges)
        hungerThreshold = System.Math.Max(1.0, System.Math.Min(99.0, Dist.Normal.Sample(genes.hungerThresholdGene[0], genes.hungerThresholdGene[1])));
        thirstThreshold = System.Math.Max(1.0, System.Math.Min(99.0, Dist.Normal.Sample(genes.thirstThresholdGene[0], genes.thirstThresholdGene[1])));
        fleeThreshold   = System.Math.Max(0.0, System.Math.Min(100.0, Dist.Normal.Sample(genes.fleeThresholdGene[0], genes.fleeThresholdGene[1])));
        sleepThreshold  = System.Math.Max(0.0, System.Math.Min(100.0, Dist.Normal.Sample(genes.sleepThresholdGene[0], genes.sleepThresholdGene[1])));
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate(); // Call the base class FixedUpdate method
        // Animal decrements hungerLevel and calls Grow
        // LivingEntity increments age

    }

    public override void Grow()
    {
        // Implement growth logic specific to Grazers here
        // Animal Grow is called in FixedUpdate
        base.Grow(); // Call the base class Grow method
    }

    public override void Die()
    {
        base.Die(); // Call the base class Die method to handle common death logic
    }



}