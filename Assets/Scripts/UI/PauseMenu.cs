using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject documentationPanel;

    public void MainMenu() => SceneManager.LoadScene("MainMenu");

    public void OpenDocumentationPanel()
    {
        documentationPanel.SetActive(true);
        documentationPanel.GetComponent<GameManual>().OpenManual();
        gameObject.SetActive(false);
    }
}
