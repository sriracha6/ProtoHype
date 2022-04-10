using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// todo: function to add elements to the various UI parts
/// </summary>
public class UIManager : MonoBehaviour
{
    public UnityEngine.UIElements.UIDocument ui; // this lne sucks

    public static bool mouseOverUI;

    private void Start()
    {
        var root = ui.rootVisualElement;
        root.RegisterCallback<UnityEngine.UIElements.MouseEnterEvent>(x => mouseOverUI = true); // worked
        root.RegisterCallback<UnityEngine.UIElements.MouseLeaveEvent>(x => mouseOverUI = false);
    }
}

