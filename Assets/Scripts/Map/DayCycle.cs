using TMPro;
using UnityEngine;

public class DayCycle : MonoBehaviour
{
    [Header("Day Cycle Data")]
    [SerializeField] private Light directionalLight;
    [SerializeField] private Gradient lightColor;
    [SerializeField] private AnimationCurve lightIntensity;
    [SerializeField] private float dayLength = 60f;
   
    private float dayTimer = 0f;
    private int dayCount = 0;

    public static DayCycle Instance;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        dayTimer += (Time.deltaTime * Time.timeScale) / dayLength;
        if(dayTimer >= 1f)
        {
            dayTimer = 0f;
            dayCount++;
            WorldEvents.RaiseDayChanged(dayCount);
        }
        UpdateLight();
    }

    public string UpdateDayInfo()
    {
        int totalMinutes = Mathf.FloorToInt(dayTimer * 24f * 60f);
        int hours = totalMinutes / 60;
        int minutes = totalMinutes % 60;
        
        string clock = $"{hours:00}:{minutes:00}";
        return $"Day {dayCount} - {clock}"; 
    }

    private void UpdateLight()
    {
        if(directionalLight != null)
        {
            directionalLight.transform.rotation = Quaternion.Euler(new Vector3((dayTimer * 360f) - 90f, 170f, 0f));
            directionalLight.color = lightColor.Evaluate(dayTimer);
            directionalLight.intensity = lightIntensity.Evaluate(dayTimer);
        }
    }
}
