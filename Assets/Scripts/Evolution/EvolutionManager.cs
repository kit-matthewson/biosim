using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

public class EvolutionManager : MonoBehaviour {
    public GameObject organismObject;
    public Transform organismParent;

    public int initialPopSize = 100;
    public int gensPerStep = 10;
    public float genLength = 5;

    public int minOffspring = 2;
    public int maxOffspring = 5;

    public double mutationStrength = 0.01;

    public static string rootFolder = "output\\csv";
    public FileHandler fitnessCSV = new(rootFolder + "fitness.csv");

    public string filesPath;
    public float bucketSize;
    public float bucketRange;

    private List<Organism> organisms;
    private readonly List<GameObject> organism_objects = new();

    private float last_generation;

    private int generation = 0;

    private void Awake() {
        InitialisePopulation();
        last_generation = -genLength;

        fitnessCSV.Write("gen,");
        for (double i = -bucketRange; i <= bucketRange; i += bucketSize) {
            fitnessCSV.Write(Math.Round(i * 100) / 100 + ",");
        }
        fitnessCSV.Write("\n");
    }

    private void Update() {
        if (Time.time - last_generation >= genLength) {
            GenerateGameObjects();
            
            for (int i = 0; i < gensPerStep; i++) {
                organisms = GetNextGeneration(organisms);
                generation++;
            }

            UpdateCSVs();

            last_generation = Time.time;
        }
    }

    private void UpdateCSVs() {
        int[] buckets = new int[(int)(4 * bucketRange / bucketSize)];

        StringBuilder line = new(buckets.Length * 2); // Use SB to reduce IO operations

        line.Append(generation + ",");
        foreach (Organism organism in organisms) {
            int bucket = Mathf.FloorToInt((float)((organism.Fitness + bucketRange) / (bucketRange * 2) * buckets.Length)); // Interpolate fitness into f-bucket
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

    private void InitialisePopulation() {
        organisms = new List<Organism>(initialPopSize);
        for (int i = 0; i < initialPopSize; i++) {
            organisms.Add(new());
        }
    }

    private void GenerateGameObjects() {
        for (int i = 0; i < organism_objects.Count; i++) {
            if (i < organisms.Count) {
                organism_objects[i].SetActive(true);
                organism_objects[i].GetComponent<OrganismController>().Initialise(organisms[i]);
            } else {
                organism_objects[i].SetActive(false);
            }
        }

        for (int i = organism_objects.Count; i < organisms.Count; i++) {
            GameObject new_org = Instantiate(organismObject, organismParent);
            new_org.GetComponent<OrganismController>().Initialise(organisms[i]);
            organism_objects.Add(new_org);
        }
    }

    private List<Organism> GetNextGeneration(List<Organism> organisms) {
        List<Organism> survivors = new(organisms.Count / 2);

        double min_fit = 1, max_fit = -1;
        foreach (Organism organism in organisms) {
            double fitness = organism.Fitness;

            if (fitness > max_fit) {
                max_fit = fitness;
            } else if (fitness < min_fit) {
                min_fit = fitness;
            }
        }

        foreach (Organism organism in organisms) {
            double normalised_fitness = (organism.Fitness - min_fit) / (max_fit - min_fit);
            if (Random.value <= normalised_fitness) {
                survivors.Add(organism);
            }
        }

        if (survivors.Count % 2 != 0) {
            survivors.RemoveAt(survivors.Count - 1);
        }

        List<Organism> next_generation = new(organisms.Count);

        while (survivors.Count > 0 && next_generation.Count < 1000) {
            Organism a = PopRandom(survivors);
            Organism b = PopRandom(survivors);

            a.DoAgeing();
            b.DoAgeing();

            next_generation.Add(a);
            next_generation.Add(b);

            int offspring = Mathf.CeilToInt((float)((a.Fitness + b.Fitness) * (maxOffspring - minOffspring) / 2) + minOffspring);

            for (int i = 0; i < offspring; i++) {
                Organism c = Reproduce(a, b);
                next_generation.Add(c);
            }
        }

        return next_generation;
    }

    private T PopRandom<T>(List<T> list) {
        T item = list[Random.Range(0, list.Count)];
        list.Remove(item);
        return item;
    }

    private Organism Reproduce(Organism a, Organism b) {
        double[] attribute_values = new double[Organism.attributes.Length];

        for (int i = 0; i < Organism.attributes.Length; i++) {
            double average = (a.AttributeValues[i] + b.AttributeValues[i]) / 2;
            attribute_values[i] = average + RandomNormal.Random(mutationStrength);
        }

        return new Organism(attribute_values);
    }
}
