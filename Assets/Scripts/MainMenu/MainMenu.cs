using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject documentationPanel;
    [SerializeField] private GameObject optionsPanel;
    
    public static MainMenu Instance;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void OpenDocumentationPanel(bool status)
    {
        mainMenuPanel.SetActive(!status);
        documentationPanel.SetActive(status); 
    }

    public void OpenOptionsPanel(bool status)
    {
        mainMenuPanel.SetActive(!status);
        optionsPanel.SetActive(status);
    }

    #region Enter/Exit Simulation
    public void StartSimulation() => SceneManager.LoadScene("FoodChain");
    public void ExitGame() => Application.Quit();
    #endregion
}
