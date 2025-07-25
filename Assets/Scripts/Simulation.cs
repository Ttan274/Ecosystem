using System.Collections.Generic;
using UnityEngine;

public class Simulation : MonoBehaviour
{
    [Header("Entity Details")]
    public GameObject herbivore;
    public GameObject carnivore;
    public bool IsDroughtEnabled = false;

    public List<Carnivore> carnivores = new List<Carnivore>();
    public List<Herbivore> herbivores = new List<Herbivore>();
    public int carnivoreCount => carnivores.Count;
    public int herbivoreCount => herbivores.Count;
    public int totalAnimals => carnivoreCount + herbivoreCount;
    
    [HideInInspector] public int plantsEaten = 0;
    [HideInInspector] public int plantsRegrow = 0;
    [HideInInspector] public int herbivoreEaten = 0;
    [HideInInspector] public int herbivoreBorn = 0;
    [HideInInspector] public int carnivoreBorn = 0;
    [HideInInspector] public int diseaseApplied = 0;
    [HideInInspector] public float droughtTimer = 0f;

    //References
    [SerializeField]private List<string> animalNames = new List<string>() { "John", "Lucia", "Maxvel", "Naber", "Hello", "Wikwik", "Susangus", "Naber" };
    private List<Tile> tiles = new List<Tile>();
    public static Simulation Instance;

    public void Initialize(List<Tile> tiles) => this.tiles = tiles;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        if (IsDroughtEnabled)
            droughtTimer += Time.deltaTime;
    }

    #region Generation Methods
    public void GenerateInitialAnimals(GameObject prefab, int count, bool t)
    {
        for (int i = 0; i < count; i++)
        {
            Tile place = tiles[Random.Range(0, tiles.Count)];
            tiles.Remove(place);
            Gender gender = (i % 2 == 0) ? Gender.Male : Gender.Female;
            GenerateAnimal(prefab, place.transform.position, t, gender);
        }
    }

    public void GenerateAnimal(GameObject animalPrefab, Vector3 spawnPos, bool t, Gender gen = Gender.Unknown)
    {
        Vector3 pos = spawnPos + new Vector3(0, 0.5f, 0);
        Animal g = Instantiate(animalPrefab, pos, Quaternion.identity, transform).GetComponent<Animal>();
        g.Initialize(GetNameForAnimal(), GenGenderForAnimal(gen));

        if (t)
            herbivores.Add(g as Herbivore);
        else
            carnivores.Add(g as Carnivore);
    }
    #endregion

    #region Utility Methods
    private Gender GenGenderForAnimal(Gender gen)
    {
        Gender gender;
        if (gen != Gender.Unknown)
            gender = gen;
        else
            gender = (Random.value < 0.5f) ? Gender.Male : Gender.Female;

        return gender;
    }

    private string GetNameForAnimal()
    {
        int rand = Random.Range(0, animalNames.Count);
        string n = animalNames[rand] + "-" + Random.Range(0, 99);
        animalNames.Remove(n);
        return n;
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
    public void ApplyDisease(int count)
    {
        if (herbivores.Count == 0)
            return;

        if (count > totalAnimals)
            count = totalAnimals;

        for (int i = 0; i < count; i++)
        {
            int rand = Random.Range(0, herbivores.Count);
            if (!herbivores[rand].isInfected)
                herbivores[rand].Infect();
        }
    }

    public void StartDrought(bool status) => IsDroughtEnabled = status;
    #endregion

    public SimulationData GetRecord()
    {
        var data = new SimulationData
        {
            time = Time.time,
            droughtTimer = droughtTimer,
            diseaseApplied = diseaseApplied,

            herbivores = herbivoreCount,
            herbivoreEaten = herbivoreEaten,
            herbivoreBorn = herbivoreBorn,

            carnivores = carnivoreCount,
            carnivoreBorn = carnivoreBorn,

            plantsEaten = plantsEaten,
            plantsRegrow = plantsRegrow
        };

        return data;
    }
}
