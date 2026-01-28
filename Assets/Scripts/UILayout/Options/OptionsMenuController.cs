using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class OptionsMenuController : MonoBehaviour
{
    public SimulationConfig runtimeConfig;
    public VisualTreeAsset mapSettingsView;
    public List<MapPresetScriptable> mapPresets;

    //References
    private VisualElement root;
    private VisualElement content;
    private TemplateContainer container;
    private Button mainMenuButton;
    private Label tooltipLabel;

    private string currentValidationMsg = "";
    private bool isApplyingPreset;

    private void Awake()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        content = root.Q<VisualElement>("content");

        root.Q<Button>("mapTab").clicked += LoadMapSettings;
        SetFooterArea();
    }

    private void SetFooterArea()
    {
        tooltipLabel = root.Q<Label>("validationTooltip");
        tooltipLabel.style.visibility = Visibility.Hidden;
        mainMenuButton = root.Q<Button>("startSimulationButton");

        mainMenuButton.RegisterCallback<MouseEnterEvent>(_ =>
        {
            if (!mainMenuButton.enabledSelf && !string.IsNullOrEmpty(currentValidationMsg))
                tooltipLabel.style.visibility = Visibility.Visible;
        });

        mainMenuButton.RegisterCallback<MouseLeaveEvent>(_ =>
        {
            tooltipLabel.style.visibility = Visibility.Hidden;
        });

        mainMenuButton.clicked += () =>
        {
            if (!MapSettingsValidator.Validate(runtimeConfig.mapSettings, out _))
                return;

            MainMenu.Instance.OpenOptionsPanel(false);
        };
    }

    #region Map Settings

    private void LoadMapSettings()
    {
        content.Clear();

        container = mapSettingsView.Instantiate();
        var map = runtimeConfig.mapSettings;
        SetPresetDropdown();
        AssingMapSettingCallbacks(map);
        
        content.Add(container);
    }

    private void ApplyPreset(MapSettings settings)
    {
        if (settings == null) return;

        isApplyingPreset = true;
        var map = runtimeConfig.mapSettings;

        //Mapping
        map.width = settings.width;
        map.height = settings.height;
        map.lakeCount = settings.lakeCount;
        map.lakeRadius = settings.lakeRadius;
        map.irregularity = settings.irregularity;
        map.tree.maxCount = settings.tree.maxCount;
        map.tree.spawnChance = settings.tree.spawnChance;
        map.bush.maxCount = settings.bush.maxCount;
        map.bush.spawnChance = settings.bush.spawnChance;

        RefreshMapUI(map);
        isApplyingPreset = false;
    }

    private void SetPresetDropdown()
    {
        //Preset
        var names = new List<string>();
        foreach (var s in mapPresets)
            names.Add(s.presetName);
        names.Add("Custom");

        container.Q<DropdownField>("presetDropdown").choices = names;
        container.Q<DropdownField>("presetDropdown").value = names[0];

        ApplyPreset(mapPresets[0].mapSettings);
    }

    private void RefreshMapUI(MapSettings map)
    {
        isApplyingPreset = true;

        //Map Size
        container.Q<IntegerField>("widthField").SetValueWithoutNotify(map.width);
        container.Q<IntegerField>("heightField").SetValueWithoutNotify(map.height);

        //Lakes
        container.Q<IntegerField>("lakeCount").SetValueWithoutNotify(map.lakeCount);
        container.Q<IntegerField>("lakeRadius").SetValueWithoutNotify(map.lakeRadius);
        container.Q<Slider>("irregularity").SetValueWithoutNotify(map.irregularity);
        container.Q<Label>("irregularityValue").text = map.irregularity.ToString("0.00");

        //Plantables-Tree
        container.Q<IntegerField>("treeMax").SetValueWithoutNotify(map.tree.maxCount);
        container.Q<Slider>("treeChance").SetValueWithoutNotify(map.tree.spawnChance);
        container.Q<Label>("treeChanceValue").text = map.tree.spawnChance.ToString("0.00");

        //Plantables-Bush
        container.Q<IntegerField>("bushMax").SetValueWithoutNotify(map.bush.maxCount);
        container.Q<Slider>("bushChance").SetValueWithoutNotify(map.bush.spawnChance);
        container.Q<Label>("bushChanceValue").text = map.bush.spawnChance.ToString("0.00");

        isApplyingPreset = false;
    }

    private void AssingMapSettingCallbacks(MapSettings map)
    {
        //Callbacks
        //MapSize
        container.Q<IntegerField>("widthField").RegisterValueChangedCallback(e =>
        {
            map.width = e.newValue;
            OnUserEdited();
            ValidateUI();
        });

        container.Q<IntegerField>("heightField").RegisterValueChangedCallback(e =>
        {
            map.height = e.newValue;
            OnUserEdited();
            ValidateUI();
        });

        //Lake
        container.Q<IntegerField>("lakeCount").RegisterValueChangedCallback(e =>
        {
            map.lakeCount = e.newValue;
            OnUserEdited();
            ValidateUI();
        });

        container.Q<IntegerField>("lakeRadius").RegisterValueChangedCallback(e =>
        {
            map.lakeRadius = e.newValue;
            OnUserEdited();
            ValidateUI();
        });

        container.Q<Slider>("irregularity").RegisterValueChangedCallback(e =>
        {
            map.irregularity = e.newValue;
            container.Q<Label>("irregularityValue").text = e.newValue.ToString("0.00");
            OnUserEdited();
            ValidateUI();
        });

        //Plantables-Tree
        container.Q<IntegerField>("treeMax").RegisterValueChangedCallback(e =>
        {
            map.tree.maxCount = e.newValue;
            OnUserEdited();
            ValidateUI();
        });

        container.Q<Slider>("treeChance").RegisterValueChangedCallback(e =>
        {
            map.tree.spawnChance = e.newValue;
            container.Q<Label>("treeChanceValue").text = e.newValue.ToString("0.00");
            OnUserEdited();
            ValidateUI();
        });

        //Plantables-Bush
        container.Q<IntegerField>("bushMax").RegisterValueChangedCallback(e =>
        {
            map.bush.maxCount = e.newValue;
            OnUserEdited();
            ValidateUI();
        });

        container.Q<Slider>("bushChance").RegisterValueChangedCallback(e =>
        {
            map.bush.spawnChance = e.newValue;
            container.Q<Label>("bushChanceValue").text = e.newValue.ToString("0.00");
            OnUserEdited();
            ValidateUI();
        });

        //Preset
        container.Q<DropdownField>("presetDropdown").RegisterValueChangedCallback(e =>
        {
            if (isApplyingPreset) return;
            if (e.newValue == "Custom") return;

            var preset = mapPresets.Find(p => p.presetName == e.newValue);
            container.Q<DropdownField>("presetDropdown").SetValueWithoutNotify(preset.presetName);
            ApplyPreset(preset.mapSettings);
        });
    }

    private void OnUserEdited()
    {
        if (isApplyingPreset) return;
        container.Q<DropdownField>("presetDropdown").SetValueWithoutNotify("Custom");
    }

    #endregion

    #region Validation

    private void ValidateUI()
    {
        if(MapSettingsValidator.Validate(runtimeConfig.mapSettings, out var errors))
        {
            mainMenuButton.SetEnabled(true);
            currentValidationMsg = "";
            tooltipLabel.style.visibility = Visibility.Hidden;
        }
        else
        {
            mainMenuButton.SetEnabled(false);
            currentValidationMsg = errors[0];
            tooltipLabel.text = currentValidationMsg;
        }
    }
   
    #endregion
}
