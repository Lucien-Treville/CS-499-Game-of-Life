// File last updated: 09/16/2025
// Author: Lucien Treville
// File Created: 09/16/2025
// Description: This file allows different predator species to be defined within Unity as ScriptableObjects.
//              This is read by the Predator class when setting attributes for an instance.


using UnityEngine;

[CreateAssetMenu(fileName = "New Predator Species", menuName = "Simulation/Predator Species")]
public class PredatorSpecies : ScriptableObject
{
    public string specieName;
    public double[] movementSpeedGene; // 
    public double[] reproductionChanceGene; // 
    public double[] attackStrengthGene; // 
    public double[] heightGene; // 
    public double[] healthGene; // 
}
