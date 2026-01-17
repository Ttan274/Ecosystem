using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    [SerializeField] private int MaxSizeForHerbivores = 20;
    [SerializeField] private int MaxSizeForCarnivores = 20;
    public int diseaseApplied { get; private set; }
    public bool IsDroughtActive { get; private set; }
    public float droughtTimer { get; private set; }

    public static WorldManager Instance { get; private set; }
    public readonly List<Animal> Animals = new();
    
    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        if(IsDroughtActive)
            droughtTimer += Time.deltaTime;
    }

    #region Animal Registration
    public void RegisterAnimal(Animal animal)
    {
        if (!Animals.Contains(animal))
            Animals.Add(animal);
    }

    public void UnregisterAnimal(Animal animal)
    {
        if (Animals.Contains(animal))
            Animals.Remove(animal);
    }

    #endregion

    #region Animal Spawn/Kill Requests
    public void KillAnimal(Animal animal, DeathType reason)
    {
        if (!Animals.Contains(animal))
            return;

        UnregisterAnimal(animal);
        WorldEvents.RaiseAnimalDied(animal, reason);
        
        //animal.OnKiled(reason);
    }

    public void RequestBirth(Animal parent)
    {
        if (!CanSpawn(parent.Species))
            return;

        Animal baby = SpawnManager.Instance.SpawnAnimal(parent.Species, parent.transform.position);
        
        RegisterAnimal(baby);
        WorldEvents.RaiseAnimalBorn(baby);
    }

    public void RequestSpawnByUser(SpeciesType species, int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (!CanSpawn(species))
                return;

            Gender gender = (i % 2 == 0) ? Gender.Male : Gender.Female;
            Animal a = SpawnManager.Instance.SpawnAnimal(species, gender);
            
            RegisterAnimal(a);
        }
    }

    private bool CanSpawn(SpeciesType species)
    {
        int maxSize = species == SpeciesType.Herbivore ? MaxSizeForHerbivores : MaxSizeForCarnivores;
        int currentSize = Count(species);

        if (currentSize < maxSize)
            return true;
        return false;
    }

    #endregion

    #region World Behaviors
    public void ApplyDisease(bool isHerbivore)
    {
        SpeciesType species = isHerbivore ? SpeciesType.Herbivore : SpeciesType.Carnivore;
        List<Animal> candidates = GetAnimalsBySpecies(species).FindAll(a => !a.isInfected);

        if (candidates.Count == 0)
            return;

        int rand = Random.Range(0, candidates.Count);
        candidates[rand].Infect();
        diseaseApplied++;
    }

    public void ApplyDrought(bool isDroughtActive) => IsDroughtActive = isDroughtActive;

    #endregion

    #region Helper Methods
    public int Count(SpeciesType species)
    {
        int count = 0;

        foreach (var a in Animals)
            if (a.Species == species && !a.isDead)
                count++;

        return count;
    }

    public List<Animal> GetAnimalsBySpecies(SpeciesType speciesType)
    {
        var list = new List<Animal>();

        list = Animals.FindAll(a => a.Species == speciesType && !a.isDead);

        return list;
    }
    #endregion
}
