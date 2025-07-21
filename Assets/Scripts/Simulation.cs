using System.Collections.Generic;
using UnityEngine;

public class Simulation : MonoBehaviour
{
    [Header("Entity Details")]
    public GameObject herbivore;
    [SerializeField] private int initialHerbivoreCount;
    public GameObject carnivore;
    [SerializeField] private int initialCarnivoreCount;

    public List<Carnivore> carnivores = new List<Carnivore>();
    public List<Herbivore> herbivores = new List<Herbivore>();
    private bool herbivoresCreated = false;
    private bool carnivoresCreated = false;

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
        if(Input.GetKeyDown(KeyCode.P))
        {
            if(!herbivoresCreated)
            {
                GenerateInitialAnimals(herbivore, initialHerbivoreCount, true);
                herbivoresCreated = true;
                return;
            }

            if(herbivoresCreated && !carnivoresCreated)
            {
                GenerateInitialAnimals(carnivore, initialCarnivoreCount, false);
                carnivoresCreated = true;
                return;
            }
        }
    }

    //Generation Methods
    private void GenerateInitialAnimals(GameObject prefab, int count, bool t)
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

    //Utility Methods
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
}
