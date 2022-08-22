using UnityEngine;

public class NoiseGenerator : MonoBehaviour {

    public float frequency;
    public float amplitude;

    Vector2 centre;

    private void Awake() {
        centre = Random.insideUnitCircle;
    }

    public float Evaluate(float x, float y) {
        float v;

        v = Mathf.PerlinNoise((x + centre.x) * frequency, (y + centre.y) * frequency);
        v *= amplitude;

        return v;
    }
}
