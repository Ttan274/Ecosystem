using System.Collections.Generic;

[System.Serializable]
public class SimulationData 
{
    //Simulation Related Data
    public float time;
    public float droughtTimer;
    public int diseaseApplied;
    public int herbivoreCount;
    public int carnivoreCount;
}

[System.Serializable]
public class AnimalStats
{
    public string type; //Herbivore or Carnivore
    public string name;
    public string gender;
    public string deadType;
    public int age;
    public int eatenObjectCount;
    public int childCount;
}


[System.Serializable]
public class Wrapper
{
    public List<SimulationData> data;
}

[System.Serializable]
public class AnimalsWrapper
{
    public List<AnimalStats> animalsStats;
}