using UnityEngine;

public class SeekFoodState : IAnimalState
{
    private Animal animal;
    private IFoodSource targetFood;
    private Herbivore prey;
    private Tile plantTile;
    
    public SeekFoodState(Animal animal)
    {
       this.animal = animal; 
    }

    public void Enter()
    {
        animal.searchIntent.Start(SearchIntentType.Food, 5f);

        //Vision
        targetFood = animal.GetClosestFood();
        if(targetFood != null)
        {
            ResolveFoodType(targetFood);
            return;
        }

        //Memory    
        Animal rememberedPrey = animal.GetMemoryEntity(MemoryType.Food);
        if(rememberedPrey != null)
        {
            prey = rememberedPrey as Herbivore;
            return;
        }

        if(animal.TryFeedMemoryToIntent(MemoryType.Food))
        {
            plantTile = Pathfinder.Instance.GetTileAtPosition(animal.searchIntent.targetPos.Value);
            SetPathToFood();
            return;
        }
    }

    public void Exit()
    {
        targetFood = null;
        prey = null;
        plantTile = null;
    }

    public string GetStateName() => "Seek-Food";

    public void Tick()
    {
        //Dynamic Food Source
        if(prey != null)
        {
            if (prey == null || prey.isDead || !prey.gameObject.activeInHierarchy)
            {
                animal.ChangeState(new WanderState(animal));
                return;
            }

            float distance = Vector3.Distance(animal.transform.position, prey.transform.position);
            if (distance <= animal.eatDistance)
            {
                prey.Consume();
                animal.searchIntent.Clear();
                animal.ChangeState(new EatState(animal));
                return;
            }

            animal.ChaseEntity(prey);
            return;
        }

        //Static Food Source
        if(plantTile != null)
        {
            if (!plantTile.hasPlant || !plantTile.plant.IsAvailable)
            {
                animal.ChangeState(new WanderState(animal));
                return;
            }

            float distance = Vector3.Distance(animal.transform.position, plantTile.transform.position);
            if(distance <= animal.eatDistance)
            {
                plantTile.plant.Consume();
                animal.searchIntent.Clear();
                animal.ChangeState(new EatState(animal));
                return;
            }

            animal.FollowPath();
            return;
        }

        //No Food Source Found
        animal.WalkRandomly();
        return;
    }

    //Helpers
    private void ResolveFoodType(IFoodSource source)
    {
        if(source is Animal a)
        {
            prey = a as Herbivore;
            plantTile = null;
        }
        else
        {
            prey = null;
            plantTile = Pathfinder.Instance.GetTileAtPosition(source.FoodTransform.position);
            SetPathToFood();
        }
    }

    private void SetPathToFood()
    {
        Tile current = Pathfinder.Instance.GetTileAtPosition(animal.transform.position);
        if (current != null && plantTile != null)
            animal.SetPath(current, plantTile);
    }
}
