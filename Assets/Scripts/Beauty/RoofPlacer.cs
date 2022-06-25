using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoofPlacer : MonoBehaviour
{
    [SerializeField] Tilemap roofTmap;
    [SerializeField] TileBase invisibleTile;

    public static RoofPlacer I;

    protected void Awake() =>
        I = this;

    public void placeRooves(List<Vector2Int> positions)
    {
        foreach(Vector3Int p in positions)// simple explanation : we can intercept the shadows if we place this tmap in between the other two
        {
            roofTmap.SetTile(p,invisibleTile); // set tile a billion times is slow. too bad!
        }
    }

    public void PlaceRoof(Buildings.Roof r, int x, int y)
    {
        roofTmap.SetTile(new Vector3Int(x, y, 0), r.tile);
    }
}
