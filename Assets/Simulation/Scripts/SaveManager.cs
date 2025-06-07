using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void SaveMap(TileType[,] typeMap, float[,] perlinMap, int width, int height)
    {
        MapData data = new MapData(width, height, typeMap, perlinMap);
        string json = JsonUtility.ToJson(data, true);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/map.json", json);
        Debug.Log("Map saved to: " + Application.persistentDataPath);
    }

    public (TileType[,], float[,]) LoadMap()
    {
        string path = Application.persistentDataPath + "/map.json";
        if (!System.IO.File.Exists(path))
        {
            Debug.LogWarning("No saved map found.");
            return (null, null);
        }

        string json = System.IO.File.ReadAllText(path);
        MapData data = JsonUtility.FromJson<MapData>(json);
        return (data.ConvertToTypeMap(), data.ConvertToPerlinMap());
    }
}
