using TMPro;
using UnityEngine;

public class SpeedPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI speedTxt;
    private bool isStopped = false;
    private float timeS;
    private void Awake()
    {
        timeS = Time.timeScale;
        SetSpeedText();
    }

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

        if(isStopped)
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
            speedTxt.text = Time.timeScale.ToString();
    }
}

[System.Serializable]
public enum SpeedType
{
    Increase,
    Decrease,
    Pause
}
