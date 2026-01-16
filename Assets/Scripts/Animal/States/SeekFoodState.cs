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
            SetPathToFood();   
            return;
        }

        //Memory    ??? SIkýntýlý çünkü sadece plant olabilir ama carnivoreda herbivore bulmamýz lazým
        if(animal.TryFeedMemoryToIntent(MemoryType.Food))
        {
            Tile targetTile = Pathfinder.Instance.GetTileAtPosition(animal.searchIntent.targetPos.Value);
            target = targetTile.hasPlant ? targetTile.plant : null;
            SetPathToFood();
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

        if(target != null)
        {
            float distance = Vector3.Distance(
           animal.transform.position,
           target.FoodTransform.position);

            if (distance <= animal.eatDistance)
            {
                animal.searchIntent.Clear();
                target.Consume();
                animal.ChangeState(new EatState(animal));
                return;
            }

            animal.FollowPath();
        }
    }

    private void SetPathToFood()
    {
        Tile current = Pathfinder.Instance.GetTileAtPosition(animal.transform.position);
        Tile destination = target != null ? Pathfinder.Instance.GetTileAtPosition(target.FoodTransform.position) : null;
        if (current != null && destination != null)
            animal.SetPath(current, destination);
    }
}
