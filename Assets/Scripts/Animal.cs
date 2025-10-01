// File last updated: 09/11/2025
// Author: Lucien Treville
// File Created: 09/11/2025
// Description: This file contains the class implementation for Animals, inheriting from LivingEntity.

using UnityEngine;
using UnityEngineInternal;
using Dist = MathNet.Numerics.Distributions; // For normal distribution sampling

public class Animal : LivingEntity
{
    // enum of growth stages
    public enum GrowthStage { Child, Teen, Adult }

    // animal attributes
    // genes
    public int movementSpeedGene; // how many tiles can move
    public double hungerLevelGene; // 0-100 scale (0 = starving, 100 = full)
    public double reproductionChanceGene; // 0-1 scale
    public double attackStrengthGene; 

    // instance attributes
    public int movementSpeed; // how many tiles can move
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

    public override void SimulateStep(float timeStep)
    {
        base.SimulateStep(timeStep);
        Grow();
        if (hungerLevel <= 0)
        {
            Die();
        }
        // need to decrement 
      
    }

    public override void Grow()
    {
        // Logic to transition between stages
        switch (currentStage)
        {
            case GrowthStage.Child:
                // Check conditions to become a Teen animal
                if (age > 10)
                {
                    currentStage = GrowthStage.Teen;
                    height *= 1.5f; // random growth factor
                    Debug.Log($"Animal, {specieName}, (ID: {instanceID}) has grown to the Teen stage.");
                }
                break;
            case GrowthStage.Teen:
                // Check conditions to become an Adult animal
                if (age > 20)
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




}