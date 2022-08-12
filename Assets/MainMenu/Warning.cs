using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warning : MonoBehaviour
{
    protected void Start() =>
        StartCoroutine(AutoChange());

    bool flag = false;

    protected void Update()
    {
        if(Input.GetKeyUp(Keybinds.Escape))
        {
            Menus.I.SwitchTo(Menus.I.mainMenu);
            flag = true;
        }
    }

    IEnumerator AutoChange()
    {
        yield return new WaitForSecondsRealtime(4);
        if(!flag) Menus.I.SwitchTo(Menus.I.mainMenu);
    }
}
