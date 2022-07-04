using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;

public class ContextMenuItem
{
    public System.Action onClick;
    public string ItemName;

    public ContextMenuItem(string ItemName, System.Action onClick)
    {
        this.ItemName = ItemName;
        this.onClick = onClick;
    }
}

public class ContextMenuManipulator : Manipulator
{
    List<ContextMenuItem> menuItems;
    bool UI;

    public ContextMenuManipulator(List<ContextMenuItem> menuItems, bool UI)
    {
        this.UI = UI;
        if (!UI)
            ContextMenuShower.I.watchOut = true;
        this.menuItems = menuItems;
    }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<MouseUpEvent>(MouseUp, TrickleDown.TrickleDown);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<MouseUpEvent>(MouseUp);
    }

    private void MouseUp(MouseUpEvent evt)
    {
        if (evt.button != Keybinds.RightMouse)
            return;
        ContextMenuShower.I.ClearItems();
        foreach (ContextMenuItem item in menuItems)
            ContextMenuShower.I.AddButton(item);
        ContextMenuShower.I.Show(evt, UI);
    }
}