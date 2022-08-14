using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeveloperMode : MonoBehaviour
{
    public static DeveloperMode I;
    protected void Awake()
    {
        I = this;
    }

    protected void OnGUI()
    {
        if(Settings.ShowFPS) GUI.Label(new Rect(25, 25, 100, 30), (1.0f/Time.deltaTime).ToString());
    }
}
