using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class EvolutionManager : MonoBehaviour {

    public MenuController menuController;

    public SimulationState State {
        get {
            return state;
        }
    }

    [Header("UI")]
    public Slider genProgress;
    public TextMeshProUGUI genText;

    [Header("Simulation Config")] // Evolution config deals with the actual evolution paramaters. Simulation config only affects how this is shown.
    public GameObject organismObject;
    public int gensPerStep = 10;
    public float genLength = 5;
    public int genQueueLength = 10;

    private EvolutionConfig config;
    private SimulationState state;

    private readonly List<GameObject> organism_objects = new();

    private float last_generation;

    private readonly Queue<List<Organism>> generations = new();
    private Thread generationThread;

    private readonly Random rnd = new();

    // Use Awake so CSVs can be made in Start
    private void Awake() {
        state = ScriptableObject.CreateInstance<SimulationState>();
    }

    private void Start() {
        config = menuController.StaticController.evolutionConfig;

        InitialisePopulation();
        last_generation = -genLength;

        generations.Enqueue(state.current_gen);
        generationThread = new Thread(DoGenerations);
        generationThread.Start();
    }

    private void Update() {
        if (Time.time - last_generation >= genLength) {
            genText.text = state.generation.ToString();

            for (int i = 0; i < gensPerStep && generations.Count() > 0; i++) {
                state.current_gen = generations.Dequeue();
                state.generation++;
            }

            GenerateGameObjects();

            last_generation = Time.time;
        }

        genProgress.value = (Time.time - last_generation) / genLength;
    }

    private void InitialisePopulation() {
        state.current_gen = new List<Organism>(config.initialPopulationSize);
        for (int i = 0; i < config.initialPopulationSize; i++) {
            state.current_gen.Add(new());
        }
    }

    private void DoGenerations() {
        while (true) {
            if (generations.Count <= genQueueLength && generations.Count > 0) {
                generations.Enqueue(GetNextGeneration(generations.Last()));
            }
        }
    }

    private void GenerateGameObjects() {
        for (int i = 0; i < organism_objects.Count; i++) {
            if (i < state.current_gen.Count) {
                organism_objects[i].SetActive(true);
                organism_objects[i].GetComponent<OrganismController>().Initialise(state.current_gen[i]);
            } else {
                organism_objects[i].SetActive(false);
            }
        }

        for (int i = organism_objects.Count; i < state.current_gen.Count; i++) {
            GameObject new_org = Instantiate(organismObject, gameObject.transform);
            new_org.GetComponent<OrganismController>().Initialise(state.current_gen[i]);
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
            if (rnd.NextDouble() <= normalised_fitness) {
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
        T item = list[rnd.Next(0, list.Count)];
        list.Remove(item);
        return item;
    }

    private Organism Reproduce(Organism a, Organism b) {
        double[] attribute_values = new double[Organism.attributes.Length];

        for (int i = 0; i < Organism.attributes.Length; i++) {
            double average = (a.AttributeValues[i] + b.AttributeValues[i]) / 2;
            attribute_values[i] = Mathf.Clamp((float)(average + RandomNormal.Random(config.mutationStrength)), -1, 1);
        }

        return new Organism(attribute_values);
    }
}
