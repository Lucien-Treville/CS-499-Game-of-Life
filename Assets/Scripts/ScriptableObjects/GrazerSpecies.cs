// File last updated: 09/16/2025
// Author: Lucien Treville
// File Created: 09/16/2025
// Description: This file allows different grazer species to be defined within Unity as ScriptableObjects.
//              This is read by the Grazer class when setting attributes for an instance.


using UnityEngine;

[CreateAssetMenu(fileName = "New Grazer Species", menuName = "Simulation/Grazer Species")]
public class GrazerSpecies : ScriptableObject
{
    public GameObject animalPrefab; // reference to the prefab for this grazer species
    public string specieName;
    public double[] movementSpeedGene; // 
    public double[] reproductionChanceGene; // 
    public double[] attackStrengthGene; // 
    public double[] heightGene; // 
    public double[] healthGene; // 
    public double[] hungerThresholdGene;
    public double[] thirstThresholdGene; // determines stage of thirst in which animal will look for drink
    public double[] fleeThresholdGene; // determines flee threshold
    public double[] sleepThresholdGene;
    public double[] nourishmentValueGene;

}
