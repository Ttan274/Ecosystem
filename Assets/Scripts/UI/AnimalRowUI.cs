using TMPro;
using UnityEngine;

public class AnimalRowUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI idTxt;
    [SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private TextMeshProUGUI genderTxt;
    [SerializeField] private TextMeshProUGUI stateTxt;
    private Animal animal;

    public void SetData(int index, Animal a)
    {
        idTxt.text = index.ToString();
        animal = a;
    }

    private void Update()
    {
        if (animal == null) return;
        nameTxt.text = animal.animalName;
        genderTxt.text = animal.gender.ToString();
        stateTxt.text = animal.status.ToString();
    }
}
