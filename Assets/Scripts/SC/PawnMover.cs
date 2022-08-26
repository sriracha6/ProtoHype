using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PawnFunctions;

public class PawnMover : MonoBehaviour
{
    Vector2 previous;
    protected void Update()
    {
        if(Input.GetMouseButton(Keybinds.LeftMouse) && Pawn.mouseOverPawn && !UIManager.mouseOverUI && PawnAdder.MoveMode)
        {
            Vector2 pos = WCMngr.I.mainCam.ScreenToWorldPoint(Input.mousePosition);
            foreach (Pawn p in Player.selectedPawns)
                p.transform.position += (Vector3)(pos - previous);
            previous = pos;
        }
    }
}
