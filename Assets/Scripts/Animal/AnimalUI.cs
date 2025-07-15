using UnityEngine;
using UnityEngine.UI;

public class AnimalUI : MonoBehaviour
{
    [Header("Animal UI Parts")]
    [SerializeField] private Slider hungerSlider;
    [SerializeField] private Slider thirstSlider;
    [SerializeField] private Transform lookTarget;
    [SerializeField] private Image image;

    private void LateUpdate()
    {
        //Setting for facing the camera
        if (lookTarget)
            transform.LookAt(Camera.main.transform);
    }

    public void SetGenderBar(Gender gender) => image.color = gender == Gender.Male ? Color.blue : Color.yellow;
    public void SetHunger(float c, float m) => SetSlider(c, m, hungerSlider);
    public void SetThirst(float c, float m) => SetSlider(c, m, thirstSlider);
    private void SetSlider(float current, float max, Slider slider)
    {
        slider.maxValue = max;
        slider.value = current;
    }
}
