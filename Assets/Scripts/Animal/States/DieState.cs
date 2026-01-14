using UnityEngine;

public class DieState : IAnimalState
{
    private Animal animal;

    public DieState(Animal animal)
    {
        this.animal = animal;
    }

    public void Enter()
    {
        animal.Die();
    }

    public void Exit()
    {
    }

    public string GetStateName() => "Die";

    public void Tick()
    {
    }
}
