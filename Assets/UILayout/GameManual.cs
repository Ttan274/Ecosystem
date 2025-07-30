using UnityEngine;
using UnityEngine.UIElements;

public class GameManual : MonoBehaviour
{
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
        root.Q<Button>("CloseButton").clicked += () => manualPanel.style.display = DisplayStyle.None;

        // Optional: open default
        manualPanel.style.display = DisplayStyle.None;
    }

    private void ShowSection(string section)
    {
        switch (section)
        {
            case "Camera":
                contentLabel.text = "📷 CAMERA CONTROLS\n\n- Right-click + drag to rotate.\n- Scroll to zoom.\n- Middle mouse to pan.";
                break;
            case "Panel":
                contentLabel.text = "🧪 SIMULATION PANEL\n\n- View animal stats.\n- Control simulation time.\n- Monitor environmental changes.";
                break;
            case "Admin":
                contentLabel.text = "🛠 ADMIN TOOLS\n\n- Add animals.\n- Trigger events like drought, disease.\n- Use minimap for fast navigation.";
                break;
        }
    }

    public void OpenManual()
    {
        manualPanel.style.display = DisplayStyle.Flex;
        ShowSection("Camera");
    }
}
