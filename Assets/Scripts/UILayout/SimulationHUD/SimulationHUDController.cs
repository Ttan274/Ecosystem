using UnityEngine;
using UnityEngine.UIElements;

public class SimulationHUDController : MonoBehaviour
{
    //References
    private VisualElement root;

    //HUD Variables
    private float timeS;
    private bool isStopped;
    private bool isCollapsed;

    private void Awake()
    {
        timeS = Time.timeScale;
        root = GetComponent<UIDocument>().rootVisualElement;

        //Default Value
        root.Q<IntegerField>("spawnCountField").value = 1;
        root.Q<Toggle>("droughtToggle").SetValueWithoutNotify(WorldManager.Instance.IsDroughtActive);

        //Button Assignments
        CallbackAssignments();
    }

    private void Update()
    {
        UpdatePanels();
    }

    private void UpdatePanels()
    {
        //Camera mod 
        root.Q<Label>("cameraModeLabel").text = $"Camera Mode : {CameraController.Instance.mode.ToString()}";

        //Day-Time
        root.Q<Label>("timeLabel").text = $"{DayCycle.Instance.UpdateDayInfo()}";

        //Game Speed
        root.Q<Label>("speedLabel").text = isStopped ? "Sim Paused" : "Game Speed: " + Time.timeScale.ToString();
        root.Q<Button>("pauseButton").text = isStopped ? "Resume" : "Pause";
    }

    private void CallbackAssignments()
    {
        //Game Speed
        root.Q<Button>("speedDownButton").clicked += () =>
        {
            ChangeGameSpeed(SpeedType.Decrease);
        };
        root.Q<Button>("speedUpButton").clicked += () =>
        {
            ChangeGameSpeed(SpeedType.Increase);
        };
        root.Q<Button>("pauseButton").clicked += () =>
        {
            ChangeGameSpeed(SpeedType.Pause);
        };

        //Spawn Animals
        root.Q<Button>("spawnHerbivoreButton").clicked += () =>
        {
            int c = GetSpawnCount();
            WorldManager.Instance.RequestSpawnByUser(SpeciesType.Herbivore, c);
        };
        root.Q<Button>("spawnCarnivoreButton").clicked += () =>
        {
            int c = GetSpawnCount();
            WorldManager.Instance.RequestSpawnByUser(SpeciesType.Carnivore, c);
        };

        //Disaster
        root.Q<Button>("diseaseHerbivore").clicked += () =>
        {
            WorldManager.Instance.ApplyDisease(true);
        };

        root.Q<Button>("diseaseCarnivore").clicked += () =>
        {
            WorldManager.Instance.ApplyDisease(false);
        };

        root.Q<Toggle>("droughtToggle").RegisterValueChangedCallback(e =>
        {
            WorldManager.Instance.ApplyDrought(e.newValue);
        });

        //Left Panel Toggle
        root.Q<Button>("toggleHUD").clicked += ToggleLeftPanel;
    }

    #region Button Helpers
    private void ChangeGameSpeed(SpeedType type)
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

        if(isStopped)
        {
            Time.timeScale = 0f;
        }
        else
        {
            timeS = Mathf.Clamp(timeS, 1f, 4f);
            Time.timeScale = timeS;
        }
    }
   
    private int GetSpawnCount()
    {
        int count = root.Q<IntegerField>("spawnCountField").value;
        count = Mathf.Clamp(count, 1, 20);

        root.Q<IntegerField>("spawnCountField").SetValueWithoutNotify(count);
        return count;
    }
    
    private void ToggleLeftPanel()
    {
        isCollapsed = !isCollapsed;

        if(isCollapsed)
        {
            root.Q<VisualElement>("leftPanel").AddToClassList("collapsed");
            root.Q<Button>("toggleHUD").text = ">";
        }
        else
        {
            root.Q<VisualElement>("leftPanel").RemoveFromClassList("collapsed");
            root.Q<Button>("toggleHUD").text = "≡";
        }
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