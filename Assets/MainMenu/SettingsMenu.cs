using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
//using static UnityEngine.Rendering.DebugUI;
using System;
using System.Xml;
using Countries;
using static CachedItems;

public partial class SettingsMenu : MonoBehaviour
{
    [SerializeField] UIDocument uidoc;
    [SerializeField] VisualTreeAsset keyrebinder;
    [SerializeField] VisualTreeAsset divider;
    VisualElement root;

    public static SettingsMenu I;

    VisualElement general;
    VisualElement controls;
    VisualElement video;
    new VisualElement audio;

    bool alreadyRebinding = false;
    System.Reflection.FieldInfo rebindingField;
    Button rebindingButton;

    List<(Button tab, VisualElement content)> content = new List<(Button tab, VisualElement content)>();

    protected void Awake()
    {
        if (I == null)
            I = this;

        root = uidoc.rootVisualElement;
        root.Q<Button>("BackButton").clicked += delegate { Menus.I.SwitchTo(Menus.I.mainMenu); };
        //root.Q<Button>("ResetToDefault").clicked += ResetDefault;

        List<VisualElement> cs = new List<VisualElement>();
        foreach (VisualElement v in root.Q<VisualElement>("TabContent").Children())
            cs.Add(v);

        foreach (VisualElement v in root.Q<VisualElement>("Tabs").Children())
        {
            Button b = v as Button;
            VisualElement tabcontent = cs.Find(x => x.name == v.name);
            b.clicked += delegate
            {
                foreach (VisualElement v in cs) v.style.display = DisplayStyle.None;
                tabcontent.style.display = DisplayStyle.Flex;
            };
            content.Add((b, tabcontent));
        }

        general = content[0].content.Q<VisualElement>("Body");
        controls = content[1].content.Q<VisualElement>("Body");
        video = content[2].content.Q<VisualElement>("Body");
        audio = content[3].content.Q<VisualElement>("Body");

        root.Q<Button>("Save").clicked += SaveSettings;
        root.Q<Button>("Reset").clicked += ResetDefault;
    }


    protected void Start()
    {
        List<string> screensizes = new List<string>();
        foreach (Resolution r in Screen.resolutions) screensizes.Add(r.ToString());

        general.Q<SliderInt>("RegimentSliderStep").RegisterValueChangedCallback(delegate { Settings.SliderStep = general.Q<SliderInt>("RegimentSliderStep").value; });
        general.Q<Toggle>("RunAndGun").RegisterValueChangedCallback(delegate { Settings.RunAndGunDefaultState = general.Q<Toggle>("RunAndGun").value; });
        general.Q<Toggle>("SpawnBlood").RegisterValueChangedCallback(delegate { Settings.SpawnBlood = general.Q<Toggle>("SpawnBlood").value; });
        general.Q<Toggle>("ScreenShake").RegisterValueChangedCallback(delegate { Settings.EnableScreenshake = general.Q<Toggle>("ScreenShake").value; });
        general.Q<DropdownField>("RegimentSortOrder").RegisterValueChangedCallback(delegate { Settings.RegimentSortOrder = general.Q<DropdownField>("RegimentSortOrder").value.StringToEnum<RegimentSortOrder>(); });
        general.Q<Toggle>("MasterMode").RegisterValueChangedCallback(delegate { Settings.MasterMode = general.Q<Toggle>("MasterMode").value; });
        general.Q<Toggle>("DeveloperMode").RegisterValueChangedCallback(delegate { Settings.DeveloperMode = general.Q<Toggle>("DeveloperMode").value; });
        general.Q<SliderInt>("HUDOpacity").RegisterValueChangedCallback(delegate { Settings.HUDOpacity01 = general.Q<SliderInt>("HUDOpacity").value / 100f; });
        general.Q<Toggle>("ShowFPS").RegisterValueChangedCallback(delegate { Settings.ShowFPS = general.Q<Toggle>("ShowFPS").value; });

        video.Q<DropdownField>("Resolution").choices = screensizes;
        video.Q<DropdownField>("Resolution").RegisterValueChangedCallback(delegate { ChangeResolution(video.Q<DropdownField>("Resolution").value); });
        video.Q<DropdownField>("WindowMode").RegisterValueChangedCallback(delegate { ChangeWindowMode(); });
        video.Q<Toggle>("VSync").RegisterValueChangedCallback(delegate { QualitySettings.vSyncCount = video.Q<Toggle>("VSync").value ? 1 : 0; });
        video.Q<TextField>("MaxFPS").RegisterValueChangedCallback(delegate { ChangeTargetFPS(video.Q<TextField>("MaxFPS").value); });
        video.Q<DropdownField>("QualityLevel").RegisterValueChangedCallback(delegate { SetQuality(video.Q<DropdownField>("QualityLevel").value); });
        video.Q<DropdownField>("AntiAliasing").RegisterValueChangedCallback(delegate { ChangeAntiAliasing(video.Q<DropdownField>("AntiAliasing").value); });

        controls.Q<DropdownField>("LeftMouse").RegisterValueChangedCallback(delegate { Keybinds.LeftMouse = ParseMouseRebind(controls.Q<DropdownField>("LeftMouse").value); });
        controls.Q<DropdownField>("RightMouse").RegisterValueChangedCallback(delegate { Keybinds.RightMouse = ParseMouseRebind(controls.Q<DropdownField>("RightMouse").value); });
        controls.Q<DropdownField>("MiddleMouse").RegisterValueChangedCallback(delegate { Keybinds.MiddleMouse = ParseMouseRebind(controls.Q<DropdownField>("MiddleMouse").value); });
        PopulateKeyRebinds();

        audio.Q<SliderInt>("Volume").RegisterValueChangedCallback(delegate { SFXManager.Volume01 = audio.Q<SliderInt>("Volume").value / 100f; });

        if (System.IO.File.Exists(Application.persistentDataPath + "\\settings.xml"))
            LoadSettings(Application.persistentDataPath + "\\settings.xml");
    }

    void PopulateKeyRebinds()
    {
        foreach (var p in typeof(Keybinds).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static))
            if(p.GetType() == typeof(KeyCode))
                foreach (var a in p.GetCustomAttributes(true))
                    if (a.GetType() == typeof(KeybindAttribute))
                    {
                        var ele = keyrebinder.CloneTree().contentContainer;
                        ele.name = p.Name;
                        ele.Q<Label>("Label").text = a.GetType().GetField("Text").GetValue(a).ToString();
                        ele.Q<Button>("Button").text = Convert.ChangeType(p, Enum.GetUnderlyingType(typeof(KeyCode))).ToString();
                        ele.Q<Button>("Button").clicked += delegate
                        {
                            alreadyRebinding = true;
                            rebindingField = p;
                            rebindingButton = ele.Q<Button>("Button");
                        };
                        controls.Add(ele);
                    }
                    else if (a.GetType() == typeof(SettingDivider))
                    {
                        controls.Add(divider.CloneTree());
                    }
    }

    int ParseMouseRebind(string value)
    {
        switch(value)
        {
            case "Left Mouse": return 0;
            case "Right Mouse": return 1;
            case "Middle Mouse": return 2;
        }
        return 69;
    }

    protected void Update()
    {
        if (!alreadyRebinding) return;
        if(Event.current.keyCode != KeyCode.None)
        {
            rebindingField.SetValue(rebindingField, Event.current.keyCode);
            rebindingField = null;
            rebindingButton.text = Event.current.keyCode.ToString();
            alreadyRebinding = false;
        }
    }
    // test
    public void ResetDefault()
    {
        if(System.IO.File.Exists(Application.persistentDataPath+"\\default.xml")) LoadSettings(Application.persistentDataPath+"\\default.xml");
    }

    void ChangeResolution(string value)
    {
        Resolution r = Screen.resolutions.ToList().Find(x=>x.ToString() == value);
        Screen.SetResolution(r.width, r.height, Screen.fullScreen, r.refreshRate);
    }

    void ChangeWindowMode()
    {
        FullScreenMode mode = FullScreenMode.FullScreenWindow;
        switch(video.Q<DropdownField>("WindowMode").value)
        {
            case "Fullscreen": mode = FullScreenMode.FullScreenWindow; break;
            case "Borderless": mode = FullScreenMode.ExclusiveFullScreen; break;
            case "Windowed": mode = FullScreenMode.Windowed; break;
        }
        Screen.fullScreenMode = mode;
    }

    void ChangeTargetFPS(string fps)
    {
        if (!int.TryParse(fps, out int fpsint)) return;
        Application.targetFrameRate = fpsint;
    }

    void SetQuality(string text)
    {
        if (text == "Sexy") QualitySettings.SetQualityLevel(3);
        if (text == "Medium") QualitySettings.SetQualityLevel(2);
        if (text == "Low") QualitySettings.SetQualityLevel(1);
    }

    void ChangeAntiAliasing(string text)
    {
        int value = 0;
        if (text == "None") value = 0;
        else if (text == "2x") value = 2;
        else if (text == "4x") value = 4;
        else if (text == "8x") value = 8;

        QualitySettings.antiAliasing = value;
    }
}
