using System.Collections.Generic;
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

public static class MapSettingsValidator
{
    public static bool Validate(MapSettings map, out List<string> errors)
    {
        errors = new List<string>();

        if (map.width < 20)
            errors.Add("Map width must be at least 20");

        if (map.height < 20)
            errors.Add("Map height must be at least 20");

        if (map.lakeCount < 1)
            errors.Add("Lake count must be at least 1");

        if (map.lakeRadius < 1)
            errors.Add("Lake radius must be at least 1");

        if (map.tree.maxCount < 0)
            errors.Add("Tree count cannot be neagtive");

        if (map.tree.spawnChance < 0 || map.tree.spawnChance > 1)
            errors.Add("Tree spawn chance must be between 0 and 1");

        if (map.bush.maxCount < 0)
            errors.Add("Bush count cannot be neagtive");

        if (map.bush.spawnChance < 0 || map.bush.spawnChance > 1)
            errors.Add("Tree spawn chance must be between 0 and 1");

        return errors.Count == 0;
    }
}