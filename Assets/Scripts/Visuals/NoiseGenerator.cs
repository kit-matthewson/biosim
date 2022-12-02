using JetBrains.Annotations;
using UnityEngine;

public class NoiseGenerator : MonoBehaviour {

    public float Frequency;

    private Vector2 _centre;

    [PublicAPI]
    private void Awake() {
        _centre = Random.insideUnitCircle * float.MaxValue;
    }

    public float Evaluate(float x, float y) {
        float v = Mathf.PerlinNoise((x + _centre.x) * Frequency, (y + _centre.y) * Frequency);

        return v;
    }
}
