// File last updated: 09/16/2025
// Author: Lucien Treville
// File Created: 09/16/2025
// Description: This file allows different predator species to be defined within Unity as ScriptableObjects.
//              This is read by the Predator class when setting attributes for an instance.


using UnityEngine;

[CreateAssetMenu(fileName = "New Predator Species", menuName = "Simulation/Predator Species")]
public class PredatorSpecies : ScriptableObject
{
    public GameObject animalPrefab; // reference to the prefab for this predator species
    public string specieName;
    public double[] movementSpeedGene; // 
    public double[] breedTimerGene; // 
    public double[] litterSizeGene; // 
    public double[] attackStrengthGene; // 
    public double[] heightGene; // 
    public double[] healthGene; // 
    public double[] hungerThresholdGene;
    public double[] thirstThresholdGene; // determines stage of thirst in which animal will look for drink
    public double[] fleeThresholdGene; // determines flee threshold
    public double[] sleepThresholdGene;
    public double[] nourishmentValueGene;

}
