using UnityEngine;

public class SeekFoodState : IAnimalState
{
    private Animal animal;
    private IFoodSource target;
    
    public SeekFoodState(Animal animal)
    {
       this.animal = animal; 
    }

    public void Enter()
    {
        animal.searchIntent.Start(SearchIntentType.Food, 5f);

        //Vision
        target = animal.GetClosestFood();
        if(target != null)
        {
            Tile current = Pathfinder.Instance.GetTileAtPosition(animal.transform.position);
            Tile destination = Pathfinder.Instance.GetTileAtPosition(target.FoodTransform.position);
            if (current != null && destination != null)
                animal.SetPath(current, destination);
            return;
        }

        //Memory
        Vector3? memPos = animal.GetMemoryPosition(MemoryType.Food);
        if(memPos.HasValue)
        {
            Tile current = Pathfinder.Instance.GetTileAtPosition(animal.transform.position);
            Tile destination = Pathfinder.Instance.GetTileAtPosition(memPos.Value);

            if (current != null && destination != null)
                animal.SetPath(current, destination);
            return;
        }
    }

    public void Exit()
    {
        target = null;
    }

    public string GetStateName() => "Seek-Food";

    public void Tick()
    {
        if(target == null)
        {
            animal.ChangeState(new WanderState(animal));
            return;
        }

        float distance = Vector3.Distance(
            animal.transform.position,
            target.FoodTransform.position);

        if(distance <= animal.eatDistance)
        {
            animal.searchIntent.Clear();
            target.Consume();
            animal.ChangeState(new EatState(animal));
            return;
        }

        animal.FollowPath();
    }
}
