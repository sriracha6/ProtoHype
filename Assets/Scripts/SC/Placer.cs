using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TargetLayer { Solid, Water, Terrain, Roof }
public class Placer : MonoBehaviour
{
    public static Placer I;

    protected void Awake() => I = this;

    public static TileBase PlacedItem;
    public static TargetLayer targetLayer;

    protected void Update()
    {
        if (Input.GetMouseButtonDown(Keybinds.LeftMouse) && !UIManager.mouseOverUI)
        {
            Vector3Int pos;
            var p = WCMngr.I.mainCam.ScreenToWorldPoint(Input.mousePosition) / 2;
            pos = new Vector3Int((int)p.x, (int)p.y, 0);
            if (targetLayer == TargetLayer.Solid) WCMngr.I.solidTilemap.SetTile(pos, PlacedItem);
            if (targetLayer == TargetLayer.Roof) WCMngr.I.roofTilemap.SetTile(pos, PlacedItem);
            if (targetLayer == TargetLayer.Terrain) WCMngr.I.groundTilemap.SetTile(pos, PlacedItem);
            if (targetLayer == TargetLayer.Water)
            {
                WCMngr.I.groundTilemap.SetTile(pos, null);
                WCMngr.I.solidTilemap.SetTile(pos, null);
            }
        }
    }
}
