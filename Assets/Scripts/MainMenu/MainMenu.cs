using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject documentationPanel;

    public void StartSimulation() => SceneManager.LoadScene("FoodChain");

    public void OpenDocumentationPanel(bool status)
    {
        mainMenuPanel.SetActive(!status);
        documentationPanel.SetActive(status); 
    }

    public void ExitGame() => Application.Quit();
}
