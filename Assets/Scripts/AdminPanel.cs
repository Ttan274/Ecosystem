using UnityEngine;

public class AdminPanel : MonoBehaviour
{
    [SerializeField] private GameObject animalBar;
    private CanvasGroup canvasGroup;
    private bool isOpen = false;
    private GameObject[] bars;

    public static AdminPanel Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        canvasGroup = GetComponent<CanvasGroup>();
        GenerateBars();
    }

    private void Update()
    {
        ChangeAdminPanelVisibility();
        UpdateAdminPanel();
    }

    private void UpdateAdminPanel()
    {
        var animals = Simulation.Instance.Animals;
        for (int i = 0; i < animals.Count; i++)
        {
            AnimalAdminUI a = bars[i].GetComponent<AnimalAdminUI>();
            a.SetUIBar(animals[i].animalName, animals[i].gender.ToString(), animals[i].status.ToString());
        }
    }

    private void GenerateBars()
    {
        bars = new GameObject[12];
        for (int i = 0; i < 12; i++)
        {
            GameObject g = Instantiate(animalBar, transform);
            g.name = "AnimalBar" + i;
            bars[i] = g;
            bars[i].SetActive(false);
        }
    }

    private void ChangeBarVisibility(bool x)
    {
        for (int i = 0; i < Simulation.Instance.animalsCount; i++)
            bars[i].SetActive(x);
    }
    
    private void ChangeAdminPanelVisibility()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (!isOpen)
            {
                isOpen = true;
                canvasGroup.alpha = 1;
                ChangeBarVisibility(isOpen);
            }
            else
            {
                isOpen = false;
                canvasGroup.alpha = 0;
                ChangeBarVisibility(isOpen);
            }
        }
    }
}
