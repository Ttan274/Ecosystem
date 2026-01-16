using UnityEngine;

[System.Serializable]
public class MemoryEntry
{
    //Memory Data
    public MemoryType type;

    //Spatial Data
    public Vector3 position;

    //Entity Data
    public Animal entity;

    //Timer
    public float timer;
    public float maxLifeTime;

    public bool IsValid
    {
        get
        {
            if (timer >= maxLifeTime)
                return false;

            if(entity != null)
                return entity.gameObject.activeInHierarchy && !entity.isDead;

            return true;
        }
    }

    public bool IsEntity => entity != null;
    public bool IsSpatial => entity == null;

    public MemoryEntry(MemoryType type, Vector3 position, float maxLifeTime)
    {
        this.type = type;
        this.position = position;
        this.maxLifeTime = maxLifeTime;
        this.timer = 0f;
    }

    public MemoryEntry(MemoryType type, Animal entity, float maxLifeTime)
    {
        this.type = type;
        this.entity = entity;
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
