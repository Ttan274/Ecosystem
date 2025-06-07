using System.Collections;
using TMPro;
using UnityEngine;

public class MapConfig : MonoBehaviour
{
    [SerializeField] private TMP_InputField widthField, heightField;
    [SerializeField] private TextMeshProUGUI warningMsg;

    public void OnGenerate()
    {
        if(string.IsNullOrEmpty(widthField.text) ||
           string.IsNullOrEmpty(heightField.text))
        {
            StartCoroutine(ShowWarning("Please fill in the blanks for generating a map"));
            return;
        }

        if(int.TryParse(widthField.text, out int width) &&
           int.TryParse(heightField.text, out int height))
        {
            GridGenerator.instance.MapSetup(width, height);
            GridGenerator.instance.GenerateGrid();
        }
        else
        {
            StartCoroutine(ShowWarning("Please enter valid numbers"));
        }
    }

    private IEnumerator ShowWarning(string message)
    {
        warningMsg.text = message;
        warningMsg.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        warningMsg.gameObject.SetActive(false);
    }
}
