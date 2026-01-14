using UnityEngine;

public class SeekFoodState : IAnimalState
{
    private Animal animal;
    
    public SeekFoodState(Animal animal)
    {
       this.animal = animal; 
    }

    public void Enter()
    {
        animal.FindFood();
    }

    public void Exit()
    {
        
    }

    public string GetStateName() => "Seek-Food";

    public void Tick()
    {
        if(animal.food == null)
        {
            animal.ChangeState(new WanderState(animal));
            return;
        }

        float distance = Vector3.Distance(
            animal.transform.position,
            animal.food.transform.position);
        if(distance <= animal.eatDistance)
        {
            animal.ChangeState(new EatState(animal));
            return;
        }

        animal.FollowPath();
    }
}
