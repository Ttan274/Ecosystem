using UnityEngine;

public class Herbivore : Animal, IFoodSource
{
    public Transform FoodTransform => transform;
    public bool IsAvailable => !isDead;

    protected override void Update()
    {
        base.Update();
    }

    public override void Breed()
    {
        childCount++;
        matingTimer = 0;

        if (gender == Gender.Female)
            SpawnManager.Instance.GenerateAnimal(true, transform.position);
    }

    public override bool CanEat(IFoodSource source) => source is Plant;

    public void Consume()
    {
        deathBehaviour.SetDirectDead(DeathType.Predator);
        Hurt();
    }
}
