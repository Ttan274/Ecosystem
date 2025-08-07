using System.Collections.Generic;
using UnityEngine;

public class Simulation : MonoBehaviour
{
    [Header("Simulation Data")]
    [SerializeField] private float interval;
    private float timer = 0f;

    //Admin Related Data
    [HideInInspector] public int diseaseApplied = 0;
    [HideInInspector] public float droughtTimer = 0f;
    public bool IsDroughtEnabled { get; private set; } = false;

    [Header("Holders")]
    public List<Carnivore> carnivores = new List<Carnivore>();
    public List<Herbivore> herbivores = new List<Herbivore>();
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
        if (IsDroughtEnabled)
            droughtTimer += Time.deltaTime;

        timer += Time.deltaTime;
        if (timer >= interval)
            RecordData();
    }

    #region Registeration Methods
    public void RegisterAnimal(Animal animal)
    {
        if (animal is Herbivore)
            herbivores.Add(animal as Herbivore);
        else
            carnivores.Add(animal as Carnivore);
    }

    public void RemoveAnimal(Animal animal)
    {
        if (animal is Herbivore)
            herbivores.Remove(animal as Herbivore);
        else
            carnivores.Remove(animal as Carnivore);
    }
    #endregion

    #region Admin Behaviours
    public void ApplyDisease(bool isHerbivore)
    {
        int max = isHerbivore ? herbivores.Count : carnivores.Count;
        if (max == 0)
            return;
        int rand = Random.Range(0, max);
            
        if(isHerbivore)
        {
            if (!herbivores[rand].isInfected)
                herbivores[rand].Infect();
        }
        else
        {
            if (!carnivores[rand].isInfected)
                carnivores[rand].Infect();
        }

        diseaseApplied++;
    }

    public void StartDrought(bool status) => IsDroughtEnabled = status;
    #endregion

    #region Data region
    private void RecordData()
    {
        timer = 0f;

        var data = new SimulationData
        {
            time = Time.time,
            droughtTimer = droughtTimer,
            diseaseApplied = diseaseApplied,
            herbivoreCount = herbivores.Count,
            carnivoreCount = carnivores.Count,
        };

        history.Add(data);
    }

    public List<AnimalStats> GatherAnimalData(List<Animal> allAnimals)
    {
        var animalHistory = new List<AnimalStats>();

        foreach (var animal in allAnimals)
        {
            if (addedAnimals.Contains(animal.Id)) continue;

            AnimalStats stat = new()
            {
                name = animal.animalName,
                gender = animal.gender.ToString(),
                deadType = animal.deathType.ToString(),
                age = 5,
                eatenObjectCount = animal.eatenObjectCount,
                childCount = animal.childCount,
                type = animal is Herbivore ? "Herbivore" : "Carnivore"
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
