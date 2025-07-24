using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class SimulationStats : MonoBehaviour
{
    [Header("SimData")]
    [SerializeField] private float interval = 1f; // Time in seconds between each data capture
    private float timer = 0f;
    public List<SimulationData> history = new List<SimulationData>();

    public static SimulationStats Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            timer = 0f;
            CaptureData();
        }

        if (Input.GetKeyDown(KeyCode.K))
            ExportToCSV();
    }

    private void CaptureData()
    {
        var data = Simulation.Instance.GetRecord();
        history.Add(data);
    }

    public void ExportToCSV(string fileName = "SimStats.csv")
    {
        StringBuilder csv = new StringBuilder();
        csv.AppendLine("Time,DroughtTimer,DiseaseApplied,Herbivores,HerbivoreEaten,HerbivoreBorn,Carnivores,CarnivoreBorn,PlantsEaten,PlantsRegrow");

        foreach (var point in history)
        {
            csv.AppendLine($"{point.time:F2},{point.droughtTimer:F2},{point.diseaseApplied},{point.herbivores},{point.herbivoreEaten},{point.herbivoreEaten},{point.carnivores},{point.carnivoreBorn},{point.plantsEaten},{point.plantsRegrow}");
        }

        string path = Path.Combine(Application.dataPath, fileName);
        File.WriteAllText(path, csv.ToString());
        Debug.Log($"Simulation data exported to {path}");
    }
}

[System.Serializable]
public class SimulationData
{
    //Simulation Related Data
    public float time;
    public float droughtTimer;
    public int diseaseApplied;

    //Entity Related Data
    public int herbivores;
    public int herbivoreEaten;
    public int herbivoreBorn;

    public int carnivores;
    public int carnivoreBorn;

    public int plantsEaten;
    public int plantsRegrow;
}