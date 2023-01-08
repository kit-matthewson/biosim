using System.Collections.Generic;
using System.IO;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;

[RequireComponent(typeof(EvolutionManager))]
public class CsvMaker : MonoBehaviour {
    public const string Root = "output\\csv";
    public float BucketSize;
    public int WriteFrequency;

    public readonly Dictionary<string, FileHandler> Files = new();
    private SimulationState _state;

    private int _lastWrite = -1;

    [PublicAPI]
    private void Start() {
        if (!Directory.Exists(Root)) {
            Directory.CreateDirectory(Root);
        }

        _state = gameObject.GetComponent<EvolutionManager>().State;

        Files["Fitness"] = new FileHandler($"{Root}\\Fitness.csv");

        for (int i = 0; i < Organism.Attributes.Length; i++) {
            string attributeName = Organism.Attributes[i].Name;
            Files[attributeName] = new FileHandler($"{Root}\\{attributeName}.csv");
        }

        StringBuilder headerRow = new("gen,");

        for (int i = 0; i < 2 / BucketSize; i++) {
            headerRow.Append(i * BucketSize - 1);

            if (i < 2 / BucketSize - 1) {
                headerRow.Append(",");
            }
        }

        headerRow.Append("\n");

        foreach (string key in Files.Keys) {
            Files[key].Write(headerRow.ToString());
        }
    }

    [PublicAPI]
    private void Update() {
        if (_state.Generation < _lastWrite + WriteFrequency) return;

        UpdateCsvs();
        _lastWrite = _state.Generation;
    }

    public void UpdateCsvs() {
        for (int i = -1; i < Organism.Attributes.Length; i++) {
            int[] buckets = new int[(int)(2 / BucketSize)];

            StringBuilder line = new(buckets.Length * 2);

            line.Append(_state.Generation + ",");

            foreach (Organism organism in _state.CurrentGen) {
                double value = i == -1 ? organism.Fitness : organism.AttributeValues[i];

                int bucket = Mathf.FloorToInt((float)((1 + value) / BucketSize));
                bucket = Mathf.Clamp(bucket, 0, buckets.Length - 1);

                buckets[bucket] += 1;
            }

            foreach (int frequency in buckets) {
                line.Append(frequency + ",");
            }

            line.Append("\n");

            Files[i == -1 ? "Fitness" : Organism.Attributes[i].Name].Write(line.ToString());
        }
    }
}