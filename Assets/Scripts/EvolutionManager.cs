using System.Collections.Generic;
using UnityEngine;

public class EvolutionManager : MonoBehaviour {
    public GameObject organismObject;

    public int initialPopSize = 100;
    public int gensPerStep = 10;
    public float genLength = 5;

    public int minOffspring = 3;
    public int maxOffspring = 5;

    public double mutationStrength = 0.01;

    private List<Organism> organisms;
    private List<GameObject> organismObjects = new();

    private float last_generation;

    private void Awake() {
        InitialisePopulation();
        last_generation = -genLength;
    }

    private void Update() {
        if (Time.time - last_generation >= genLength) {
            GenerateGameObjects();
            
            for (int i = 0; i < gensPerStep; i++) {
                organisms = GetNextGeneration(organisms);
            }

            last_generation = Time.time;
        }
    }

    private void InitialisePopulation() {
        organisms = new List<Organism>(initialPopSize);
        for (int i = 0; i < initialPopSize; i++) {
            organisms.Add(new());
        }
    }

    private void GenerateGameObjects() {
        for (int i = 0; i < organismObjects.Count; i++) {
            if (i < organisms.Count) {
                organismObjects[i].SetActive(true);
                organismObjects[i].GetComponent<OrganismController>().Initialise(organisms[i]);
            } else {
                organismObjects[i].SetActive(false);
            }
        }

        for (int i = organismObjects.Count; i < organisms.Count; i++) {
            GameObject new_org = Instantiate(organismObject);
            new_org.GetComponent<OrganismController>().Initialise(organisms[i]);
            organismObjects.Add(new_org);
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
