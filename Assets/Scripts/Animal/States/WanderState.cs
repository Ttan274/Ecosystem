using UnityEngine;

public class WanderState : IAnimalState
{
    private Animal animal;

    public WanderState(Animal animal)
    {
        this.animal = animal;
    }

    public void Enter()
    {
        
    }

    public void Exit()
    {
        
    }

    public string GetStateName() => "Wander";

    public void Tick()
    {
        if(animal.IsReadyToMate)
        {
            animal.ChangeState(new MateState(animal));
            return;
        }

        if(animal.currentHunger < animal.hungerThreshold)
        {
            animal.ChangeState(new SeekFoodState(animal));
            return;
        }

        if (animal.currentThirst < animal.thirstThreshold)
        {
            animal.ChangeState(new SeekWaterState(animal));
            return;
        }

        animal.WalkRandomly();
    }
}