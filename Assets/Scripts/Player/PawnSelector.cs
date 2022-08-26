using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PawnFunctions;
using UnityEngine.UIElements;

public class PawnSelector : MonoBehaviour
{
    [SerializeField]
    PawnInfo pInfo;

    [SerializeField]
    Camera mainCam;

    protected void Update()
    {
        if (Input.GetMouseButtonUp(Keybinds.LeftMouse) && !UIManager.mouseOverUI)
        {
            var find = PawnManager.GetAll().Find(p => p.thisPawnMouseOver);
            if (find != null)
                pInfo.ShowPawnInfo(find);
        }
    }
}
