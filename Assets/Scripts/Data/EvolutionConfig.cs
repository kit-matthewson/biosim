using UnityEngine;

/// <summary>
/// Stores configuration data on evolution parameters.
/// </summary>
[CreateAssetMenu(fileName = "Evolution Config", menuName = "ScriptableObject/EvolutionConfig")]
public class EvolutionConfig : ScriptableObject {
    public int InitialPopulationSize = 500;
    public int MinimumOffspring = 2;
    public int MaximumOffspring = 5;
    public double MutationStrength = 0.04;
}
