using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public static float Rotation;

    protected void Update()
    {
        if(Input.GetKeyDown(Keybinds.rotate))
        {
            if (Rotation <= 180) Rotation += 90;
            else                 Rotation = 0;
        }
    }
}
