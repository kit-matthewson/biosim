using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the SimulationSetup scene.
/// </summary>
public class SimulationSetupController : MonoBehaviour {
    public StaticMenuControllerHandle MenuController;
    public Slider MutationSlider;
    public TextMeshProUGUI MutationText;
    public Slider PopSizeSlider;
    public TextMeshProUGUI PopSizeText;

    private EvolutionConfig _config;

    [PublicAPI]
    private void Start() {
        Defaults();
    }

    [PublicAPI]
    private void Update() {
        _config.MutationStrength = MutationSlider.value;
        _config.InitialPopulationSize = Mathf.FloorToInt(PopSizeSlider.value);

        // Set text to a value from 0-10 instead of small fractions
        MutationText.text = (Mathf.Round(Mathf.Lerp(0, 10, MutationSlider.value / MutationSlider.maxValue) * 10) / 10).ToString();
        PopSizeText.text = PopSizeSlider.value.ToString();

        MenuController.StaticController.EvolutionConfig = _config;
    }

    /// <summary>
    /// Reset values to defaults (overwrites with a new <c>EvolutionConfig</c>).
    /// </summary>
    public void Defaults() {
        _config = ScriptableObject.CreateInstance<EvolutionConfig>();

        MutationSlider.value = (float)_config.MutationStrength;
        PopSizeSlider.value = _config.InitialPopulationSize;
    }
}