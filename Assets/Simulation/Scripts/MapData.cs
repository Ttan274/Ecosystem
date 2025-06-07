using UnityEngine;

[System.Serializable]
public class MapData
{
    public int width;
    public int height;
    public TileType[] tiles;
    public float[] perlin;

    public MapData(int w, int h, TileType[,] typeMap, float[,] perlinMap)
    {
        width = w;
        height = h;
        tiles = new TileType[w * h];
        perlin = new float[w * h];

        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                int index = y * w + x;
                tiles[index] = typeMap[x, y]; // flatten to 1D
                perlin[index] = perlinMap[x, y]; // flatten to 1D
            }
        }
    }

    public TileType[,] ConvertToTypeMap()
    {
        TileType[,] result = new TileType[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                result[x, y] = tiles[y * width + x];
            }
        }
        return result;
    }

    public float[,] ConvertToPerlinMap()
    {
        float[,] result = new float[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                result[x, y] = perlin[y * width + x];
            }
        }
        return result;
    }
}
