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
        mate = animal.FindClosestMate();

        if(mate == null)
        {
            animal.ChangeState(new WanderState(animal));
            return;
        }

        animal.hasMate = true;
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
            Debug.Log("Üredim.  " + animal.animalName);
            animal.Breed();
            animal.ChangeState(new WanderState(animal));
            return;
        }

        animal.MoveTo(mate.transform.position);
    }
}
