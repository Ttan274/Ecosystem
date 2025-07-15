using System.Collections.Generic;
using UnityEngine;

public class Simulation : MonoBehaviour
{
    [Header("Entity Details")]
    [SerializeField] private GameObject herbivore;
    [SerializeField] private int initialHerbivoreCount;
    [SerializeField] private GameObject carnivore;
    [SerializeField] private int initialCarnivoreCount;

    public List<Animal> animals = new List<Animal>();
    private bool herbivoresCreated = false;
    private bool carnivoresCreated = false;

    //References
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
            Vector3 spawnPos = place.transform.position + new Vector3(0, 0.5f, 0);
            Animal g = Instantiate(prefab, spawnPos, Quaternion.identity).GetComponent<Animal>();
            animals.Add(g);
        }
    }
}
