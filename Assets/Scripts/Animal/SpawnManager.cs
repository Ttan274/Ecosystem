using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Entity Prefabs")]
    [SerializeField] private GameObject sheep;
    [SerializeField] private GameObject goat;
    [SerializeField] private GameObject bear;
    [SerializeField] private List<string> animalNames = new List<string>();
    
    //References
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
    }

    public void Initialize(List<Tile> tiles) => this.tiles = tiles;

    #region Animal Spawn Methods
    public Animal SpawnAnimal(SpeciesType type, Gender gender)
    {
        Tile tile = tiles[Random.Range(0, tiles.Count)];
        tiles.Remove(tile);
        ParentData data = new ParentData("User","User");
        return SpawnAnimal(type, tile.transform.position, data, gender);
    }

    public Animal SpawnAnimal(SpeciesType type, Vector3 spawnPos, ParentData data, Gender gen = Gender.Unknown)
    {
        //Setting up the gender and prefab for the animal
        Gender gender = GetGenderForAnimal(gen);
        GameObject prefab = GetPrefabForAnimal(type, gender);

        //Spawning the animal
        Vector3 pos = spawnPos + new Vector3(0, 0.5f, 0);
        Animal a = Instantiate(prefab, pos, Quaternion.identity, transform).GetComponent<Animal>();
        a.Initialize(GetNameForAnimal(), gender, type, data);

        return a;
    }
    #endregion

    #region Utility Methods
    private Gender GetGenderForAnimal(Gender gen) => (gen != Gender.Unknown) ? gen : (Random.value < 0.5f) ? Gender.Male : Gender.Female;

    private GameObject GetPrefabForAnimal(SpeciesType type, Gender gender)
    {
        if (type == SpeciesType.Carnivore)
            return bear;

        return (gender == Gender.Male) ? goat : sheep;
    }

    private string GetNameForAnimal() => animalNames[Random.Range(0, animalNames.Count)] + "-" + Random.Range(0, 99);
    #endregion
}
