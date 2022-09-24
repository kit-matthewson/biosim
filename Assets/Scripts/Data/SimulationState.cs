using System.Collections.Generic;
using UnityEngine;

public class SimulationState : ScriptableObject {
    public int generation;
    public List<Organism> current_gen;
}
