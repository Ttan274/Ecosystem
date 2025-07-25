using TMPro;
using UnityEngine;

public class InGameUI : MonoBehaviour
{
    [Header("Speed Panel")]
    [SerializeField] private TextMeshProUGUI speedTxt;
    private bool isStopped = false;
    private float timeS;

    [Header("Disaster Panel")]
    [SerializeField] private TextMeshProUGUI droughtTxt;
    private bool isDroughtActive = false;

    private void Awake()
    {
        timeS = Time.timeScale;
        SetSpeedText();
    }

    #region Simulation Control Region
    public void CreateHerbivore(TMP_InputField field)
    {
       if(CanCreateEntity(field, out int size))
            Simulation.Instance.GenerateInitialAnimals(Simulation.Instance.herbivore, size, true);
    }

    public void CreateCarnivore(TMP_InputField field)
    {
        if (CanCreateEntity(field, out int size))
            Simulation.Instance.GenerateInitialAnimals(Simulation.Instance.carnivore, size, false);
    }

    public void CreatePlant(TMP_InputField field)
    {
        if (CanCreateEntity(field, out int size))
            Debug.Log("Plants has been added");
    }

    private bool CanCreateEntity(TMP_InputField field, out int size)
    {
        size = -1;
        // Validate that the disease field is not empty
        if (string.IsNullOrEmpty(field.text))
        {
            Debug.Log("Disease count cannot be empty!");
            return false;
        }

        // Validate that the input is a positive integer
        if (!int.TryParse(field.text, out size) || size <= 0)
        {
            Debug.Log("Invalid disease count provided!");
            return false;
        }

        field.text = "Size";
        return true;
    }

    #endregion

    #region Game Speed Control Region
    public void ChangeSpeed(int s) => ChangeSpeed((SpeedType)s);
    private void ChangeSpeed(SpeedType type)
    {
        switch (type)
        {
            case SpeedType.Increase:
                if (isStopped)
                    return;
                timeS *= 2f;
                break;
            case SpeedType.Decrease:
                if (isStopped)
                    return;
                timeS /= 2f;
                break;
            case SpeedType.Pause:
                isStopped = !isStopped;
                break;
            default:
                Debug.LogError("Invalid SpeedType provided.");
                break;
        }

        if (isStopped)
        {
            Time.timeScale = 0f;
        }
        else
        {
            timeS = Mathf.Clamp(timeS, 1f, 4f);
            Time.timeScale = timeS;
        }

        SetSpeedText();
    }

    private void SetSpeedText()
    {
        if (isStopped)
            speedTxt.text = "Sim Paused";
        else
            speedTxt.text = "Game Speed : " + Time.timeScale.ToString();
    }
    #endregion

    #region Disaster Control Region
    public void ApplyDisease(bool isHerbivore) => Simulation.Instance.ApplyDisease(isHerbivore);
    
    public void StartDrought()
    {
        isDroughtActive = !isDroughtActive;
        droughtTxt.text = isDroughtActive ? "Stop Drought" : "Start Drought";

        Simulation.Instance.StartDrought(isDroughtActive);
    }
    #endregion
}

[System.Serializable]
public enum SpeedType
{
    Increase,
    Decrease,
    Pause
}