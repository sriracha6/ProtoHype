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

    public bool inBattle;

    public UIDocument prefab_country;

    public void SwitchTo(UIDocument u)
    {
        foreach (var c in GameObject.FindGameObjectsWithTag("Menu"))
        {
            var x = c.GetComponent<UIDocument>();
            if (x != u)
                x.rootVisualElement.style.display = DisplayStyle.None;
            else
                x.rootVisualElement.style.display = DisplayStyle.Flex;
        }
        UIManager.ui = u;
    }

    protected void Start()
    {
        if (I == null)
        {
            I = this;
            SwitchTo(mainMenu);
        }
    }
}
