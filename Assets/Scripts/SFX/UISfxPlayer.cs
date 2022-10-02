using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISfxPlayer : MonoBehaviour
{
    protected void Update()
    {
        if(Input.GetMouseButtonDown(Keybinds.LeftMouse) || Input.GetMouseButtonDown(Keybinds.RightMouse) || Input.GetMouseButtonDown(Keybinds.MiddleMouse))
        {
            if (UIManager.mouseOverUI)
                UIManager.I.PlayUISound();
        }
    }
}
