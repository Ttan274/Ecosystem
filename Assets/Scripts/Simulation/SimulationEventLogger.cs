using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SimulationEventLogger : MonoBehaviour
{
    [Header("Logger Data")]
    [SerializeField] private int maxLogItems = 5;
    private VisualElement root;
    private bool isCollapsed;

    private readonly Queue<VisualElement> logItems = new();

    #region Enable/Disable

    private void OnEnable()
    {
        WorldEvents.OnAnimalBorn += SEL_OnAnimalBorn;
        WorldEvents.OnAnimalDied += SEL_OnAnimalDied;
        WorldEvents.OnDayChanged += SEL_OnDayChanged;
    }

    private void OnDisable()
    {
        WorldEvents.OnAnimalBorn -= SEL_OnAnimalBorn;
        WorldEvents.OnAnimalDied -= SEL_OnAnimalDied;
        WorldEvents.OnDayChanged -= SEL_OnDayChanged;
    }

    #endregion

    private void Awake()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        //Callback
        root.Q<Button>("logCollapse").clicked += ToggleLogPanel;
    }

    private void ToggleLogPanel()
    {
        isCollapsed = !isCollapsed;

        if (isCollapsed)
        {
            root.Q<VisualElement>("logPanel").AddToClassList("collapsed");
            root.Q<Button>("logCollapse").text = "+";
        }
        else
        {
            root.Q<VisualElement>("logPanel").RemoveFromClassList("collapsed");
            root.Q<Button>("logCollapse").text = "-";
        }
    }

    #region Logs

    private void SEL_OnAnimalBorn(Animal animal)
    {
        //Debug.Log(
        //    $"[EVENT] Birth | {animal.Species} | Name : {animal.animalName} | Gender : {animal.gender}"
        //);

        AddLog($"[EVENT] Birth | {animal.Species} | Name : {animal.animalName} | Gender : {animal.gender}", "log-birth");
    }

    private void SEL_OnAnimalDied(Animal animal, DeathType deathType)
    {
        //Debug.Log(
        //   $"[EVENT] Death | {animal.Species} | Name : {animal.animalName} | Death Reason : {deathType.ToString()} "
        //);

        AddLog($"[EVENT] Death | {animal.Species} | Name : {animal.animalName} | Death Reason : {deathType.ToString()}", "log-death");
    }

    private void SEL_OnDayChanged(int day)
    {
        //Debug.Log(
        //    $"[EVENT] Day Changed | Day {day}"
        //);

        AddLog($"[EVENT] Day Changed | Day {day}", "log-day");
    }

    private void AddLog(string message, string cssClass)
    {
        //Label Setup
        var label = new Label(message);
        label.AddToClassList("log-entry");
        label.AddToClassList(cssClass);

        //Adding to the scroll & queue
        root.Q<ScrollView>("logScroll").Add(label);
        logItems.Enqueue(label);

        if(logItems.Count > maxLogItems)
        {
            var old = logItems.Dequeue();
            old.RemoveFromHierarchy();
        }

        root.Q<ScrollView>("logScroll").ScrollTo(label);
    }

    #endregion
}
