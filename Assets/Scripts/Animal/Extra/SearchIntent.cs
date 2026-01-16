using UnityEngine;

[System.Serializable]
public class SearchIntent 
{
    public SearchIntentType type = SearchIntentType.None;
    public float timer = 0f;
    public float maxDuration = 5f;
    public Vector3? targetPos;  
    public bool IsActive => type != SearchIntentType.None;

    public void Start(SearchIntentType t, float duration)
    {
        type = t;
        timer = 0f;
        maxDuration = duration;
    }

    public void Update()
    {
        if (!IsActive) return;

        timer += Time.deltaTime;
        if (timer >= maxDuration)
            Clear();
    }

    public void SetTargetPosition(Vector3 pos)
    {
        targetPos = pos;
    }

    public void Clear()
    {
        timer = 0f;
        type = SearchIntentType.None;
    }

    public bool Is(SearchIntentType t) => type == t;
}

public enum SearchIntentType
{
    None,
    Food,
    Water,
    Mate
}
