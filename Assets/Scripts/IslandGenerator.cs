using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(NoiseGenerator))]
public class IslandGenerator : MonoBehaviour {

    public float width;
    public float density;
    public float edgeHeight;

    int size;

    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;

    NoiseGenerator noise;

    private void Start() {
        mesh = GetComponent<MeshFilter>().mesh;
        noise = GetComponent<NoiseGenerator>();

        size = (int)(width / density) + 3;

        PopulateMeshArrays();

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    private void PopulateMeshArrays() {
        vertices = new Vector3[(int)Mathf.Pow(size + 1, 2)];
        triangles = new int[(int)Mathf.Pow(size, 2) * 6];

        for (float y = -density, i = 0; y <= width + density; y += density) {
            for (float x = -density; x <= width + density; x += density, i++) {
                Vector3 pos = new Vector3(x, noise.Evaluate(x, y), y);

                if (y == -density) {
                    pos.y = edgeHeight;
                    pos.z = 0;
                } else if (y > width) {
                    pos.y = edgeHeight;
                    pos.z = (size * density) - (2 * density);
                }

                if (x == -density) {
                    pos.y = edgeHeight;
                    pos.x = 0;
                } else if (x > width) {
                    pos.y = edgeHeight;
                    pos.x = (size * density) - (2 * density);
                }

                vertices[(int)i] = pos;
            }
        }

        for (int y = 0, i = 0; y < size; y++) {
            for (int x = 0; x < size; x++, i += 6) {
                int offset = (y * (size + 1)) + x;
                new int[] { offset, offset + size + 1, offset + size + 2 }.CopyTo(triangles, i);
                new int[] { offset, offset + size + 2, offset + 1 }.CopyTo(triangles, i + 3);
            }
        }
    }
}
