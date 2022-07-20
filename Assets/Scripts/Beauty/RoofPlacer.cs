using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoofPlacer : MonoBehaviour
{
    [SerializeField] Tilemap roofTmap;
    [SerializeField] TileBase invisibleTile;

    public Buildings.Roof[,] rooves;

    public static RoofPlacer I;

    protected void Awake()
    {
        if(I == null)
            I = this;
        else
            I.roofTmap = roofTmap;
    }

    protected void Update()
    {
        if (Input.GetKeyDown(Keybinds.showRooves))
        {
            Player.isRoofShow = !Player.isRoofShow;
            for (int i = 0; i < I.rooves.GetLength(0); i++)
            {
                for(int j = 0; j < I.rooves.GetLength(1); j++)
                {
                    if (Player.isRoofShow)
                        roofTmap.SetTile(new Vector3Int(i,j,0), rooves[i,j].tile);
                    else
                        roofTmap.SetTile(new Vector3Int(i, j, 0), null);
                }
            }
        }
    }

    public void Setup(int width, int height)
    {
        rooves = new Buildings.Roof[width, height];
    }

    public void PlaceRoof(Buildings.Roof r, int x, int y)
    {
        try
        {
            rooves[x, y] = r;
            if (MapGenerator.I.drawMode == MapGenerator.DrawMode.Place)
                roofTmap.SetTile(new Vector3Int(x, y, 0), r.tile);
        }
        catch(System.Exception)
        {
            Debug.Log($"ROOF ERROR @ {x},{y}");
        }
    }
}
