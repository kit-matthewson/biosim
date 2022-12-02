using JetBrains.Annotations;
using UnityEngine;

[System.Serializable]
public struct ItemType {
    public GameObject Prefab;
    public bool Inverse;

    [Range(0, 1)]
    public float Frequency;
}

[RequireComponent(typeof(NoiseGenerator))]
public class ItemPlacer : MonoBehaviour {
    public ItemType[] Items;
    public Transform Centre;

    [Header("Distribution")]
    public float Density;
    public float Variation;
    public Vector2 Range;

    [PublicAPI]
    private void Start() {
        NoiseGenerator noise = gameObject.GetComponent<NoiseGenerator>();

        for (float y = -Range.y; y < Range.y; y += Density) {
            for (float x = -Range.x; x < Range.x; x += Density) {
                float v = noise.Evaluate(x, y);

                foreach (ItemType item in Items) {
                    switch (item.Inverse) {
                        case true when v < item.Frequency:
                        case false when 1 - v < item.Frequency:
                            continue;
                    }

                    Vector2 pos = new Vector2(x, y) + Random.insideUnitCircle * Variation;
                    GameObject itemGameObject = Instantiate(item.Prefab, Centre);
                    itemGameObject.transform.position = new Vector3(pos.x, 0, pos.y);
                }
            }
        }
    }
}