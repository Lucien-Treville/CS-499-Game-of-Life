// File last updated: 09/11/2025
// Author: Lucien Treville
// File Created: 09/11/2025
// Description: This file contains the class implementation for Animals, inheriting from LivingEntity.

using UnityEngine;
using UnityEngineInternal;
using Dist = MathNet.Numerics.Distributions; // For normal distribution sampling

public struct AnimalGenes
{
    public double[] movementSpeedGene;
    public double[] reproductionChanceGene;
    public double[] attackStrengthGene;
    public double[] heightGene;
    public double[] healthGene;
    public string specieName;
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
    public double reproductionChance; // 0-1 scale
    public double attackStrength;
    public GrowthStage currentStage;
    // public AnimalState currentState; // not implemented yet, but public variable for other entities to read

    protected override void Start()
    {
        base.Start(); // assigns instanceID

        // specieName = speciesData.specieName;
        currentStage = GrowthStage.Child;
        hungerLevel = 70; // start mostly nourished

        // initialize a default state
        // setstate(new RoamingState());
        // currentstate = AnimalState.Roaming;
    }

    protected override void FixedUpdate()
    {
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
                    height *= 1.5f; // random growth factor
                    Debug.Log($"Animal, {specieName}, (ID: {instanceID}) has grown to the Teen stage.");
                }
                break;
            case GrowthStage.Teen:
                // Check conditions to become an Adult animal
                if (age > 30)
                {
                    currentStage = GrowthStage.Adult;
                    height *= 1.5f;
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

            return childGenes;
        }


}