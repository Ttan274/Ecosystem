using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdminPanel : MonoBehaviour
{
    [SerializeField] private GameObject animalRow;
    [SerializeField] private Transform animalTable;

    [SerializeField] private Camera minimapCam;
    [SerializeField] private Slider minimapSlider;
    [SerializeField] private TextMeshProUGUI minimapTxt;
    private float minimapMovementAmount = 1f;

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
                dir = Vector3.left * minimapMovementAmount;
                break;
            case MinimapMovement.MoveRight:
                dir = Vector3.right * minimapMovementAmount;
                break;
            case MinimapMovement.MoveUp:
                dir = Vector3.forward * minimapMovementAmount;
                break;
            case MinimapMovement.MoveDown:
                dir = Vector3.back * minimapMovementAmount;
                break;
            case MinimapMovement.ZoomIn:
                minimapCam.orthographicSize = Mathf.Max(7, minimapCam.orthographicSize - minimapMovementAmount);
                break;
            case MinimapMovement.ZoomOut:
                minimapCam.orthographicSize = Mathf.Min(25, minimapCam.orthographicSize + minimapMovementAmount);
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
            Mathf.Clamp(minimapCam.transform.position.x, 10f, 50f),
            minimapCam.transform.position.y,
            Mathf.Clamp(minimapCam.transform.position.z, 10f, 50f)
        );
    }

    public void ChangeMinimapMovementAmount()
    {
        minimapMovementAmount = minimapSlider.value;
        minimapTxt.text = "Move Amount : " + minimapMovementAmount.ToString();
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