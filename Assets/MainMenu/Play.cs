using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Play : MonoBehaviour
{
    void Start()
    {
        var root = Menus.I.start.rootVisualElement;
        root.Q<Button>("BackButton").clicked += Back;
        root.Q<Button>("QuickBattle").clicked += QuickBattle;
    }

    private void Back() =>
        Menus.I.SwitchTo(Menus.I.mainMenu);

    private void QuickBattle() =>
        Menus.I.SwitchTo(Menus.I.quickstart);
}
