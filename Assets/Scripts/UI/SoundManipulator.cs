using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SoundManipulator : Manipulator
{

    public SoundManipulator()
    {
    }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<MouseDownEvent>(delegate { MouseDown(); });
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<MouseDownEvent>(delegate { MouseDown(); });
    }

    void MouseDown()
    {
        SFXManager.I.PlaySound(UIManager.UISounds.randomElement().name, "UI", 1, Vector2.zero, true);
    }
}
