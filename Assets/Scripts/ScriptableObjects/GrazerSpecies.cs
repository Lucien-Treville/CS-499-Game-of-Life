// File last updated: 09/16/2025
// Author: Lucien Treville
// File Created: 09/16/2025
// Description: This file allows different grazer species to be defined within Unity as ScriptableObjects.
//              This is read by the Grazer class when setting attributes for an instance.


using UnityEngine;

[CreateAssetMenu(fileName = "New Grazer Species", menuName = "Simulation/Grazer Species")]
public class GrazerSpecies : ScriptableObject
{
    public string specieName;
    public double[] movementSpeedGene; // 
    public double[] reproductionChanceGene; // 
    public double[] attackStrengthGene; // 
    public double[] heightGene; // 
    public double[] healthGene; // 
}
