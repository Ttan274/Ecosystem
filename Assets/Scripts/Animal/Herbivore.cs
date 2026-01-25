using UnityEngine;

public class Herbivore : Animal, IFoodSource
{
    public Transform FoodTransform => transform;
    public bool IsAvailable => !isDead;

    protected override void Update()
    {
        base.Update();
    }

    public void Consume()
    {
        deathBehaviour.SetDirectDead(DeathType.Predator);
        Hurt();
    }
}
