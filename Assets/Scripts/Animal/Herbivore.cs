using UnityEngine;

public class Herbivore : Animal
{
    protected override void Update()
    {
        base.Update();
    }

    #region Food
    protected override void FoodSearch()
    {
        if(!canSearch)
            return; //No food left for herbivores

        if (food == null)
        {
            FindFood();
            if (food != null)
            {
                Tile current = Pathfinder.Instance.GetTileAtPosition(transform.position);
                Tile destination = Pathfinder.Instance.GetTileAtPosition(food.transform.position);
                SetPath(current, destination);
            }
        }

        if (food != null)
        {
            float distance = Vector3.Distance(transform.position, food.transform.position);
            if (distance < eatDistance)
            {
                Eat();
                return;
            }
            FollowPath();
        }
    }

    protected override void FindFood()
    {
        Plant[] allPlants = FindObjectsByType<Plant>(FindObjectsSortMode.None);

        if (allPlants.Length <= 0)
        {
            canSearch = false;
            return; //No plants left
        }

        float closestDist = Mathf.Infinity;
        Plant closest = null;

        foreach (Plant plant in allPlants)
        {
            float d = Vector3.Distance(transform.position, plant.transform.position);
            if (d < closestDist)
            {
                closestDist = d;
                closest = plant;
            }
        }

        food = closest.gameObject;
    }

    protected override void Eat()
    {
        food.GetComponent<Plant>().Eat();
        base.Eat();
    }
    #endregion
   
    #region Mate
    protected override void FindMate()
    {
        Herbivore[] herbivores = FindObjectsByType<Herbivore>(FindObjectsSortMode.None);

        foreach (Herbivore other in herbivores)
        {
            if (other == this || !other.IsReadyToMate || other.gender == this.gender || other.hasMate) continue;

            float distance = Vector3.Distance(transform.position, other.transform.position);
            if(distance <= matingDistance)
            {
                hasMate = true;
                other.hasMate = true;
                Vector3 pos = (this.gender == Gender.Female) ? this.transform.position : other.transform.position;
                Breed(other, pos);
                break;
            }
        }
    }

    private void Breed(Herbivore partner, Vector3 pos)
    {
        //Instantiate a child herbivore
        SpawnManager.Instance.GenerateAnimal(true, pos);
        childCount++;
        partner.childCount++;
        
        //resetting mating datas;
        matingTimer = 0;
        partner.matingTimer = 0;
        hasMate = false;
        partner.hasMate = false;
    }
    #endregion

    //Helper method when eaten by carnivores
    public void GotEaten() => Die(0f, true);
}
