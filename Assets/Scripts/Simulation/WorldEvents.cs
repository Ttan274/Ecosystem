using System;

public static class WorldEvents
{
    // =========================
    // Animal Events
    // =========================

    public static event Action<Animal, DeathType> OnAnimalDied;
    public static event Action<Animal> OnAnimalBorn;

    // =========================
    // Plant Events
    // =========================

    public static event Action<Plant> OnPlantConsumed;

    // =========================
    // Time / World Events
    // =========================

    public static event Action<int> OnDayChanged;

    // =========================
    // Invoke Methods
    // =========================

    public static void RaiseAnimalDied(Animal animal, DeathType deathType) => OnAnimalDied?.Invoke(animal, deathType);
    public static void RaiseAnimalBorn(Animal animal) => OnAnimalBorn?.Invoke(animal);
    public static void RaisePlantConsumed(Plant plant) => OnPlantConsumed?.Invoke(plant);
    public static void RaiseDayChanged(int newDay) => OnDayChanged?.Invoke(newDay);
}
