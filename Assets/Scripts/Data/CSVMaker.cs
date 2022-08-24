using System;
using System.IO;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(EvolutionManager))]
public class CSVMaker : MonoBehaviour {

    public const string root = "output\\csv";
    public float bucketSize; // Size of buckets for frequency distributions
    public float bucketRounding;

    private FileHandler[] files;
    private SimulationState state;

    private void Start() {
        if (!Directory.Exists(root)) {
            Directory.CreateDirectory(root);
        }

        state = gameObject.GetComponent<EvolutionManager>().State;

        files = new FileHandler[Organism.attributes.Length + 1];

        files[0] = new FileHandler($"{root}\\Fitness.csv");

        for (int i = 0; i < Organism.attributes.Length; i++) {
            string attribute_name = Organism.attributes[i].name;
            files[i + 1] = new FileHandler($"{root}\\{attribute_name}.csv");
        }

        StringBuilder sb = new();
        for (double i = -1; i < 1 + bucketSize; i += bucketSize) {
            // sb.Append(Mathf.Round((float)(i * (double)Mathf.Pow(10, bucketRounding))) / (double)Mathf.Pow(10, bucketRounding));
            sb.Append(i.ToString("0.00"));
            if (i < 1) {
                sb.Append(",");
            }
        }
        sb.Append("\n");

        string header_row = sb.ToString();

        for (int i = 0; i < files.Length; i++) {
            files[i].Write(header_row);
        }
    }

    public void UpdateCSVs() {
        //int[] buckets = new int[(int)(2 / bucketSize)];

        //StringBuilder line = new(buckets.Length * 2); // Use SB to reduce IO operations

        //line.Append(state.generation + ",");
        //foreach (Organism organism in state.organisms) {
        //    int bucket = Mathf.FloorToInt((float)((organism.Fitness + 1) / (buckets.Length / 2))); // Interpolate fitness into f-bucket
        //    try {
        //        buckets[bucket] = buckets[bucket] + 1;
        //    } catch (IndexOutOfRangeException e) {
        //        Debug.LogError($"{bucket} {buckets.Length} {organism.Fitness}, {e}");
        //    }
        //}

        //foreach (int f in buckets) {
        //    line.Append(f + ",");
        //}

        //line.Append("\n");

        //fitnessCSV.Write(line.ToString());
    }
}
