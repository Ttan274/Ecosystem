using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject inGamePanel;
    [SerializeField] private AdminPanel adminPanel;
    [SerializeField] private GameObject pauseMenu;
    private CanvasGroup adminPanelCanvas;
    private bool isAdminPanelActive = false;
    private bool isPauseMenuActive = false;
    private float timeS;
    private CameraController cam;

    private void Awake()
    {
        adminPanelCanvas = adminPanel.GetComponent<CanvasGroup>();
        cam = Camera.main.GetComponent<CameraController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M) && !isPauseMenuActive)
        {
            isAdminPanelActive = !isAdminPanelActive;
            inGamePanel.SetActive(!isAdminPanelActive);
            adminPanelCanvas.alpha = isAdminPanelActive ? 1 : 0;
        }

        if(Input.GetKeyDown(KeyCode.P) && !isAdminPanelActive)
        {
            isPauseMenuActive = !isPauseMenuActive;
            inGamePanel.SetActive(!isPauseMenuActive);
            pauseMenu.SetActive(isPauseMenuActive);
            cam.ChangeCamActiveness(!isPauseMenuActive);
            ChangeGameSpeed(isPauseMenuActive);
        }
    }

    private void ChangeGameSpeed(bool isStopped)
    {
        if (isStopped)
        {
            timeS = Time.timeScale;
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = timeS;
        }
    }
}
