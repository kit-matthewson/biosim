using System;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(EvolutionManager))]
public class UiController : MonoBehaviour {
    private EvolutionManager _evolutionManager;

    public CsvMaker CsvMaker;

    public GameObject NonGraphUi;
    public GameObject GraphUi;

    public Transform GraphParent;
    public GameObject GraphPoint;
    public GameObject GraphTick;
    public Transform GraphMin;
    public Transform GraphMax;

    public TMP_Dropdown GraphTypeDropdown;
    public TMP_Dropdown GraphTimeDropdown;

    private bool _graphsShown;

    [PublicAPI]
    private void Start() {
        _evolutionManager = GetComponent<EvolutionManager>();

        foreach (Attribute attribute in Organism.Attributes) {
            GraphTypeDropdown.options.Add(new TMP_Dropdown.OptionData($"{attribute.Name}: Average"));
            GraphTypeDropdown.options.Add(new TMP_Dropdown.OptionData($"{attribute.Name}: Distribution"));
        }
    }
    
    [PublicAPI]
    public void SkipGenerations(int n) {
        _evolutionManager.GenerationGoal = _evolutionManager.State.Generation + n;
    }

    [PublicAPI]
    public void PlayPause() {
        _evolutionManager.State.Paused = !_evolutionManager.State.Paused;
    }

    [PublicAPI]
    public void Graph() {
        _graphsShown = !_graphsShown;

        NonGraphUi.SetActive(!_graphsShown);
        GraphUi.SetActive(_graphsShown);

        DrawGraph();
    }

    /// <summary>
    /// Generates a graph from the selected dropdowns.
    /// </summary>
    public void DrawGraph() {
        string typeDropdown = GraphTypeDropdown.options[GraphTypeDropdown.value].text;
        string timeDropdown = GraphTimeDropdown.options[GraphTimeDropdown.value].text;

        foreach (Transform child in GraphParent) {
            Destroy(child.gameObject);
        }

        var files = CsvMaker.Files;

        FileHandler file;
        GraphType type = GraphType.Average;

        switch (typeDropdown) {
            case "Total Population":
                type = GraphType.Sum;
                file = files["Fitness"];
                break;
            case "Change in Population":
                return;
            default:
                file = typeDropdown == "Fitness" ? files["Fitness"] : files[typeDropdown.Split(":")[0]];

                if (typeDropdown != "Fitness" && typeDropdown.Split(": ")[1] == "Distribution") {
                    type = GraphType.Distribution;
                }

                break;
        }

        string[] contents = file.ReadLines();

        int time = timeDropdown switch {
            "Last 10 Generations" => Mathf.Min(contents.Length - 2, 10),
            "Last 50 Generations" => Mathf.Min(contents.Length - 2, 50),
            _ => contents.Length - 2
        };

        if (type == GraphType.Distribution) {
            Histogram(contents);
        } else {
            LineGraph(contents, time, type);
        }
    }

    /// <summary>
    /// Draws a line graph of the sum or average value from the contents file.
    /// </summary>
    /// <param name="contents">Lines of csv to graph.</param>
    /// <param name="time">Will graph the last <c>time</c> rows of <c>contents</c>.</param>
    /// <param name="type">Statistic to graph (sum or average).</param>
    private void LineGraph(string[] contents, int time, GraphType type) {
        float graphHeight = GraphMax.position.y - GraphMin.position.y;

        const int maxYTick = 10;
        float ySpacing = graphHeight / (maxYTick * 2 + 1);

        for (int i = -10; i <= 10; i++) {
            float y = i * ySpacing + (graphHeight / 2 + GraphMin.position.y);
            GameObject tick = Instantiate(GraphTick, new Vector3(GraphMin.position.x, y, 0), Quaternion.identity, GraphParent);

            tick.transform.localScale = MathF.Abs(i) % 10 == 0 ? new Vector3(15, 2, 1) : new Vector3(10, 2, 1);
        }

        string[] header = contents[0].Split(",");
        for (int i = contents.Length - time - 1; i < contents.Length - 1; i++) {
            string[] row = contents[i].Split(",");

            float sum = 0;
            int n = 0;

            for (int j = 1; j < row.Length; j++) {
                try {
                    sum += float.Parse(header[j]) * float.Parse(row[j]);
                    n += int.Parse(row[j]);
                } catch (FormatException e) {
                    Console.WriteLine(e);
                }
            }

            float value = type == GraphType.Average ? sum / n : n;
            float min = type == GraphType.Average ? -1 : 0;
            float max = type == GraphType.Average ? 1 : 1_500;

            float x = Mathf.Lerp(GraphMin.position.x, GraphMax.position.x, (i - contents.Length + time + 1) / (float)time);
            float y = Mathf.Lerp(GraphMin.position.y, GraphMax.position.y, Map(value, min, max));

            Instantiate(GraphPoint, new Vector3(x, y, 0), Quaternion.identity, GraphParent);
            GameObject tick = Instantiate(GraphTick, new Vector3(x, GraphMin.position.y, 0), Quaternion.identity, GraphParent);
            tick.transform.localScale = (i - 1) % 10 == 0 ? new Vector3(2, 15, 1) : new Vector3(2, 10, 1);

        }
    }

    private void Histogram(string[] contents) { }

    /// <summary>
    /// Maps a value from then range [<c>min</c>:<c>max</c>] to [0:1]
    /// </summary>
    /// <param name="v">Value to map.</param>
    /// <param name="min">Minimum input value.</param>
    /// <param name="max">Maximum input value.</param>
    private static float Map(float v, float min, float max) {
        return (v - min) / (max - min);
    }
}

internal enum GraphType {
    Average,
    Sum,
    Distribution,
}