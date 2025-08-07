using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Animal : MonoBehaviour
{
    [Header("Identity")]
    public int Id { get; private set; }
    private static int globalId = 0;
    public Gender gender {get; private set;}
    public string animalName { get; private set;}
    public DeathType deathType { get; private set; } = DeathType.Alive;
    public AnimalState status => state;
    public int childCount { get; protected set; } = 0;
    public int eatenObjectCount { get; protected set; } = 0;

    [Header("Movement")]
    [SerializeField] protected AnimalState state = AnimalState.Idle;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float tileTolerance;
    [SerializeField] private float randomWalkRange;
    private int pathIndex = 0;
    protected List<Tile> currentPath = new List<Tile>();

    //Animator
    private Animator animator;

    [Header("Hunger")]
    [SerializeField] private float hungerDecayRate;
    [SerializeField] private float hungerThreshold;
    [SerializeField] protected float eatDistance;
    [SerializeField] protected float waitForHunger;
    protected GameObject food;
    protected float currentHunger;
    protected bool canSearchFood = true;
    private float searchFoodTimer = 0;
    protected bool canSearch = true;

    [Header("Thirst")]
    [SerializeField] private float thirstDecayRate;
    [SerializeField] private float thirstThreshold;
    [SerializeField] private float drinkDistance;
    [SerializeField] private float waitForThirst;
    private float currentThirst;
    private bool canSearchWater = true;
    private float searchWaterTimer = 0;

    [Header("Mating")]
    [SerializeField] private float matingCooldown;
    [SerializeField] private float matingThreshold;
    [SerializeField] protected float matingDistance;
    protected float matingTimer = 0;
    protected bool hasMate = false;
    protected bool IsReadyToMate => !isInfected && !hasMate && matingTimer >= matingCooldown
                                    && currentHunger >= 100f * matingThreshold
                                    && currentThirst >= 100f * matingThreshold;

    [Header("Health")]
    [SerializeField] private Color infectedColor = Color.green;
    [SerializeField] private float infectionDamage;
    [SerializeField] private float needsDamage;
    private float currentHealth;
    private bool isDead = false;
    public bool isInfected {get; private set;} = false;
    
    //UI
    private AnimalUI animUI;
    private Color gizmoColor;

    public void Initialize(string aName, Gender g)
    {
        gizmoColor = Random.ColorHSV();
        animUI = GetComponentInChildren<AnimalUI>();
        animator = GetComponent<Animator>();

        currentHunger = 100f;
        currentThirst = 100f;
        currentHealth = 100f;

        Id = globalId++;
        animalName = aName;
        gameObject.name = animalName;
        gender = g;
        animUI.SetGenderBar(gender);
    }

    protected virtual void Update()
    {
        UpdateNeeds();
        UpdateNeeds2();
        
        switch (state)
        {
            case AnimalState.Wander:
                WalkRandomly();
                break;
            case AnimalState.SeekFood:
                FoodSearch();
                break;
            case AnimalState.SeekWater:
                WaterSearch();
                break;
        }

        if (animator != null)
            animator.SetBool("Move", state != AnimalState.Idle);

        if (isInfected)
            Die(infectionDamage, DeathType.Infection);
    }

    #region Needs
    protected void UpdateNeeds()
    {
        if(canSearchFood)
        {
            currentHunger -= hungerDecayRate * Time.deltaTime;
            currentHunger = Mathf.Clamp(currentHunger, 0, 100f);
        }
        if(canSearchWater)
        {
            currentThirst -= thirstDecayRate * Time.deltaTime;
            currentThirst = Mathf.Clamp(currentThirst, 0, 100f);
        }

        if (currentHunger < hungerThreshold)
            state = AnimalState.SeekFood;
        if (currentThirst < thirstThreshold && food == null)
            state = AnimalState.SeekWater;
        if (currentHunger > hungerThreshold && currentThirst > thirstThreshold)
            state = AnimalState.Wander;

        if (currentHunger <= 0 || currentThirst <= 0)
            Die(needsDamage, DeathType.HungerORThirst);

        animUI.SetHunger(currentHunger, 100f);
        animUI.SetThirst(currentThirst, 100f);
    }

    private void UpdateNeeds2()
    {
        if (!canSearchWater)
        {
            searchWaterTimer += Time.deltaTime;

            if (searchWaterTimer >= waitForThirst)
            {
                canSearchWater = true;
                searchWaterTimer = 0;
            }
        }
        if (!canSearchFood)
        {
            searchFoodTimer += Time.deltaTime;

            if (searchFoodTimer >= waitForHunger)
            {
                canSearchFood = true;
                searchFoodTimer = 0;
            }
        }

        matingTimer += Time.deltaTime;
        if (IsReadyToMate)
            FindMate();
    }

    protected virtual void FindMate(){ }
    #endregion

    #region Movement
    protected void RandomTarget()
    {
        Tile current = Pathfinder.Instance.GetTileAtPosition(transform.position);
        if (current == null)
        {
            Debug.Log("Naber02");
            return;
        }

        bool canWalk = false;
        while (!canWalk)
        {
            int pX = current.x + Random.Range(-Mathf.RoundToInt(randomWalkRange), Mathf.RoundToInt(randomWalkRange));
            int pZ = current.z + Random.Range(-Mathf.RoundToInt(randomWalkRange), Mathf.RoundToInt(randomWalkRange));

            Tile destination = Pathfinder.Instance.GetTileGrid(pX, pZ);
            if(destination == null || destination.IsWalkable() == false)
                state = AnimalState.Idle;

            if (destination != null && current != null && destination.IsWalkable())
            {
                canWalk = true;
                SetPath(current, destination);
            }
        }
    }

    protected void WalkRandomly()
    {
        if (currentPath == null)
            return;

        if (currentPath.Count == 0 || pathIndex >= currentPath.Count)
            RandomTarget();
        else
            FollowPath();
    }

    protected void FollowPath()
    {
        if (currentPath == null || pathIndex >= currentPath.Count) return;

        Vector3 targetPos = currentPath[pathIndex].transform.position;
        if (targetPos == null)
            Debug.Log("Naber03");
        float dist = Vector3.Distance(transform.position, targetPos);

        if (dist <= tileTolerance)
            pathIndex++;
        else
            MoveTo(targetPos);
    }

    protected void MoveTo(Vector3 targetPos)
    {
        Vector3 dir = (targetPos - transform.position).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;
        transform.LookAt(new Vector3(targetPos.x, transform.position.y, targetPos.z));
    }

    protected void SetPath(Tile c, Tile d)
    {
        currentPath = Pathfinder.Instance.CreatePath(c, d);
        pathIndex = 0;
    }
    #endregion

    #region Food & Water
    protected virtual void FoodSearch() { }

    protected virtual void FindFood() { }
    
    protected virtual void Eat() 
    {
        currentPath.Clear();
        currentHunger = 100f;
        state = AnimalState.Wander;
        eatenObjectCount++;
        canSearchFood = false;
    }
   
    protected void WaterSearch()
    {
        Tile current = Pathfinder.Instance.GetTileAtPosition(transform.position);
        if (current == null) return;

        Tile closestWater = Pathfinder.Instance.GetClosestWaterTile(current);
        if (closestWater == null) return;

        float distance = Vector3.Distance(transform.position, closestWater.transform.position);
        if (distance < drinkDistance)
        {
            currentPath.Clear();
            currentThirst = 100f;
            state = AnimalState.Wander;
            canSearchWater = false;
            return;
        }
        SetPath(current, closestWater);
        FollowPath();
    }
    #endregion

    #region Health
    protected virtual void Die(float damage, DeathType d, bool directDead = false)
    {
        if(directDead)
        {
            Simulation.Instance.RemoveAnimal(this);
            isDead = true;
            //Destroy(gameObject);
        }
        else
        {
            currentHealth -= damage * Time.deltaTime;
            if (currentHealth <= 0)
            {
                Simulation.Instance.RemoveAnimal(this);
                isDead = true;
                //Destroy(gameObject);
            }
        }

        if(isDead)
        {
            gameObject.SetActive(false);
            deathType = d;
        }
    }
    
    public void Infect()
    {
        isInfected = true;
        StartCoroutine(InfectionBehaviour());
    }
    
    private IEnumerator InfectionBehaviour()
    {
        var renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        Material mat = renderers[0].material;
        Color defaultColor = mat.color;

        while (isInfected)
        {
            yield return new WaitForSeconds(0.4f);
            foreach (var rend in renderers)
                rend.material.color = infectedColor;
            yield return new WaitForSeconds(0.2f);
            foreach (var rend in renderers)
                rend.material.color = defaultColor;
        }
    }
    #endregion
    
    //debugging
    protected void OnDrawGizmos()
    {
        if (currentPath == null || currentPath.Count == 0) return;

        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, matingDistance);

        Gizmos.color = gizmoColor;
        for (int i = 0; i < currentPath.Count - 1; i++)
        {
            Vector3 from = currentPath[i].transform.position + Vector3.up * 0.1f;
            Vector3 to = currentPath[i + 1].transform.position + Vector3.up * 0.1f;
            Gizmos.DrawLine(from, to);
        }
    }
}

public enum AnimalState
{
    Idle,
    Wander,
    SeekFood,
    SeekWater
}

public enum Gender
{
    Unknown,
    Male,
    Female
}

public enum DeathType
{
    Alive,
    Infection,
    HungerORThirst,
    Predator
}