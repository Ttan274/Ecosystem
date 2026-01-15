using UnityEngine;

public class Plant : MonoBehaviour, IFoodSource
{
    private Tile parentTile;
    private bool isAlive = true;
    public Transform FoodTransform => transform;
    public bool IsAvailable => isAlive;
    
    public void Initalize(Tile t) => parentTile = t;

    public void Consume()
    {
        isAlive = false;
        parentTile.ResetTile(Simulation.Instance.IsDroughtEnabled);
        Destroy(gameObject);
    }
}
