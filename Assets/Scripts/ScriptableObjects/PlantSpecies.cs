// File last updated: 09/09/2025
// Author: Lucien Treville
// File Created: 09/09/2025
// Description: This file allows different plant species to be defined within Unity as ScriptableObjects.
//              This is read by the Plant class when setting attributes for an instance.


using UnityEngine;

[CreateAssetMenu(fileName = "New Plant Species", menuName = "Simulation/Plant Species")]
public class PlantSpecies : ScriptableObject
{
    public GameObject plantPrefab; // reference to the prefab for this plant species
    public string specieName;
    public double[] nourishmentValueGene; // [mean, stddev]
    public double[] fruitingChanceGene;
    public double[] sproutingChanceGene;
    public double[] heightGene;
    public double[] healthGene;
}