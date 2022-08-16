using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Play : MonoBehaviour, IMenu
{
    public static Play I;

    protected void Start()
    {
        I = this;
        var root = Menus.I.start.rootVisualElement;
        root.Q<Button>("BackButton").clicked += Back;
        root.Q<Button>("QuickBattle").clicked += QuickBattl;
    }

    public void Back() =>
        Menus.I.SwitchTo(Menus.I.mainMenu, null);

    private void QuickBattl() =>
        Menus.I.SwitchTo(Menus.I.quickstart, QuickBattle.I);
}
