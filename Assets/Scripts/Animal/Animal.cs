using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Animal : MonoBehaviour
{
    [Header("Identity")]
    private static int globalId = 0;
    public int Id { get; private set; }
    public Gender gender {get; private set;}
    public string animalName { get; private set;}
    public int childCount { get; protected set; } = 0;
    public int eatenObjectCount { get; protected set; } = 0;
    public DeathBehaviour deathBehaviour;
    
    //Age
    public int age { get; private set; } = 0;
    private int maxAge;
    private int nextAgeCounter = 0;
    private bool isAdult => age >= 2 && age < 10;

    //State
    public IAnimalState currentState { get; private set; }

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float tileTolerance;
    [SerializeField] private float randomWalkRange;
    private int pathIndex = 0;
    protected List<Tile> currentPath = new List<Tile>();
    private Animator animator;

    [Header("Hunger")]
    [SerializeField] private float hungerDecayRate;
    public float hungerThreshold;
    public float eatDistance;
    [SerializeField] protected float waitForHunger;
    public GameObject food {  get; set; }
    public float currentHunger {  get; private set; }   
    protected bool canSearchFood = true;
    private float searchFoodTimer = 0;
    protected bool canSearch = true;

    [Header("Thirst")]
    [SerializeField] private float thirstDecayRate;
    public float thirstThreshold;
    [SerializeField] private float drinkDistance;
    [SerializeField] private float waitForThirst;
    public float currentThirst { get; private set; }
    private bool canSearchWater = true;
    private float searchWaterTimer = 0;

    [Header("Mating")]
    [SerializeField] private float matingCooldown;
    [SerializeField] private float matingThreshold;
    public float matingDistance;
    protected float matingTimer = 0;
    public bool hasMate = false;
    public bool IsReadyToMate => isAdult && !isInfected && !hasMate && matingTimer >= matingCooldown
                                    && currentHunger >= 100f * matingThreshold
                                    && currentThirst >= 100f * matingThreshold;

    [Header("Health")]
    [SerializeField] private Color infectedColor = Color.green;
    [SerializeField] private float infectionDamage;
    [SerializeField] private float needsDamage;
    private float currentHealth;
    public bool isDead { get; private set; } = false;
    public bool isInfected {get; private set;} = false;
    
    //UI
    private AnimalUI animUI;
    private Color gizmoColor;

    //Debug
    [SerializeField] public bool showDebug = false;

    public void Initialize(string aName, Gender g)
    {
        gizmoColor = Random.ColorHSV();
        animUI = GetComponentInChildren<AnimalUI>();
        animator = GetComponent<Animator>();

        //Starting value of animal datas
        currentHunger = 100f;
        currentThirst = 100f;
        currentHealth = 100f;

        //Animal specific data
        Id = globalId++;
        animalName = aName;
        gameObject.name = animalName;
        gender = g;
        maxAge = Random.Range(8, 15);
        DayCycle.OnDayEnd += UpdateAge;

        //Default UI Bar
        animUI.SetGenderBar(gender);
        animUI.SetHunger(currentHunger, 100f);
        animUI.SetThirst(currentThirst, 100f);

        //Death Behaviour Setup
        deathBehaviour = new DeathBehaviour(0f);

        ChangeState(new WanderState(this));
    }

    protected virtual void Update()
    {
        UpdateNeeds();
        UpdateNeeds2();

        currentState?.Tick();

        if (animator != null)
            animator.SetBool("Move", currentState is WanderState
                                  || currentState is SeekFoodState
                                  || currentState is SeekWaterState);

        if (isInfected)
        {
            deathBehaviour.SetDeathBehaviour(infectionDamage, DeathType.Infection, false);
            Hurt();
        }
           

        if (age >= maxAge)
        {
            deathBehaviour.SetDirectDead(DeathType.Age);
            Hurt();
        }
    }

    public void ChangeState(IAnimalState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
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

        if (currentHunger <= 0 || currentThirst <= 0)
        {
            deathBehaviour.SetDeathBehaviour(needsDamage, DeathType.HungerORThirst, false);
            Hurt();
            return;
        }

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
    }

    public virtual Animal FindClosestMate() { return null; }
   
    public virtual void Breed() { }
    #endregion

    #region Movement
    protected void RandomTarget()
    {
        Tile current = Pathfinder.Instance.GetTileAtPosition(transform.position);
        if (current == null)
            return;

        bool canWalk = false;
        while (!canWalk)
        {
            int pX = current.x + UnityEngine.Random.Range(-Mathf.RoundToInt(randomWalkRange), Mathf.RoundToInt(randomWalkRange));
            int pZ = current.z + UnityEngine.Random.Range(-Mathf.RoundToInt(randomWalkRange), Mathf.RoundToInt(randomWalkRange));

            Tile destination = Pathfinder.Instance.GetTileGrid(pX, pZ);
            if(destination == null || destination.IsWalkable() == false)
                return;

            if (destination != null && current != null && destination.IsWalkable())
            {
                canWalk = true;
                SetPath(current, destination);
            }
        }
    }

    public void WalkRandomly()
    {
        if (currentPath == null)
            return;

        if (currentPath.Count == 0 || pathIndex >= currentPath.Count)
            RandomTarget();
        else
            FollowPath();
    }

    public void FollowPath()
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

    public void MoveTo(Vector3 targetPos)
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
    public virtual void FindFood() { }
    
    public virtual void Eat() 
    {
        currentPath.Clear();
        currentHunger = 100f;
        eatenObjectCount++;
        canSearchFood = false;
        //food = null;
    }
   
    public void WaterSearch()
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
            ChangeState(new WanderState(this));
            canSearchWater = false;
            return;
        }
        SetPath(current, closestWater);
        FollowPath();
    }
    #endregion

    #region Health
    protected void Hurt()
    {
        if (deathBehaviour.isDirectDead)
        {
            isDead = true;
        }
        else
        {
            currentHealth -= deathBehaviour.damage * Time.deltaTime;
            if (currentHealth <= 0)
                isDead = true;
        }

        if(isDead)
        {
            ChangeState(new DieState(this));
            return;
        }
    }

    public virtual void Die()
    {
        Simulation.Instance.RemoveAnimal(this);
        gameObject.SetActive(false);
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

    protected void UpdateAge()
    {
        if (isDead)
            return;

        nextAgeCounter++;

        if (nextAgeCounter >= 3)
        {
            nextAgeCounter = 0;
            age++;
        }
    }

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
    Predator,
    Age
}

public struct DeathBehaviour
{
    public float damage;
    public DeathType deathType;
    public bool isDirectDead;

    public DeathBehaviour(float dmg)
    {
        damage = dmg;
        deathType = DeathType.Alive;
        isDirectDead = false;
    }

    public void SetDeathBehaviour(float dmg, DeathType t, bool d)
    {
        damage = dmg;
        deathType = t;
        isDirectDead = d;
    }

    public void SetDirectDead(DeathType t)
    {
        damage = 0f;
        deathType = t;
        isDirectDead = true;
    }
}