using UnityEngine;

public class AdminPanel : MonoBehaviour
{
    [SerializeField] private GameObject animalRow;
    [SerializeField] private Transform animalTable;
    private bool isActive = false;
    private CanvasGroup canvas;

    [SerializeField] private Camera minimapCam;

    private void Awake()
    {
        canvas = GetComponent<CanvasGroup>();
    }

    public void ShowHerbivores()
    {
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
        GameObject row = Instantiate(animalRow, animalTable);
        AnimalRowUI rowUI = row.GetComponent<AnimalRowUI>();
        rowUI.SetData(index, animal);
    }

    public void MoveMinimap(int movementDir)
    {
        Vector3 dir = Vector3.zero;
        switch ((MinimapMovement)movementDir)
        {
            case MinimapMovement.MoveLeft:
                dir = Vector3.left;
                break;
            case MinimapMovement.MoveRight:
                dir = Vector3.right;
                break;
            case MinimapMovement.MoveUp:
                dir = Vector3.forward;
                break;
            case MinimapMovement.MoveDown:
                dir = Vector3.back;
                break;
            case MinimapMovement.ZoomIn:
                minimapCam.orthographicSize = Mathf.Max(7, minimapCam.orthographicSize - 1);
                break;
            case MinimapMovement.ZoomOut:
                minimapCam.orthographicSize = Mathf.Min(20, minimapCam.orthographicSize + 1);
                break;
            default:
                Debug.Log("No movement method like this");
                break;
        }
        MoveCamera(dir);
    }

    private void MoveCamera(Vector3 dir)
    {
        minimapCam.transform.position += dir;

        minimapCam.transform.position = new Vector3(
            Mathf.Clamp(minimapCam.transform.position.x, 0f, 40f),
            minimapCam.transform.position.y,
            Mathf.Clamp(minimapCam.transform.position.z, 0f, 40f)
        );
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

public enum MinimapMovement
{
    MoveLeft,
    MoveRight,
    MoveUp,
    MoveDown,
    ZoomIn,
    ZoomOut
}