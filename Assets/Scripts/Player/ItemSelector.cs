using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using static TilemapPlace;

public class ItemSelector : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    List<Build[,]> validClickables = new List<Build[,]>() {buildings};

    protected void Start()
    {
        spriteRenderer.forceRenderingOff = true;
        if (doors == null)
            return;
        Build[,] builds = new Build[doors.GetLength(0), doors.GetLength(1)];
        for(int i = 0; i < doors.GetLength(0); i++)
            for(int j = 0; j < doors.GetLength(1); j++)
                builds[i, j] = doors[i, j].door;

        validClickables.Add(builds); // DOES THIS UPDATE WHEN YOu CHANGE DOORS?? AHH
    }

    protected void Update()
    {
        if(Input.GetMouseButtonUp(Keybinds.LeftMouse) && !UIManager.mouseOverUI)
        {
            spriteRenderer.forceRenderingOff = false;
            var mousePos = WCMngr.I.groundTilemap.WorldToCell(Input.mousePosition);
            object item = null;
            foreach (var build in validClickables)
            {
                if (build != null && build[mousePos.x, mousePos.y] != null)
                    item = build[mousePos.x, mousePos.y];
            }
            transform.position = mousePos;
            // animation ...
            if(item != null)
                ItemViewer.I.DisplayItem(item);
            else
                spriteRenderer.forceRenderingOff = true;
        }
        if(Input.GetMouseButtonDown(Keybinds.RightMouse) || Input.GetMouseButtonDown(Keybinds.MiddleMouse) || UIManager.mouseOverUI)
            spriteRenderer.forceRenderingOff = true;
    }
}