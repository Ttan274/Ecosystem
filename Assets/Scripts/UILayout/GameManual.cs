using UnityEngine;
using UnityEngine.UIElements;

public class GameManual : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    private VisualElement manualPanel;
    private Label contentLabel;

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        manualPanel = root.Q<VisualElement>("ManualPanel");
        contentLabel = root.Q<Label>("ContentLabel");

        root.Q<Button>("CameraTab").clicked += () => ShowSection("Camera");
        root.Q<Button>("PanelTab").clicked += () => ShowSection("Panel");
        root.Q<Button>("AdminTab").clicked += () => ShowSection("Admin");
        root.Q<Button>("CloseButton").clicked += () => CloseManual();

        // Optional: open default
        manualPanel.style.display = DisplayStyle.None;
    }

    private void ShowSection(string section)
    {
        switch (section)
        {
            case "Camera":
                contentLabel.text = "📷 CAMERA CONTROLS\n\n- Camera movement with W-A-S-D keys.\n- Left shift key for faster movement.\n- Hold left mouse click move mouse for rotation of camera.\n- TAB key for switching between camera modes.\n- In fly mode user controls camera.\n- In orbit mode camera rotates itself around the map.";
                break;
            case "Panel":
                contentLabel.text = "🧪 SIMULATION PANEL\n\n- Opens with M key.\n- View animal stats from the buttons.\n- Minimap for checking animals.\n- Can control minimap with buttons";
                break;
            case "Admin":
                contentLabel.text = "🛠 ADMIN TOOLS\n\n- Add animals.\n- Trigger events like drought, disease.\n- Change speed of simulation or pause the simulation.";
                break;
        }
    }

    public void OpenManual()
    {
        manualPanel.style.display = DisplayStyle.Flex;
        ShowSection("Camera");
    }

    private void CloseManual()
    {
        manualPanel.style.display = DisplayStyle.None;
        gameObject.SetActive(false);
        pauseMenu.SetActive(true);
    }
}
