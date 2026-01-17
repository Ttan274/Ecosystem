using UnityEngine;

public class Plant : MonoBehaviour, IFoodSource
{
    public Tile parentTile;
    private bool isAlive = true;
    public Transform FoodTransform => transform;
    public bool IsAvailable => isAlive;
    
    public void Initalize(Tile t) => parentTile = t;

    public void Consume()
    {
        if (!isAlive) return;

        isAlive = false;
        gameObject.SetActive(false);

        WorldEvents.RaisePlantConsumed(this);
    }
}
