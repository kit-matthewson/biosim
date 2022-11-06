using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(EvolutionManager))]
public class CSVMaker : MonoBehaviour {

    public const string root = "output\\csv";
    public float bucketSize;
    public int writeFrequency;

    private Dictionary<string, FileHandler> files = new();
    private SimulationState state;

    private int last_write = -1;

    private void Start() {
        if (!Directory.Exists(root)) {
            Directory.CreateDirectory(root);
        }

        state = gameObject.GetComponent<EvolutionManager>().State;

        files["Fitness"] = new FileHandler($"{root}\\Fitness.csv");

        for (int i = 0; i < Organism.attributes.Length; i++) {
            string attribute_name = Organism.attributes[i].name;
            files[attribute_name] = new FileHandler($"{root}\\{attribute_name}.csv");
        }

        StringBuilder header_row = new("gen,");
        for (int i = 0; i < 2 / bucketSize; i++) {
            header_row.Append(i * bucketSize - 1);

            if (i < (2 / bucketSize) - 1) {
                header_row.Append(",");
            }
        }

        header_row.Append("\n");

        foreach (string key in files.Keys) {
            files[key].Write(header_row.ToString());
        }
    }

    private void Update() {
        if (state.generation != last_write && state.generation % writeFrequency == 0) {
            UpdateCSVs();
            last_write = state.generation;
        }
    }

    public void UpdateCSVs() {
        for (int i = -1; i < Organism.attributes.Length; i++) {
            int[] buckets = new int[(int)(2 / bucketSize)];

            StringBuilder line = new(buckets.Length * 2);

            line.Append(state.generation + ",");
            foreach (Organism organism in state.current_gen) {
                double value = i == -1 ? organism.Fitness : organism.AttributeValues[i];

                int bucket = Mathf.FloorToInt((float)((1 + value) / bucketSize));
                bucket = Mathf.Clamp(bucket, 0, buckets.Length - 1);

                buckets[bucket] += 1;
            }

            foreach (int frequency in buckets) {
                line.Append(frequency + ",");
            }

            line.Append("\n");

            files[i == -1 ? "Fitness" : Organism.attributes[i].name].Write(line.ToString());
        }
    }
}
