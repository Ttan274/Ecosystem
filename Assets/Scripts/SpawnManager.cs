using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    [Header("Entity Prefabs")]
    [SerializeField] private GameObject sheep;
    [SerializeField] private GameObject goat;
    [SerializeField] private GameObject bear;
    [SerializeField] private int maxAnimalCount;
    [SerializeField] private List<string> animalNames = new List<string>();
    
    //References
    public List<Animal> animalList { get; private set; }
    public static SpawnManager Instance;
    private List<Tile> tiles = new List<Tile>();

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        animalList = new List<Animal>();
    }

    public void Initialize(List<Tile> tiles) => this.tiles = tiles;

    #region Animal Spawn Methods
    public void GenerateAnimals(bool isHerbivore, int count)
    {
        if (animalList.Count >= maxAnimalCount)
            return;

        for (int i = 0; i < count; i++)
        {
            Tile tile = tiles[Random.Range(0, tiles.Count)];
            tiles.Remove(tile);
            Gender gender = (i % 2 == 0) ? Gender.Male : Gender.Female;
            GenerateAnimal(isHerbivore, tile.transform.position, gender);
        }
    }

    public void GenerateAnimal(bool isHerbivore, Vector3 spawnPos, Gender gen = Gender.Unknown)
    {
        if (animalList.Count >= maxAnimalCount)
            return;

        //Setting up the gender and prefab for the animal
        Gender gender = GetGenderForAnimal(gen);
        GameObject prefab = GetPrefabForAnimal(isHerbivore, gender);

        //Spawning the animal
        Vector3 pos = spawnPos + new Vector3(0, 0.5f, 0);
        Animal a = Instantiate(prefab, pos, Quaternion.identity, transform).GetComponent<Animal>();
        a.Initialize(GetNameForAnimal(), gender);

        //Listing the animals
        animalList.Add(a);
        Simulation.Instance.RegisterAnimal(a);
    }
    #endregion

    #region Utility Methods
    private Gender GetGenderForAnimal(Gender gen) => (gen != Gender.Unknown) ? gen : (Random.value < 0.5f) ? Gender.Male : Gender.Female;

    private GameObject GetPrefabForAnimal(bool isHerbivore, Gender gender) => (isHerbivore == false) ? bear : (gender == Gender.Male) ? goat : sheep;

    private string GetNameForAnimal() => animalNames[Random.Range(0, animalNames.Count)] + "-" + Random.Range(0, 99);
    #endregion
}
