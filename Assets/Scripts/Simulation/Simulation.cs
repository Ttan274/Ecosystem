using System.Collections.Generic;
using UnityEngine;

public class Simulation : MonoBehaviour
{
    [Header("Simulation Data")]
    [SerializeField] private float interval;
    private float timer = 0f;

    [Header("Holders")]
    private HashSet<int> addedAnimals = new HashSet<int>();
    public List<SimulationData> history { get; private set; }
    public static Simulation Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        history = new();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= interval)
            RecordData();
    }

    #region Data region
    private void RecordData()
    {
        timer = 0f;

        var data = new SimulationData
        {
            time = Time.time,
            droughtTimer = WorldManager.Instance.droughtTimer,
            diseaseApplied = WorldManager.Instance.diseaseApplied,
            herbivoreCount = WorldManager.Instance.Count(SpeciesType.Herbivore),
            carnivoreCount = WorldManager.Instance.Count(SpeciesType.Carnivore),
        };

        history.Add(data);
    }

    public List<AnimalStats> GatherAnimalData()
    {
        var animalHistory = new List<AnimalStats>();

        foreach (var animal in WorldManager.Instance.Animals)
        {
            if (addedAnimals.Contains(animal.Id)) continue;

            AnimalStats stat = new()
            {
                name = animal.animalName,
                gender = animal.gender.ToString(),
                deadType = animal.deathBehaviour.deathType.ToString(),
                age = animal.age,
                eatenObjectCount = animal.eatenObjectCount,
                childCount = animal.childCount,
                type = animal.Species.ToString()
            };

            addedAnimals.Add(animal.Id);
            animalHistory.Add(stat);
        }

        return animalHistory;
    }

    /*Right now not necessary
    private void ExportToCSV(string fileName = "SimStats.csv")
    {
        StringBuilder csv = new StringBuilder();
        csv.AppendLine("Time,DroughtTimer,DiseaseApplied,Herbivores,HerbivoreEaten,HerbivoreBorn,Carnivores,CarnivoreBorn,PlantsEaten,PlantsRegrow");

        foreach (var point in history)
            csv.AppendLine($"{point.time:F2},{point.droughtTimer:F2},{point.diseaseApplied},{point.herbivores},{point.herbivoreEaten},{point.herbivoreEaten},{point.carnivores},{point.carnivoreBorn},{point.plantsEaten},{point.plantsRegrow}");

        string path = Path.Combine(Application.dataPath, fileName);
        File.WriteAllText(path, csv.ToString());
        Debug.Log($"Simulation data exported to {path}");
    }

    public void ExportToJSON(string fileName = "SimStats.json")
    {
        string json = JsonUtility.ToJson(new Wrapper { data = history}, true);
        string path = Path.Combine(Application.persistentDataPath, fileName);
        File.WriteAllText(path, json);
        Debug.Log($"Simulation data exported to {path}");
    }*/
    #endregion
}
