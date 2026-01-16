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
        if(!animal.searchIntent.IsActive)
        {
            BaseNeed urgent = animal.GetMostUrgentNeed();
            if (urgent != null)
            {
                urgent.Resolve();
                return;
            }
        }

        animal.WalkRandomly();
    }
}