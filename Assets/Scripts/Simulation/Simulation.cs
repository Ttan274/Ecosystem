using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Simulation : MonoBehaviour
{
    [Header("Simulation Data")]
    [SerializeField] private float interval;
    private float timer = 0f;
    private int day = 0;

    [Header("Holders")]
    private HashSet<int> addedAnimals = new HashSet<int>();
    public List<SimulationData> history { get; private set; }
    public static Simulation Instance;

    //FileNames
    public const string simDataFileName = "SimStats.json";
    public const string simAnimalDataFileName = "SimAnimalStats.json";

    #region Enable/Disable
    private void OnEnable()
    {
        WorldEvents.OnDayChanged += DayEnded;
    }


    private void OnDisable()
    {
        WorldEvents.OnDayChanged -= DayEnded;
    }

    #endregion

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

    private void DayEnded(int day)
    {
        //RecordData();
        this.day = day;
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
            dayCount = day,
            totalHerbivoreCount = WorldManager.Instance.AllAnimals.FindAll(a => a.Species == SpeciesType.Herbivore).Count,
            totalCarnivoreCount = WorldManager.Instance.AllAnimals.FindAll(a => a.Species == SpeciesType.Carnivore).Count
        };

        history.Add(data);
    }

    public List<AnimalStats> GatherAnimalData()
    {
        var animalHistory = new List<AnimalStats>();

        foreach (var animal in WorldManager.Instance.AllAnimals)
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
                type = animal.Species.ToString(),
                motherName = animal.parentData.motherName,
                fatherName = animal.parentData.fatherName
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
    }*/

    public void ExportToJSON()
    {
        //Export Simulation Data
        string jsonSimData = JsonUtility.ToJson(new Wrapper { data = history }, true);
        string pathSimData = Path.Combine(Application.dataPath, simDataFileName);

        //Export Animal Data
        string jsonSimAnimalData = JsonUtility.ToJson(new AnimalsWrapper { animalsStats = GatherAnimalData() }, true);
        string pathSimAnimalData = Path.Combine(Application.dataPath, simAnimalDataFileName);

        //Write Files
        File.WriteAllText(pathSimData, jsonSimData);
        File.WriteAllText(pathSimAnimalData, jsonSimAnimalData);
        //Debug.Log($"Simulation data exported to {path}");
    }
    #endregion
}
