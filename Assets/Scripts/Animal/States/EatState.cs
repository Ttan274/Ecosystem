using UnityEngine;

public class EatState : IAnimalState
{
    private Animal animal;

    public EatState(Animal animal)
    {
        this.animal = animal;
    }

    public void Enter()
    {
        animal.Eat();
        animal.ChangeState(new WanderState(animal));
    }

    public void Exit()
    {
    }

    public string GetStateName() => "Eat";

    public void Tick()
    {
    }
}
