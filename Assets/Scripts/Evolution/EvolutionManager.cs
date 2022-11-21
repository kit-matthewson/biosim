using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

/// <summary>
/// Runs the evolution simulation.
/// </summary>
public class EvolutionManager : MonoBehaviour {

    public MenuController menuController;

    public SimulationState State { get; private set; }

    [Header("UI")]
    public Slider GenProgress;
    public TextMeshProUGUI GenText;

    // Evolution config deals with the actual evolution paramaters. Simulation config only affects how this is shown.
    [Header("Simulation Config")]
    public GameObject OrganismObject;
    public int GensPerStep = 10;
    public float GenLength = 5;
    public int GenQueueLength = 10;

    private EvolutionConfig _config;

    private readonly List<GameObject> _organism_objects = new();

    private float _last_generation;

    private readonly Queue<List<Organism>> _generations = new();
    private Thread _generationThread;

    private readonly Random rnd = new();

    // Use Awake so CSVs can be made in Start
    private void Awake() {
        State = ScriptableObject.CreateInstance<SimulationState>();
    }

    private void Start() {
        _config = MenuController.StaticController.evolutionConfig;

        InitialisePopulation();
        _last_generation = -GenLength;

        _generations.Enqueue(State.current_gen);
        _generationThread = new Thread(DoGenerations);
        _generationThread.Start();
    }

    private void Update() {
        if (Time.time - _last_generation >= GenLength) {
            GenText.text = State.generation.ToString();

            for (int i = 0; i < GensPerStep && _generations.Count() > 0; i++) {
                State.current_gen = _generations.Dequeue();
                State.generation++;
            }

            GenerateGameObjects();

            _last_generation = Time.time;
        }

        GenProgress.value = (Time.time - _last_generation) / GenLength;
    }

    /// <summary>
    /// Initialises the population based on the  <c>EvolutionConfig</c>.
    /// </summary>
    private void InitialisePopulation() {
        State.current_gen = new List<Organism>(_config.initialPopulationSize);
        for (int i = 0; i < _config.initialPopulationSize; i++) {
            State.current_gen.Add(new());
        }
    }

    /// <summary>
    /// Runs generations whenever the generation queue drops below <c>GenQueueLength</c>
    /// </summary>
    private void DoGenerations() {
        while (true) {
            if (_generations.Count <= GenQueueLength && _generations.Count > 0) {
                _generations.Enqueue(GetNextGeneration(_generations.Last()));
            }
        }
    }

    /// <summary>
    /// Efficiently creates gameobjects corresponding to the current generation. Deactivates unused objects.
    /// </summary>
    private void GenerateGameObjects() {
        for (int i = 0; i < _organism_objects.Count; i++) {
            if (i < State.current_gen.Count) {
                _organism_objects[i].SetActive(true);
                _organism_objects[i].GetComponent<OrganismController>().Initialise(State.current_gen[i]);
            } else {
                _organism_objects[i].SetActive(false);
            }
        }

        for (int i = _organism_objects.Count; i < State.current_gen.Count; i++) {
            GameObject new_org = Instantiate(OrganismObject, gameObject.transform);
            new_org.GetComponent<OrganismController>().Initialise(State.current_gen[i]);
            _organism_objects.Add(new_org);
        }
    }

    /// <summary>
    /// Runs the generation algorithms on <c>organisms</c>.
    /// </summary>
    /// <param name="organisms">Previous generation</param>
    /// <returns>Next generation</returns>
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

            int offspring = Mathf.CeilToInt((float)((a.Fitness + b.Fitness) * (_config.maximumOffspring - _config.minimumOffspring) / 2) + _config.minimumOffspring);

            for (int i = 0; i < offspring; i++) {
                Organism c = Reproduce(a, b);
                next_generation.Add(c);
            }
        }

        return next_generation;
    }
    
    /// <summary>
    /// Pops a random item from a list.
    /// </summary>
    private T PopRandom<T>(List<T> list) {
        T item = list[rnd.Next(0, list.Count)];
        list.Remove(item);
        return item;
    }

    /// <summary>
    /// Calculates the offspring of two organisms.
    /// </summary>
    /// <param name="a">First organism</param>
    /// <param name="b">Second organism</param>
    /// <returns>Offspring</returns>
    private Organism Reproduce(Organism a, Organism b) {
        double[] attribute_values = new double[Organism.attributes.Length];

        for (int i = 0; i < Organism.attributes.Length; i++) {
            double average = (a.AttributeValues[i] + b.AttributeValues[i]) / 2;
            attribute_values[i] = Mathf.Clamp((float)(average + RandomNormal.Random(_config.mutationStrength)), -1, 1);
        }

        return new Organism(attribute_values);
    }
}
