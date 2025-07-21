using TMPro;
using UnityEngine;

public class AnimalAdminUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI animalName;
    [SerializeField] private TextMeshProUGUI animalGender;
    [SerializeField] private TextMeshProUGUI animalStatus;

    public void SetUIBar(string n, string g, string s)
    {
        animalName.text = n;
        animalGender.text = g;
        animalStatus.text = s;
    }
}
