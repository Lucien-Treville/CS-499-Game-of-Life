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
        specieName = speciesGeneData.specieName;
        movementSpeed = (int)Dist.Normal.Sample(speciesGeneData.movementSpeedGene[0], speciesGeneData.movementSpeedGene[1]);
        reproductionChance = Dist.Normal.Sample(speciesGeneData.reproductionChanceGene[0], speciesGeneData.reproductionChanceGene[1]);
        attackStrength = Dist.Normal.Sample(speciesGeneData.attackStrengthGene[0], speciesGeneData.attackStrengthGene[1]);
        height = Dist.Normal.Sample(speciesGeneData.heightGene[0], speciesGeneData.heightGene[1]);
        health = Dist.Normal.Sample(speciesGeneData.healthGene[0], speciesGeneData.healthGene[1]);
    }

    public override void SimulateStep(float timeStep)
    {
        base.SimulateStep(timeStep); // Call the base class SimulateStep method
        // Additional simulation logic for Grazer can be added here
    }

    public override void Grow()
    {
        base.Grow();
        // Implement growth logic specific to Grazers here
    }

    public override void Die()
    {
        base.Die(); // Call the base class Die method to handle common death logic
    }
}