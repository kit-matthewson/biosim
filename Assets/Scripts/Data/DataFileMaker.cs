using System;
using System.Text;
using UnityEngine;

public class DataFileMaker : MonoBehaviour {

    public SimulationState state;

    [Header("File Config")]
    public float bucketSize;
    public float bucketRange;
    public const string rootFolder = "output\\csv";

    private FileHandler fitnessCSV;

    private void Awake() {
        fitnessCSV = new(rootFolder + "fitness.csv");

        fitnessCSV.Write("gen,");
        for (double i = -bucketRange; i <= bucketRange; i += bucketSize) {
            fitnessCSV.Write(i + ",");
        }
        fitnessCSV.Write("\n");
    }

    public void UpdateCSVs() {
        int[] buckets = new int[(int)(2 * bucketRange / bucketSize)];

        StringBuilder line = new(buckets.Length * 2); // Use SB to reduce IO operations

        line.Append(state.generation + ",");
        foreach (Organism organism in state.organisms) {
            int bucket = Mathf.FloorToInt((float)((organism.Fitness + bucketRange) / bucketRange * (buckets.Length / 2))); // Interpolate fitness into f-bucket
            try {
                buckets[bucket] = buckets[bucket] + 1;
            } catch (IndexOutOfRangeException e) {
                Debug.LogError($"{bucket} {buckets.Length} {organism.Fitness}, {e}");
            }
        }

        foreach (int f in buckets) {
            line.Append(f + ",");
        }

        line.Append("\n");

        fitnessCSV.Write(line.ToString());
    }
}
