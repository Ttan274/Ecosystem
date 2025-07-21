using TMPro;
using UnityEngine;

public class AnimalRowUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI idTxt;
    [SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private TextMeshProUGUI genderTxt;
    [SerializeField] private TextMeshProUGUI stateTxt;

    public void SetData(int index, Animal a)
    {
        idTxt.text = index.ToString();
        nameTxt.text = a.animalName;
        genderTxt.text = a.gender.ToString();
        stateTxt.text = a.status.ToString();
    }
}
