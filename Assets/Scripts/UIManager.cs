using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// todo: function to add elements to the various UI parts
/// </summary>
public class UIManager : MonoBehaviour
{
    public UIDocument __ui;
    public static UIDocument ui; // this lne sucks

    public static bool mouseOverUI { get; private set; }

    private void mouseEnter(VisualElement v)
    {
        if(v.style.display == DisplayStyle.Flex)
            mouseOverUI = true;
    }

    private void Start()
    {
        ui = __ui;
        var root = ui.rootVisualElement;
        foreach(VisualElement v in root.Children())
        {
            v.AddManipulator(new ToolTipManipulator());
            v.RegisterCallback<MouseEnterEvent>(x=>mouseEnter(v)); // worked
            v.RegisterCallback<MouseLeaveEvent>(x => mouseOverUI = false);
        }
    }
}

