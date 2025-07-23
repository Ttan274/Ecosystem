using TMPro;
using UnityEngine;

public class InGameUI : MonoBehaviour
{
    [Header("Speed Panel")]
    [SerializeField] private TextMeshProUGUI speedTxt;
    private bool isStopped = false;
    private float timeS;

    [Header("Disaster Panel")]
    [SerializeField] private TMP_InputField diseaseField;
    [SerializeField] private TextMeshProUGUI droughtTxt;
    private bool isDroughtActive = false;

    private void Awake()
    {
        timeS = Time.timeScale;
        SetSpeedText();
    }

    #region Game Speed Control Region
    public void ChangeSpeed(int s) => ChangeSpeed((SpeedType)s);
    private void ChangeSpeed(SpeedType type)
    {
        switch (type)
        {
            case SpeedType.Increase:
                timeS *= 2f;
                break;
            case SpeedType.Decrease:
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
    public void ApplyDisease()
    {
        // Validate that the disease field is not empty
        if (string.IsNullOrEmpty(diseaseField.text))
        {
            Debug.Log("Disease count cannot be empty!");
            return;
        }

        // Validate that the input is a positive integer
        if (!int.TryParse(diseaseField.text, out int diseaseCount) || diseaseCount <= 0)
        {
            Debug.Log("Invalid disease count provided!");
            return;
        }

        Simulation.Instance.ApplyDisease(diseaseCount);
    }

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