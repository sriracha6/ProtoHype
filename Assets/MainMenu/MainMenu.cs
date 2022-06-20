using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum Difficulty
{
    Hard, Medium, Easy
}
public class MainMenu : MonoBehaviour
{
    VisualElement root;

    void Start()
    {
        root = Menus.I.mainMenu.rootVisualElement;
        root.Q<VisualElement>("PlayButton").RegisterCallback<MouseDownEvent>(Play);
    }

    private void Play(MouseDownEvent v) =>
        Menus.I.SwitchTo(Menus.I.start);
}
