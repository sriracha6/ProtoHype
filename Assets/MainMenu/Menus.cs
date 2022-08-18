using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Menus : MonoBehaviour
{
    public static Menus I { get; private set; }
    public UIDocument mainMenu;
    public UIDocument start;
    public UIDocument quickstart;
    public UIDocument loading;
    public UIDocument warning;
    public UIDocument settings;
    public UIDocument scenarioCreator;

    public bool inBattle;
    public bool inSC;

    public UIDocument prefab_country;
    public static IMenu currentMenu;

    public void SwitchTo(UIDocument u, IMenu menu)
    {
        foreach (var c in GameObject.FindGameObjectsWithTag("Menu"))
        {
            var x = c.GetComponent<UIDocument>();
            if (x != u)
                x.rootVisualElement.style.display = DisplayStyle.None;
            else
                x.rootVisualElement.style.display = DisplayStyle.Flex;
        }
        currentMenu = menu;
        UIManager.ui = u;
    }

    protected void Awake()
    {
        if (I == null)
        {
            I = this;
            I.SwitchTo(I.warning, null);
        }
    }

    protected void Update()
    {
        if (Input.GetKey(Keybinds.Escape) && !SettingsMenu.IsRebinding && !I.inBattle && currentMenu != null)
            currentMenu.Back();
    }
}
