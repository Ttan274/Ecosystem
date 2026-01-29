using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PauseMenuController : MonoBehaviour
{
    //Variables
    private VisualElement root;

    public System.Action OnContinue;

    private void Awake()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        AssignCallbacks();
        Hide();
    }

    private void AssignCallbacks()
    {
        root.Q<Button>("continueButton").clicked += () =>
        {
            OnContinue?.Invoke();
        };

        root.Q<Button>("exportButton").clicked += () =>
        {
            Simulation.Instance.ExportToJSON();
        };

        root.Q<Button>("mainMenuButton").clicked += () =>
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        };
    }

    #region Show/Hide
    public void Show()
    {
        root.style.display = DisplayStyle.Flex;
        Time.timeScale = 0f;
    }

    public void Hide()
    {
        root.style.display = DisplayStyle.None;
        Time.timeScale = 1f;
    }
    #endregion
}
