using JetBrains.Annotations;
using UnityEngine;

[RequireComponent(typeof(EvolutionManager))]
public class UiController : MonoBehaviour {
    private EvolutionManager _evolutionManager;

    [PublicAPI]
    private void Start() {
        _evolutionManager = GetComponent<EvolutionManager>();
    }
    
    [PublicAPI]
    public void SkipGenerations(int n) {
        _evolutionManager.GenerationGoal = _evolutionManager.State.Generation + n;
    }

    [PublicAPI]
    public void PlayPause() {
        _evolutionManager.State.Paused = !_evolutionManager.State.Paused;
    }
}