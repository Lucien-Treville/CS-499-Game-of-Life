// File last updated: 09/16/2025
// Author: Lucien Treville
// File Created: 09/16/2025
// Description: This file contains the class implementation for Predators, inheriting from Animal.

using UnityEngine;
using UnityEngineInternal;
using Dist = MathNet.Numerics.Distributions; // For normal distribution sampling

public class Predator : Animal
{
    // Additional attributes specific to Predators can be added here
    public PredatorSpecies speciesGeneData; // reference to ScriptableObject defining species attributes


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
            }
        // assign instance attributes based on genes
        specieName = genes.specieName;
        movementSpeed = Dist.Normal.Sample(genes.movementSpeedGene[0], genes.movementSpeedGene[1]);
        reproductionChance = Dist.Normal.Sample(genes.reproductionChanceGene[0], genes.reproductionChanceGene[1]);
        attackStrength = Dist.Normal.Sample(genes.attackStrengthGene[0], genes.attackStrengthGene[1]);
        height = Dist.Normal.Sample(genes.heightGene[0], genes.heightGene[1]);
        health = Dist.Normal.Sample(genes.healthGene[0], genes.healthGene[1]);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        // Animal FixedUpdate
        // handles growthstage development
        // handles death if hungerLevel <= 0
        // Additional simulation logic for Predator can be added here
    }

    public override void Grow()
    {
        // Animal Grow called in FixedUpdate
        // Implement growth logic specific to Predators here
    }

    public override void Die()
    {
        base.Die(); // Call the base class Die method to handle common death logic
    }
}