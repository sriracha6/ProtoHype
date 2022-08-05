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
        if (Input.GetMouseButtonUp(Keybinds.LeftMouse))
        {
            var find = PawnManager.GetAll().Find(p => p.thisPawnMouseOver);
            if (find != null)
                pInfo.ShowPawnInfo(find);
            /*Vector2 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Raycast(mousePos, mousePos);
            //var ray = mainCam.ScreenPointToRay(Input.mousePosition);
            //RaycastHit hit;
            Debug.DrawLine(mousePos, hit.point, Color.magenta, 5, false);
            //if(hit.transform == null)
            //    Debug.Log($"big fat penis");
            //if(hit.transform.gameObject.TryGetComponent(out Pawn enis))
            //    Debug.Log($"bigger, fatter, larger, penis");
            //if (Physics2D.Raycast(ray, out hit))
            //{
                if (hit.transform != null && hit.transform.gameObject.TryGetComponent(out Pawn p) && !UIManager.mouseOverUI)
                {
                    pInfo.ShowPawnInfo(p);
                }
            //}*/
            
        }
    }
}
