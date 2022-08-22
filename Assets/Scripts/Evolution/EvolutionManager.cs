using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

public class EvolutionManager : MonoBehaviour {

    public EvolutionConfig config;
    [SerializeField] private SimulationState state;

    [Header("Simulation Config")] // Evolution config deals with the actual evolution paramaters. Simulation config only affects how this is shown.
    public GameObject organismObject;
    public int gensPerStep = 10;
    public int updateCSV = 10;
    public float genLength = 5;
    
    private readonly List<GameObject> organism_objects = new();

    private float last_generation;

    private void Awake() {
        state = (SimulationState)ScriptableObject.CreateInstance(typeof(SimulationState)); // Could require SO that is assumed zeroed instead

        InitialisePopulation();
        last_generation = -genLength;
    }

    private void Update() {
        if (Time.time - last_generation >= genLength) {
            GenerateGameObjects();
            
            for (int i = 0; i < gensPerStep; i++) {
                state.organisms = GetNextGeneration(state.organisms);
                state.generation++;
            }

            last_generation = Time.time;
        }
    }

    private void InitialisePopulation() {
        state.organisms = new List<Organism>(config.initialPopulationSize);
        for (int i = 0; i < config.initialPopulationSize; i++) {
            state.organisms.Add(new());
        }
    }

    private void GenerateGameObjects() {
        for (int i = 0; i < organism_objects.Count; i++) {
            if (i < state.organisms.Count) {
                organism_objects[i].SetActive(true);
                organism_objects[i].GetComponent<OrganismController>().Initialise(state.organisms[i]);
            } else {
                organism_objects[i].SetActive(false);
            }
        }

        for (int i = organism_objects.Count; i < state.organisms.Count; i++) {
            GameObject new_org = Instantiate(organismObject, gameObject.transform);
            new_org.GetComponent<OrganismController>().Initialise(state.organisms[i]);
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

            int offspring = Mathf.CeilToInt((float)((a.Fitness + b.Fitness) * (config.maximumOffspring - config.minimumOffspring) / 2) + config.minimumOffspring);

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
            attribute_values[i] = average + RandomNormal.Random(config.mutationStrength);
        }

        return new Organism(attribute_values);
    }
}
