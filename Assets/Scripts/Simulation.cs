using System.Collections.Generic;
using UnityEngine;

public class Simulation : MonoBehaviour
{
    [Header("Entity Details")]
    public GameObject herbivore;
    [SerializeField] private int initialHerbivoreCount;
    public GameObject carnivore;
    [SerializeField] private int initialCarnivoreCount;

    private List<Animal> animals = new List<Animal>();
    public List<Animal> Animals => animals;
    public int animalsCount => animals.Count;
    private bool herbivoresCreated = false;
    private bool carnivoresCreated = false;

    //References
    [SerializeField]private List<string> animalNames = new List<string>() { "John", "Lucia", "Maxvel", "Naber", "Hello", "Wikwik", "Susangus", "Naber" };
    private List<Tile> tiles = new List<Tile>();
    public static Simulation Instance;

    public void Initalize(List<Tile> tiles) => this.tiles = tiles;

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
                GenerateAnimals(herbivore, initialHerbivoreCount);
                herbivoresCreated = true;
                return;
            }

            if(herbivoresCreated && !carnivoresCreated)
            {
                GenerateAnimals(carnivore, initialCarnivoreCount);
                carnivoresCreated = true;
                return;
            }
        }
    }

    private void GenerateAnimals(GameObject prefab, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Tile place = tiles[Random.Range(0, tiles.Count)];
            tiles.Remove(place);
            Gender gender = (i % 2 == 0) ? Gender.Male : Gender.Female;
            GenerateAnimal(prefab, place.transform.position, gender);
        }
    }

    public void GenerateAnimal(GameObject animalPrefab, Vector3 spawnPos, Gender gen = Gender.Unknown)
    {
        Vector3 pos = spawnPos + new Vector3(0, 0.5f, 0);
        Animal g = Instantiate(animalPrefab, pos, Quaternion.identity).GetComponent<Animal>();
        
        Gender gender;
        if (gen != Gender.Unknown)
            gender = gen;
        else
            gender = (Random.value < 0.5f) ? Gender.Male : Gender.Female;

        g.Initialize(GetNameForAnimal(), gender);
        animals.Add(g);
    }

    private string GetNameForAnimal()
    {
        int rand = Random.Range(0, animalNames.Count);
        string n = animalNames[rand];
        animalNames.Remove(n);
        return n;
    }
}
