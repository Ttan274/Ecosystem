using UnityEngine;

public class AdminPanel : MonoBehaviour
{
    [SerializeField] private GameObject animalRow;
    [SerializeField] private Transform animalTable;
    private bool isActive = false;
    private CanvasGroup canvas;

    private void Awake()
    {
        canvas = GetComponent<CanvasGroup>();
    }

    public void ShowHerbivores()
    {
        Debug.Log("Showing herbivores");
        ClearTable();
        var list = Simulation.Instance.herbivores;
        for (int i = 0; i < list.Count; i++)
            CreateRow(i, list[i]);
    }

    public void ShowCarnivores()
    {
        ClearTable();
        var list = Simulation.Instance.carnivores;
        for (int i = 0; i < list.Count; i++)
            CreateRow(i, list[i]);
    }

    private void ClearTable()
    {
        foreach (Transform child in animalTable)
            Destroy(child.gameObject);
    }

    private void CreateRow(int index, Animal animal)
    {
        Debug.Log(animal.animalName);
        GameObject row = Instantiate(animalRow, animalTable);
        AnimalRowUI rowUI = row.GetComponent<AnimalRowUI>();
        rowUI.SetData(index, animal);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            isActive = !isActive;
            canvas.alpha = isActive ? 1 : 0;
        }
    }
}
