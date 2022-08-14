using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum Difficulty
{
    Hard = 1, Medium = 0, Easy = 2
}
public class MainMenu : MonoBehaviour
{
    VisualElement root;

    protected void Start()
    {
        root = Menus.I.mainMenu.rootVisualElement;
        root.Q<VisualElement>("PlayButton").RegisterCallback<MouseDownEvent>(delegate { Menus.I.SwitchTo(Menus.I.start); });
        root.Q<VisualElement>("SettingsButton").RegisterCallback<MouseDownEvent>(delegate { Menus.I.SwitchTo(Menus.I.settings); });
    }
}
