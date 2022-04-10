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

    void Update()
    {
        if (Input.GetMouseButtonUp(0))// && !CameraMove.IsPointerOverUIObject()) // todo: keybinds
        {
            Vector2 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Linecast(mousePos, Vector3.one);
            if (hit.transform.gameObject.TryGetComponent(out Pawn p) && !UIManager.mouseOverUI)
            {
                pInfo.ShowPawnInfo(p);
            }
        }
    }
}
