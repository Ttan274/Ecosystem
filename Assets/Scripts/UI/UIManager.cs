using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Canvas Panels")]
    [SerializeField] private GameObject adminPanel;
    
    [Header("UI Toolkit Panels")]
    [SerializeField] private PauseMenuController pauseMenu;
    [SerializeField] private SimulationHUDController hudController;

    //bool flags
    private bool isAdminPanelActive = false;
    private bool isPauseActive = false;

    //References
    private CameraController cam;

    private void Awake()
    {
        cam = Camera.main.GetComponent<CameraController>();
        pauseMenu.OnContinue += Continue;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M) && !isPauseActive)
            ToggleAdminPanel();

        if(Input.GetKeyDown(KeyCode.P))
            TogglePause();
    }

    private void ToggleAdminPanel()
    {
        isAdminPanelActive = !isAdminPanelActive;
        adminPanel.SetActive(isAdminPanelActive);

        if(isAdminPanelActive)
        {
            cam.ChangeCamActiveness(false);
            hudController.Hide();
        }
        else
        {
            cam.ChangeCamActiveness(true);
            hudController.Show();
        }
    }

    private void TogglePause()
    {
        isPauseActive = !isPauseActive;

        if(isPauseActive)
        {
            pauseMenu.Show();
            hudController.Hide();
            adminPanel.SetActive(false);
            isAdminPanelActive = false;
            cam.ChangeCamActiveness(false);
        }
        else
        {
            Continue();
        }
    }

    private void Continue()
    {
        isPauseActive = false;

        pauseMenu.Hide();
        hudController.Show();
        
        cam.ChangeCamActiveness(true);
        Time.timeScale = 1f;
    }
}
