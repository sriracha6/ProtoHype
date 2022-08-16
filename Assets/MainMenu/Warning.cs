using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warning : MonoBehaviour
{
    protected void Start() =>
        StartCoroutine(AutoChange());

    bool flag = false;
    bool done = false;

    protected void Update()
    {
        if(Input.GetKeyUp(Keybinds.Escape) && !done)
        {
            Menus.I.SwitchTo(Menus.I.mainMenu, null);
            flag = true;
            done = true;
        }
    }

    IEnumerator AutoChange()
    {
        yield return new WaitForSecondsRealtime(4);
        if(!flag) Menus.I.SwitchTo(Menus.I.mainMenu, null);
        done = true;
    }
}
