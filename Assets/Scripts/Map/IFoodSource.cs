using UnityEngine;

public interface IFoodSource
{
    Transform FoodTransform { get; }
    bool IsAvailable { get; }
    void Consume();
}
