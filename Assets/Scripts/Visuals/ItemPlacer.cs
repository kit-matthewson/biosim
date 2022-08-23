using UnityEngine;

public struct ItemType {
    [Header("Distribution")]
    float frequency;
    [Range(0, 1)]
    float cutoff;
}

[RequireComponent(typeof(IslandGenerator))]
[RequireComponent(typeof(NoiseGenerator))]
public class ItemPlacer : MonoBehaviour {
    void Start() {

    }

    void Update() {

    }
}
