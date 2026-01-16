using UnityEngine;

[System.Serializable]
public class MemoryEntry
{
    public MemoryType type;
    public Vector3 position;
    public float timer;
    public float maxLifeTime;

    public bool IsValid => timer < maxLifeTime;

    public MemoryEntry(MemoryType type, Vector3 position, float maxLifeTime)
    {
        this.type = type;
        this.position = position;
        this.maxLifeTime = maxLifeTime;
        this.timer = 0f;
    }

    public void Update()
    {
        timer += Time.deltaTime;
    }
}

public enum MemoryType
{
    Food,
    Water,
    Mate
}
