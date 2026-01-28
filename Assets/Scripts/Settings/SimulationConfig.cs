using UnityEngine;

[CreateAssetMenu(menuName = "Simulation/SimConfig")]
public class SimulationConfig : ScriptableObject
{
    public MapSettings mapSettings;
}

[CreateAssetMenu(menuName = "Simulation/MapPreset")]
public class MapPresetScriptable : ScriptableObject
{
    public string presetName;
    public MapSettings mapSettings;
}