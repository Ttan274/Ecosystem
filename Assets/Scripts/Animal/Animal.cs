using System.Collections;
using System.Collections.Generic;
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
    public SpeciesType Species;
    
    //Age
    public int age { get; private set; } = 0;
    private int maxAge;
    private int nextAgeCounter = 0;
    public bool isAdult => age >= 1;    //age >= 2 && age < 10;

    //State - Needs
    public IAnimalState currentState { get; private set; }
    protected List<BaseNeed> needs = new List<BaseNeed>();
    private VisionSensor sensor;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float tileTolerance;
    [SerializeField] private float randomWalkRange;
    private int pathIndex = 0;
    public List<Tile> currentPath = new List<Tile>();
    private Animator animator;
    protected bool canSearch = true;

    [Header("Search Intent + Memory")]
    public SearchIntent searchIntent = new SearchIntent();
    public List<MemoryEntry> memories = new List<MemoryEntry>();

    [Header("Hunger")]
    [SerializeField] private float hungerDecayRate;
    [SerializeField] private float hungerThreshold;
    public float eatDistance;
    public float currentHunger {  get; private set; }   

    [Header("Thirst")]
    [SerializeField] private float thirstDecayRate;
    [SerializeField] private float thirstThreshold;
    public float drinkDistance;
    public float currentThirst { get; private set; }

    [Header("Mating")]
    public float matingCooldown;
    public float matingThreshold;
    public float matingDistance;
    private float chaseTimer = 0f;
    public bool hasMate = false;
    public float matingTimer { get; set; }

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
    public bool showDebug = false;
    public ParentData parentData;

    #region Enable/Disable
    private void OnEnable()
    {
        WorldEvents.OnDayChanged += HandleDayChange;
    }

    private void OnDisable()
    {
        WorldEvents.OnDayChanged -= HandleDayChange;
    }
    #endregion

    public void Initialize(string aName, Gender g, SpeciesType speciesType, ParentData data)
    {
        gizmoColor = Random.ColorHSV();
        animUI = GetComponentInChildren<AnimalUI>();
        animator = GetComponent<Animator>();
        sensor = GetComponent<VisionSensor>();

        //Starting value of animal datas
        currentHunger = 100f;
        currentThirst = 100f;
        currentHealth = 100f;

        //Animal specific data
        Id = globalId++;
        animalName = aName;
        Species = speciesType;
        gameObject.name = animalName;
        gender = g;
        maxAge = Random.Range(8, 15);
        parentData = new ParentData(data.motherName, data.fatherName);

        //Default UI Bar
        animUI.SetGenderBar(gender);
        animUI.SetHunger(currentHunger, 100f);
        animUI.SetThirst(currentThirst, 100f);

        //Death Behaviour Setup
        deathBehaviour = new DeathBehaviour(0f);

        //Setting the needs
        needs.Add(new HungerNeed(this, hungerDecayRate, hungerThreshold));
        needs.Add(new ThirstNeed(this, thirstDecayRate, thirstThreshold));
        needs.Add(new ReproductionNeed(this));

        ChangeState(new WanderState(this));
    }

    protected virtual void Update()
    {
        foreach (var need in needs)
            need.Update();

        currentState?.Tick();

        if (animator != null)
            animator.SetBool("Move", currentState is WanderState
                                  || currentState is SeekFoodState
                                  || currentState is SeekWaterState);

        for(int i = memories.Count - 1; i >= 0; i--)
        {
            memories[i].Update();
            if (!memories[i].IsValid)
                memories.RemoveAt(i);
        }

        DeathCheck();
        searchIntent.Update();
        matingTimer += Time.deltaTime;
    }

    public void ChangeState(IAnimalState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

    #region Memory
    public void Remember(MemoryType type, Vector3 pos, float lifeTime)
    {
        Forget(type);

        //add if type not exists
        memories.Add(new MemoryEntry(type, pos, lifeTime));
    }

    public void Remember(MemoryType type, Animal entity, float lifeTime)
    {
        Forget(type);

        //add if type not exists
        memories.Add(new MemoryEntry(type, entity, lifeTime));
    }

    private void Forget(MemoryType type) => memories.RemoveAll(m => m.type == type);

    private Vector3? GetMemoryPosition(MemoryType type)
    {
        foreach (MemoryEntry m in memories)
        {
            if (m.type == type && m.IsValid && m.IsSpatial)
                return m.position;
        }
        return null;
    }

    public Animal GetMemoryEntity(MemoryType type)
    {
        foreach (MemoryEntry m in memories)
        {
            if (m.type == type && m.IsValid && m.IsEntity)
                return m.entity;
        }
        return null;
    }

    public bool TryFeedMemoryToIntent(MemoryType type)
    {
        Vector3? memPos = GetMemoryPosition(type);
        if (!memPos.HasValue)
            return false;

        searchIntent.SetTargetPosition(memPos.Value);
        return true;
    }

    #endregion

    #region Needs
    public void Breed(Animal mate) 
    {
        childCount++;
        matingTimer = 0;

        if (gender == Gender.Female)
            WorldManager.Instance.RequestBirth(this, mate);
    }     

    public BaseNeed GetMostUrgentNeed()
    {
        BaseNeed mostUrgent = null;
        float highest = float.MinValue;

        foreach (var need in needs)
        {
            if (!need.IsUrgent())
                continue;

            float score = need.UrgencyScore();
            if (score > highest)
            {
                highest = score;
                mostUrgent = need;
            }
        }

        return mostUrgent;
    }

    public T GetNeed<T>() where T : BaseNeed
    {
        foreach (var need in needs)
        {
            if (need is T)
                return need as T;
        }

        return null;
    }

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
            int pX = current.x + Random.Range(-Mathf.RoundToInt(randomWalkRange), Mathf.RoundToInt(randomWalkRange));
            int pZ = current.z + Random.Range(-Mathf.RoundToInt(randomWalkRange), Mathf.RoundToInt(randomWalkRange));

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
        
        float dist = Vector3.Distance(transform.position, targetPos);

        if (dist <= tileTolerance)
            pathIndex++;
        else
            MoveTo(targetPos);
    }

    private void MoveTo(Vector3 targetPos)
    {
        Vector3 dir = (targetPos - transform.position).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;
        transform.LookAt(new Vector3(targetPos.x, transform.position.y, targetPos.z));
    }

    public void ChaseEntity(Animal target, float repathInterval = 0.5f)
    {
        if (target == null) return;

        chaseTimer += Time.deltaTime;
        if(chaseTimer < repathInterval && currentPath != null && currentPath.Count > 0)
        {
            FollowPath();
            return;
        }

        chaseTimer = 0f;

        Tile current = Pathfinder.Instance.GetTileAtPosition(transform.position);
        Tile destination = Pathfinder.Instance.GetTileAtPosition(target.transform.position);

        if (current == null || destination == null)
            return;

        if(!destination.IsWalkable())
            destination = Pathfinder.Instance.GetClosestWalkableTile(destination);

        if (destination == null)
            return;

        SetPath(current, destination);
    }

    public void SetPath(Tile c, Tile d)
    {

        currentPath = Pathfinder.Instance.CreatePath(c, d);
        pathIndex = 0;
    }
    #endregion

    #region Vision

    public IFoodSource GetClosestFood()
    {
        if (sensor == null) return null;

        float minDist = float.MaxValue;
        IFoodSource closest = null;

        foreach (var food in sensor.foodSources)
        {
            if (!CanEat(food))
                continue;

            float d = Vector3.Distance(transform.position, food.FoodTransform.position);

            if (d < minDist)
            {
                minDist = d;
                closest = food;
            }
        }

        return closest;
    }

    public Tile GetClosestWater()
    {
        if (sensor == null) return null;

        float minDist = float.MaxValue;
        Tile closest = null;

        foreach (GameObject obj in sensor.visibleTargets)
        {
            Tile t = obj.GetComponent<Tile>();
            if (t == null) continue;

            float d = Vector3.Distance(transform.position, t.transform.position);
            if (d < minDist)
            {
                minDist = d;
                closest = t;
            }
        }

        return closest;
    }

    public Animal GetClosestMate()
    {
        if (sensor == null) return null;

        float minDist = float.MaxValue;
        Animal closest = null;

        foreach (Animal other in sensor.visibleAnimals)
        {
            if (!CanMate(other)) continue;

            float distance = Vector3.Distance(transform.position, other.transform.position);
            if(distance < minDist)
            {
                minDist = distance;
                closest = other;
            }
        }

        return closest;
    }

    public bool CanMate(Animal other)
    {
        //If other is null cannot mate
        if (other == null) return false;
        //If other is dead cannot mate
        if (other.isDead) return false;
        //If other has same gender cannot mate
        if (this.gender == other.gender) return false;
        //If other has mate  cannot mate
        if (other.hasMate) return false;
        //Check species both are different then they cannot mate
        if (other.Species != this.Species) return false;

        return true;
    }

    #endregion

    #region Food & Water
    public bool CanEat(IFoodSource source)
    {
        if (source == null || !source.IsAvailable)
            return false;

        if (Species == SpeciesType.Herbivore && source is Plant)
            return true;

        if(Species == SpeciesType.Carnivore && source is Herbivore)
            return true;

        return false;
    }

    public void Eat() 
    {
        currentPath.Clear();
        GetNeed<HungerNeed>()?.ResolveCompleted();
        eatenObjectCount++;
    }

    public void SetThirst(float val)
    {
        currentThirst = val;
        animUI.SetThirst(currentThirst, 100f);
    }

    public void SetHunger(float val)
    {
        currentHunger = val;
        animUI.SetHunger(currentHunger, 100f);
    }
    
    #endregion

    #region Health
    private void DeathCheck()
    {
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

        if (currentHunger <= 0 || currentThirst <= 0)
        {
            deathBehaviour.SetDeathBehaviour(needsDamage, DeathType.HungerORThirst, false);
            Hurt();
        }
    }

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
        WorldManager.Instance.KillAnimal(this, deathBehaviour.deathType);
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

    private void HandleDayChange(int day)
    {
        UpdateAge();
    }

    protected void UpdateAge()
    {
        if (isDead)
            return;

        nextAgeCounter++;

        if (nextAgeCounter >= 2)
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

[System.Serializable]
public struct ParentData
{
    public int motherId;
    public int fatherId;
    public string motherName;
    public string fatherName;

    public ParentData(string m, string f)
    {
        motherId = -1;
        fatherId = -1;

        motherName = m;
        fatherName = f;
    }

    public ParentData(Animal m, Animal f)
    {
        motherId = m.Id;
        fatherId = f.Id;

        motherName = m.animalName;
        fatherName = f.animalName;
    }
}

public enum SpeciesType
{
    Herbivore,
    Carnivore,
    Omnivore
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