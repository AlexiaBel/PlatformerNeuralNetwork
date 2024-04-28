using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvolutionController : MonoBehaviour
{
    [Range(0, 5)]
    public float simulationSpeed = 1;
    public GameObject p_Agent;
    public int[] neuralNetworkShape = { 16, 32, 32, 2 };
    public int agentsAmount;
    public int agentsAllowedToReproduceAmount;
    [Range(0, 1)]
    public float mutationChance;
    [Range(0, 1)]
    public float mutationAmount;
    public Vector2 spawnLocation;
    public float timeAllowedToComplete;
    public static Transform flag;
    public GameObject[] maps;
    public enum SimulationVisualization
    {
        All,
        AllHighlightBest,
        AllHighlightWorst,
        AllHighlightBoth,
        OnlyBest,
        OnlyWorst,
        OnlyBoth
    }
    public SimulationVisualization simVisualization;
    public float highlightUpdateTime;

    List<GameObject> agents = new List<GameObject>();

    void Start()
    {
        LoadRandomMap();

        for (int i = 0; i < agentsAmount; i++)
        {
            GameObject newAgent = Instantiate(p_Agent, spawnLocation, Quaternion.identity);

            var newBrain = new NeuralNetwork.NeuralNetwork(neuralNetworkShape);

            newBrain.MutateNetwork(1, mutationAmount);

            newAgent.GetComponent<AiBrain>().SetBrain(newBrain);

            Debug.Log(newAgent.GetComponent<AiBrain>().Brain);

            agents.Add(newAgent);
        }

        StartCoroutine(StartGeneration());
        StartCoroutine(UpdateVisualization());
    }

    private void Update()
    {
        Time.timeScale = simulationSpeed;
    }

    bool allActive;
    void ShowAll()
    {
        allActive = true;
        for(int i = 0; i < agents.Count; i++)
        {
            if (agents[i] == null) { continue; }

            agents[i].GetComponent<SpriteRenderer>().color = Color.white;
            agents[i].GetComponent<SpriteRenderer>().sortingOrder = 0;

            agents[i].GetComponent<SpriteRenderer>().enabled = true;
        }
    }

    void HideAll()
    {
        allActive = false;
        for (int i = 0; i < agents.Count; i++)
        {
            if (agents[i] == null) { continue; }

            agents[i].GetComponent<SpriteRenderer>().color = Color.white;
            agents[i].GetComponent<SpriteRenderer>().sortingOrder = 0;

            agents[i].GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    void HighlightBest()
    {
        float currentBest = 99999;
        int currentBestIndex = 0;

        for (int i = 0; i < agents.Count; i++)
        {
            if (agents[i] == null) { continue; }

            if (agents[i].GetComponent<Player>().dead) { continue; }

            float dist = Vector2.Distance(agents[i].transform.position, flag.position);
            if (dist < currentBest)
            {
                currentBest = dist;
                currentBestIndex = i;
            }
        }

        agents[currentBestIndex].GetComponent<SpriteRenderer>().color = Color.blue;
        agents[currentBestIndex].GetComponent<SpriteRenderer>().enabled = true;
        agents[currentBestIndex].GetComponent<SpriteRenderer>().sortingOrder = 1;
    }

    void HighlightWorst()
    {
        float currentBest = 0;
        int currentBestIndex = 0;

        for (int i = 0; i < agents.Count; i++)
        {
            if (agents[i] == null) { continue; }

            float dist = Vector2.Distance(agents[i].transform.position, flag.position);
            if (dist > currentBest)
            {
                currentBest = dist;
                currentBestIndex = i;
            }
        }

        agents[currentBestIndex].GetComponent<SpriteRenderer>().color = Color.red;
        agents[currentBestIndex].GetComponent<SpriteRenderer>().enabled = true;
        agents[currentBestIndex].GetComponent<SpriteRenderer>().sortingOrder = 1;
    }

    void LoadRandomMap()
    {
        foreach(GameObject x in maps)
        {
            x.SetActive(false);
        }

        var loadedmap = maps[Random.Range(0, maps.Length)];

        loadedmap.SetActive(true);
        spawnLocation = loadedmap.GetComponent<map>().spawnPos;
        flag = loadedmap.GetComponent<map>().goal;
    }

    IEnumerator UpdateVisualization()
    {
        while (true)
        {
            switch (simVisualization)
            {
                case SimulationVisualization.All:
                    ShowAll();
                    break;
                case SimulationVisualization.AllHighlightBest:
                    ShowAll();
                    HighlightBest();
                    break;
                case SimulationVisualization.AllHighlightWorst:
                    ShowAll();
                    HighlightWorst();
                    break;
                case SimulationVisualization.AllHighlightBoth:
                    ShowAll();
                    HighlightBest();
                    HighlightWorst();
                    break;
                case SimulationVisualization.OnlyBest:
                    HideAll();
                    HighlightBest();
                    break;
                case SimulationVisualization.OnlyWorst:
                    HideAll();
                    HighlightWorst();
                    break;
                case SimulationVisualization.OnlyBoth:
                    HideAll();
                    HighlightBest();
                    HighlightWorst();
                    break;
            }

            yield return new WaitForSeconds(highlightUpdateTime);
        }
    }

    IEnumerator StartGeneration()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeAllowedToComplete);

            GameObject[] allowedToReproduce = new GameObject[agentsAllowedToReproduceAmount];

            for (int a = agents.Count - 1; a >= 0; a--)
            {
                GameObject x = agents[a];

                Player agentPlayer = x.GetComponent<Player>();
                AiBrain agentBrain = x.GetComponent<AiBrain>();

                for (int i = 0; i < allowedToReproduce.Length; i++)
                {
                    if (allowedToReproduce[i] == null) { allowedToReproduce[i] = x; break; }

                    if (agentPlayer.dead) { continue; }

                    if (Vector2.Distance(x.transform.position, flag.position) < Vector2.Distance(allowedToReproduce[i].transform.position, flag.position))
                    {
                        allowedToReproduce[i] = x;
                        break;
                    }
                }
            }

            List<GameObject> childAgent = new List<GameObject>();

            int y = 0;

            LoadRandomMap();

            for (int i = 0; i < agentsAmount; i++)
            {
                if (y >= allowedToReproduce.Length) { y = 0; }

                if (allowedToReproduce[y] == null) { y++; continue; }

                GameObject newAgent = Instantiate(p_Agent, spawnLocation, Quaternion.identity);

                NeuralNetwork.NeuralNetwork newBrain = new NeuralNetwork.NeuralNetwork(neuralNetworkShape);

                newBrain.layers = allowedToReproduce[y].GetComponent<AiBrain>().Brain.CopyLayers();

                newBrain.MutateNetwork(mutationChance, mutationAmount);

                newAgent.GetComponent<AiBrain>().SetBrain(newBrain);

                childAgent.Add(newAgent);

                y++;
            }

            for (int a = agents.Count - 1; a >= 0; a--)
            {
                Destroy(agents[a]);
            }

            agents.Clear();

            agents.AddRange(childAgent);
        }
    }
}
