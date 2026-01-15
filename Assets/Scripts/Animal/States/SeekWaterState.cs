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
        animal.isSearchingWater = true;
        animal.ResetWaterSearchTimer();

        targetWater = animal.GetClosestWater();

        if(targetWater != null)
        {
            Tile current = Pathfinder.Instance.GetTileAtPosition(animal.transform.position);
            Tile waterTile = Pathfinder.Instance.GetClosestWalkableToWaterTile(targetWater);
            if (current != null && waterTile != null)
                animal.SetPath(current, waterTile);
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
            animal.currentPath.Clear();
            animal.GetNeed<ThirstNeed>()?.ResolveCompleted();
            animal.ChangeState(new WanderState(animal));
            return;
        }

        animal.FollowPath();
    }
}
