using Generics;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UIElements;

public partial class SettingsMenu : MonoBehaviour
{
    public void SaveSettings()
    {
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Indent = true;
        settings.NewLineOnAttributes = true;

        using XmlWriter writer = XmlWriter.Create(Application.persistentDataPath + "\\settings.xml", settings);
        writer.WriteStartDocument();
        writer.WriteComment("Auto-Generated");
        writer.WriteStartElement("Settings");
        writer.WriteEl("SliderStep", Settings.SliderStep);
        writer.WriteEl("RunAndGunDefaultState", Settings.RunAndGunDefaultState);
        writer.WriteEl("SpawnBlood", Settings.SpawnBlood);
        writer.WriteEl("EnableScreenshake", Settings.EnableScreenshake);
        writer.WriteEl("RegimentSortOrder", (int)Settings.RegimentSortOrder);
        writer.WriteEl("MasterMode", Settings.MasterMode);
        writer.WriteEl("DeveloperMode", Settings.DeveloperMode);
        writer.WriteEl("HUDOpacity01", Settings.HUDOpacity01);
        writer.WriteEl("ShowFPS", Settings.ShowFPS);
        writer.WriteStartElement("Controls");
        writer.WriteEl("LeftMouse", controls.Q<DropdownField>("LeftMouse").value);
        writer.WriteEl("RightMouse", controls.Q<DropdownField>("RightMouse").value);
        writer.WriteEl("MiddleMouse", controls.Q<DropdownField>("MiddleMouse").value);
        foreach (var p in typeof(Keybinds).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static))
            if (p.GetType() == typeof(KeyCode))
                writer.WriteEl(p.Name, p.GetValue(p).ToString());
        writer.WriteEndElement();
        writer.WriteEl("MasterAudioLevel", SFXManager.Volume01);

        writer.WriteStartElement("Video");
        writer.WriteEl("Resolution", Screen.currentResolution);
        writer.WriteEl("WindowMode", video.Q<DropdownField>("WindowMode").value);
        writer.WriteEl("VSync", QualitySettings.vSyncCount > 0 ? "True" : "False");
        writer.WriteEl("MaxFPS", Application.targetFrameRate);
        writer.WriteEl("QualityLevel", video.Q<DropdownField>("QualityLevel").value);
        writer.WriteEl("AntiAliasing", video.Q<DropdownField>("AntiAliasing").text);
        writer.WriteEndElement();
        writer.WriteEndElement();
        writer.WriteEndDocument();
        writer.Close();
    }
    
    public void LoadSettings(string filepath)
    {
        XmlElement xmls = XMLLoader.Loaders.LoadXML(filepath);
        general.Q<SliderInt>("RegimentSliderStep").value = xmls.Q<int>("SliderStep");
        general.Q<Toggle>("RunAndGun").value = xmls.Q<bool>("RunAndGunDefaultState");
        general.Q<Toggle>("SpawnBlood").value = xmls.Q<bool>("SpawnBlood");
        general.Q<Toggle>("ScreenShake").value = xmls.Q<bool>("EnableScreenshake");
        general.Q<DropdownField>("RegimentSortOrder").value = general.Q<DropdownField>("RegimentSortOrder").choices[xmls.Q<int>("RegimentSortOrder")];
        general.Q<Toggle>("MasterMode").value = xmls.Q<bool>("MasterMode");
        general.Q<Toggle>("DeveloperMode").value = xmls.Q<bool>("DeveloperMode");
        general.Q<SliderInt>("HUDOpacity").value = (int)(xmls.Q<float>("HUDOpacity01") * 100);
        general.Q<Toggle>("ShowFPS").value = xmls.Q<bool>("ShowFPS");

        audio.Q<SliderInt>("Volume").value = (int)(xmls.Q<float>("MasterAudioLevel") * 100);

        video.Q<DropdownField>("Resolution").value = xmls.Q<string>("Resolution");
        video.Q<DropdownField>("WindowMode").value = xmls.Q<string>("WindowMode");
        video.Q<Toggle>("VSync").value = xmls.Q<bool>("VSync");
        video.Q<TextField>("MaxFPS").value = xmls.Q<int>("MaxFPS").ToString();
        video.Q<DropdownField>("QualityLevel").value = xmls.Q<string>("QualityLevel");
        video.Q<DropdownField>("AntiAliasing").value = xmls.Q<string>("AntiAliasing");

        foreach (XmlNode x in xmls.Qs("Controls"))
        {
            if (x.Name == "LeftMouse") controls.Q<DropdownField>("LeftMouse").value = x.InnerText;
            else if (x.Name == "RightMouse") controls.Q<DropdownField>("RightMouse").value = x.InnerText;
            else if (x.Name == "MiddleMouse") controls.Q<DropdownField>("MiddleMouse").value = x.InnerText;
            else
            {
                typeof(Keybinds).GetField(x.Name).SetValue(typeof(Keybinds), x.InnerText.StringToEnum<KeyCode>());
                controls.Q<Button>(x.Name).text = x.InnerText.StringToEnum<KeyCode>().ToString();
            }
        }
    }
}
