using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static TilemapPlace;

public class RoofPlacer : MonoBehaviour
{
    [SerializeField] internal Tilemap roofTmap;
    [SerializeField] TileBase invisibleTile;

    public static RoofPlacer I;

    protected void Awake()
    {
        if (I == null)
            I = this;
    }

    protected void Update()
    {
        if (Input.GetKeyDown(Keybinds.showRooves))
        {
            Player.isRoofShow = !Player.isRoofShow;
            Refresh();
        }
    }

    public void Refresh()
    {
        for (int i = 0; i < rooves.GetLength(0); i++)
        {
            for (int j = 0; j < rooves.GetLength(1); j++)
            {
                if (Player.isRoofShow)
                    roofTmap.SetTile(new Vector3Int(i, j, 0), rooves[i,j] == null ? null : rooves[i, j].tile);
                else
                { 
                    if (invisibleTile == null) DB.Attention("penis alert");
                    else roofTmap.SetTile(new Vector3Int(i, j, 0), invisibleTile);
                }
            }
        }
    }

    public void PlaceRoof(Buildings.Roof r, int x, int y)
    {
        try
        {
            rooves[x, y] = r;
            if (MapGenerator.I.drawMode == MapGenerator.DrawMode.Place && Player.isRoofShow)
                roofTmap.SetTile(new Vector3Int(x, y, 0), r.tile);
        }
        catch(System.Exception)
        {
            Debug.Log($"ROOF ERROR @ {x},{y}");
        }
    }
}
