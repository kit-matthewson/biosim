using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores data on the current state of the evolution simulation.
/// </summary>
public class SimulationState : ScriptableObject {
    public int Generation;
    public List<Organism> CurrentGen;
    public bool Paused;
}
