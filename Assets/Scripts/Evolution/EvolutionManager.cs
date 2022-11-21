using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

/// <summary>
///     Runs the evolution simulation.
/// </summary>
public class EvolutionManager : MonoBehaviour
{
    public StaticMenuControllerHandle MenuControllerHandle;

    public SimulationState State { get; private set; }

    [Header("UI")]
    public Slider GenProgress;

    public TextMeshProUGUI GenText;

    // Evolution config deals with the actual evolution parameters. Simulation config only affects how this is shown.
    [Header("Simulation Config")]
    public GameObject OrganismObject;

    public int GensPerStep = 10;
    public float GenLength = 5;
    public int GenQueueLength = 10;

    private EvolutionConfig _config;

    private readonly List<GameObject> _organismObjects = new();

    private float _lastGeneration;

    private readonly Queue<List<Organism>> _generations = new();
    private Thread _generationThread;

    private readonly Random _rnd = new();

    // Use Awake so CSVs can be made in Start
    [PublicAPI]
    private void Awake() {
        State = ScriptableObject.CreateInstance<SimulationState>();
    }

    [PublicAPI]
    private void Start() {
        _config = MenuControllerHandle.StaticController.EvolutionConfig;

        InitialisePopulation();
        _lastGeneration = -GenLength;

        _generations.Enqueue(State.CurrentGen);
        _generationThread = new Thread(DoGenerations);
        _generationThread.Start();
    }

    [PublicAPI]
    private void Update() {
        if (Time.time - _lastGeneration >= GenLength) {
            GenText.text = State.Generation.ToString();

            for (int i = 0; i < GensPerStep && _generations.Any(); i++) {
                State.CurrentGen = _generations.Dequeue();
                State.Generation++;
            }

            GenerateGameObjects();

            _lastGeneration = Time.time;
        }

        GenProgress.value = (Time.time - _lastGeneration) / GenLength;
    }

    /// <summary>
    ///     Initialises the population based on the <c>EvolutionConfig</c>.
    /// </summary>
    private void InitialisePopulation() {
        State.CurrentGen = new List<Organism>(_config.InitialPopulationSize);

        for (int i = 0; i < _config.InitialPopulationSize; i++) {
            State.CurrentGen.Add(new Organism());
        }
    }

    // ReSharper disable once FunctionNeverReturns
    /// <summary>
    ///     Runs generations whenever the generation queue drops below <c>GenQueueLength</c>
    /// </summary>
    private void DoGenerations() {
        while (true) {
            if (_generations.Count <= GenQueueLength && _generations.Count > 0) {
                _generations.Enqueue(GetNextGeneration(_generations.Last()));
            }
        }
    }

    /// <summary>
    ///     Efficiently creates GameObjects corresponding to the current generation. Deactivates unused objects.
    /// </summary>
    private void GenerateGameObjects() {
        for (int i = 0; i < _organismObjects.Count; i++) {
            if (i < State.CurrentGen.Count) {
                _organismObjects[i].SetActive(true);
                _organismObjects[i].GetComponent<OrganismController>().Initialise(State.CurrentGen[i]);
            } else {
                _organismObjects[i].SetActive(false);
            }
        }

        for (int i = _organismObjects.Count; i < State.CurrentGen.Count; i++) {
            GameObject newOrg = Instantiate(OrganismObject, gameObject.transform);
            newOrg.GetComponent<OrganismController>().Initialise(State.CurrentGen[i]);
            _organismObjects.Add(newOrg);
        }
    }

    /// <summary>
    ///     Runs the generation algorithms on <c>organisms</c>.
    /// </summary>
    /// <param name="organisms">Previous generation</param>
    /// <returns>Next generation</returns>
    private List<Organism> GetNextGeneration(List<Organism> organisms) {
        List<Organism> survivors = new(organisms.Count / 2);

        double minFit = 1, maxFit = -1;

        foreach (double fitness in organisms.Select(organism => organism.Fitness)) {
            if (fitness > maxFit) {
                maxFit = fitness;
            } else if (fitness < minFit) {
                minFit = fitness;
            }
        }
        
        foreach (Organism organism in organisms) {
            double normalisedFitness = (organism.Fitness - minFit) / (maxFit - minFit);

            if (_rnd.NextDouble() <= normalisedFitness) {
                survivors.Add(organism);
            }
        }

        if (survivors.Count % 2 != 0) {
            survivors.RemoveAt(survivors.Count - 1);
        }

        List<Organism> nextGeneration = new(organisms.Count);

        while (survivors.Count > 0 && nextGeneration.Count < 1000) {
            Organism a = PopRandom(survivors);
            Organism b = PopRandom(survivors);

            a.DoAgeing();
            b.DoAgeing();

            nextGeneration.Add(a);
            nextGeneration.Add(b);

            int offspring = Mathf.CeilToInt((float)((a.Fitness + b.Fitness) * (_config.MaximumOffspring - _config.MinimumOffspring) / 2) +
                                            _config.MinimumOffspring);

            for (int i = 0; i < offspring; i++) {
                Organism c = Reproduce(a, b);
                nextGeneration.Add(c);
            }
        }

        return nextGeneration;
    }

    /// <summary>
    ///     Pops a random item from a list.
    /// </summary>
    private T PopRandom<T>(List<T> list) {
        T item = list[_rnd.Next(0, list.Count)];
        list.Remove(item);

        return item;
    }

    /// <summary>
    ///     Calculates the offspring of two organisms.
    /// </summary>
    /// <param name="a">First organism</param>
    /// <param name="b">Second organism</param>
    /// <returns>Offspring</returns>
    private Organism Reproduce(Organism a, Organism b) {
        double[] attributeValues = new double[Organism.Attributes.Length];

        for (int i = 0; i < Organism.Attributes.Length; i++) {
            double average = (a.AttributeValues[i] + b.AttributeValues[i]) / 2;
            attributeValues[i] = Mathf.Clamp((float)(average + RandomNormal.Random(_config.MutationStrength)), -1, 1);
        }

        return new Organism(attributeValues);
    }
}