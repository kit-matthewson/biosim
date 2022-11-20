using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SimulationSetupController : MonoBehaviour {

    public Slider mutationSlider;
    public TextMeshProUGUI mutationText;
    public Slider popSizeSlider;
    public TextMeshProUGUI popSizeText;

    EvolutionConfig config;

    private void Start() {
        Defaults();
    }

    private void Update() {
        config.mutationStrength = mutationSlider.value;
        config.initialPopulationSize = Mathf.FloorToInt(popSizeSlider.value);

        // Set mutation text to a value from 0-10 instead of small fractions
        mutationText.text = (Mathf.Round(Mathf.Lerp(0, 10, mutationSlider.value / mutationSlider.maxValue) * 10) / 10).ToString();
        popSizeText.text = popSizeSlider.value.ToString();
    }

    public void Defaults() {
        config = ScriptableObject.CreateInstance<EvolutionConfig>();
        
        mutationSlider.value = (float)config.mutationStrength;
        popSizeSlider.value = config.initialPopulationSize;
    }
}