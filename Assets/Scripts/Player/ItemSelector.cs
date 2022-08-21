using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using static TilemapPlace;

public class ItemSelector : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    readonly List<Build[,]> validClickables = new List<Build[,]>(); // todo: traps here, rooves, blah whatever else

    protected void Start()
    {
        spriteRenderer.forceRenderingOff = true;
        if (doors == null)
            return;

        validClickables.Add(doors);
        validClickables.Add(TilemapPlace.buildings);
    }

    protected void Update()
    {
        if(Input.GetMouseButtonUp(Keybinds.LeftMouse) && !UIManager.mouseOverUI && Placer.PlacedItem == null)
        {
            spriteRenderer.forceRenderingOff = false;
            var p = WCMngr.I.mainCam.ScreenToWorldPoint(Input.mousePosition) / 2;
            var mousePos = new Vector2Int((int)p.x, (int)p.y);
            mousePos = new Vector2Int(mousePos.x-1, mousePos.y-1);
            object item = null;

            foreach (Build[,] build in validClickables)
            {
                if (build[mousePos.x/2, mousePos.y/2] != null)
                    item = build[mousePos.x/2, mousePos.y/2];
            }
            transform.position = new Vector3(mousePos.x, mousePos.y);
            // todo: animation ...
            if (item != null)
                ItemViewer.I.DisplayItem(item);
            else
                spriteRenderer.forceRenderingOff = true;
        }
        if(Input.GetMouseButtonDown(Keybinds.RightMouse) || Input.GetMouseButtonDown(Keybinds.MiddleMouse) || UIManager.mouseOverUI)
            spriteRenderer.forceRenderingOff = true;
    }
}
