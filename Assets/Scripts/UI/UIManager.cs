using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject inGamePanel;
    [SerializeField] private AdminPanel adminPanel;
    private CanvasGroup adminPanelCanvas;
    private bool isAdminPanelActive = false;

    private void Awake()
    {
        adminPanelCanvas = adminPanel.GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            isAdminPanelActive = !isAdminPanelActive;
            inGamePanel.SetActive(!isAdminPanelActive);
            adminPanelCanvas.alpha = isAdminPanelActive ? 1 : 0;
        }
    }
}
