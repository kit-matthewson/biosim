using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(EvolutionManager))]
public class UiController : MonoBehaviour {
    private EvolutionManager _evolutionManager;

    public GameObject NonGraphUi;
    public GameObject GraphUi;

    private bool _graphsShown = false;

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

    [PublicAPI]
    public void Graph() {
        _graphsShown = !_graphsShown;

        NonGraphUi.SetActive(!_graphsShown);
        GraphUi.SetActive(_graphsShown);
    }
}