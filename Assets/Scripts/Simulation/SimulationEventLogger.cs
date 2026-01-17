using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.LightingExplorerTableColumn;

public class SimulationEventLogger : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform eventLogContainer;
    [SerializeField] private GameObject eventItemPrefab;
    [SerializeField] private int maxLogItems = 5;

    private readonly Queue<GameObject> logItems = new Queue<GameObject>();

    #region
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

    private void SEL_OnAnimalBorn(Animal animal)
    {
        //Debug.Log(
        //    $"[EVENT] Birth | {animal.Species} | Name : {animal.animalName} | Gender : {animal.gender}"
        //);

        AddLog($"[EVENT] Birth | {animal.Species} | Name : {animal.animalName} | Gender : {animal.gender}", Color.green);
    }

    private void SEL_OnAnimalDied(Animal animal, DeathType deathType)
    {
        //Debug.Log(
        //   $"[EVENT] Death | {animal.Species} | Name : {animal.animalName} | Death Reason : {deathType.ToString()} "
        //);

        AddLog($"[EVENT] Death | {animal.Species} | Name : {animal.animalName} | Death Reason : {deathType.ToString()}", Color.red);
    }

    private void SEL_OnDayChanged(int day)
    {
        //Debug.Log(
        //    $"[EVENT] Day Changed | Day {day}"
        //);

        AddLog($"[EVENT] Day Changed | Day {day}", Color.black);
    }


    private void AddLog(string message, Color color)
    {
        GameObject logItem = Instantiate(eventItemPrefab, eventLogContainer);
        TextMeshProUGUI logText = logItem.GetComponentInChildren<TextMeshProUGUI>();

        logText.text = message;
        logText.color = color;

        //Queue management
        logItems.Enqueue(logItem);
        if(logItems.Count > maxLogItems)
            Destroy(logItems.Dequeue());
    }
}
