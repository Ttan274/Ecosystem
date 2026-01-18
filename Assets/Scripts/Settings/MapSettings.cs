using UnityEngine;

//Congigurable settings for map generation

[System.Serializable]
public class MapSettings
{
    [Header("Map Size")]
    public int width;
    public int height;

    [Header("Lake Settings")]
    public int lakeCount;
    public int lakeRadius;
    [Range(0f, 1f)]
    public float irregularity;

    [Header("Plantable Settings")]
    public PlantableSettings tree;
    public PlantableSettings bush;
}


[System.Serializable]
public class  PlantableSettings
{
    public int maxCount;
    [Range(0f, 1f)]
    public float spawnChance;
}