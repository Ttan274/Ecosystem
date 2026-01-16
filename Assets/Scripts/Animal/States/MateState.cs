using UnityEngine;

public class MateState : IAnimalState
{
    private Animal animal;
    private Animal mate;

    public MateState(Animal animal)
    {
        this.animal = animal;
    }

    public void Enter()
    {
        animal.hasMate = true;

        //Vision
        mate = animal.GetClosestMate();
        if (mate != null)
            return;

        //Memory
        mate = animal.GetMemoryEntity(MemoryType.Mate);
    }

    public void Exit()
    {
        animal.hasMate = false;
    }

    public string GetStateName() => "Mate";

    public void Tick()
    {
        if (mate == null || !mate.gameObject.activeInHierarchy)
        {
            animal.hasMate = false;
            animal.ChangeState(new WanderState(animal));
            return;
        }

        float distance = Vector3.Distance(animal.transform.position, mate.transform.position);
        if(distance <= animal.matingDistance)
        {
            animal.Breed();
            animal.searchIntent.Clear();
            animal.ChangeState(new WanderState(animal));
            return;
        }

        animal.ChaseEntity(mate);
    }
}
