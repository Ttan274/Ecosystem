using UnityEngine;

public class SeekWaterState : IAnimalState
{
    private Animal animal;
    private Tile targetWater;

    public SeekWaterState(Animal animal)
    {
        this.animal = animal;
    }

    public void Enter()
    {
        animal.searchIntent.Start(SearchIntentType.Water, 5f);

        //Vision
        targetWater = animal.GetClosestWater();
        if(targetWater != null)
        {
            Tile current = Pathfinder.Instance.GetTileAtPosition(animal.transform.position);
            Tile waterTile = Pathfinder.Instance.GetClosestWalkableToWaterTile(targetWater);
            if (current != null && waterTile != null)
                animal.SetPath(current, waterTile);
            return;
        }

        //Memory
        Vector3? memPos = animal.GetMemoryPosition(MemoryType.Water);
        if(memPos.HasValue)
        {
            Tile current = Pathfinder.Instance.GetTileAtPosition(animal.transform.position);
            Tile tileAtPos = Pathfinder.Instance.GetTileAtPosition(memPos.Value);
            Tile waterTile = Pathfinder.Instance.GetClosestWalkableToWaterTile(tileAtPos);

            if (current != null && waterTile != null)
                animal.SetPath(current, waterTile);
            return;
        }
    }

    public void Exit()
    {
        targetWater = null;
    }

    public string GetStateName() => "Seek-Water";

    public void Tick()
    {
        if(targetWater == null)
        {
            animal.ChangeState(new WanderState(animal));
            return;
        }

        float distance = Vector3.Distance(animal.transform.position, targetWater.transform.position);
        if (distance <= animal.drinkDistance + 0.5f)
        {
            animal.GetNeed<ThirstNeed>()?.ResolveCompleted();
            animal.currentPath.Clear();
            animal.searchIntent.Clear();
            animal.ChangeState(new WanderState(animal));
            return;
        }

        animal.FollowPath();
    }
}
