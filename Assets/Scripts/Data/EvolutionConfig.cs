using UnityEngine;

[CreateAssetMenu(fileName = "Evolution Config", menuName = "ScriptableObject/EvolutionConfig")]
public class EvolutionConfig : ScriptableObject {
    public int initialPopulationSize = 500;
    public int minimumOffspring = 2;
    public int maximumOffspring = 5;
    public double mutationStrength = 0.04;
}
