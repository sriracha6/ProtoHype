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
    public void Setup(int width, int height)
    {
        rooves = new Buildings.Roof[width, height];
    }

    public void PlaceRoof(Buildings.Roof r, int x, int y)
    {
        rooves[x, y] = r;
        if(MapGenerator.I.drawMode == MapGenerator.DrawMode.Place)
            roofTmap.SetTile(new Vector3Int(x, y, 0), r.tile);
    }
}
