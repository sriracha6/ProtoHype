using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoofPlacer : MonoBehaviour
{
    [SerializeField] Tilemap roofTmap;
    [SerializeField] TileBase invisibleTile;

    public void placeRooves(List<Vector2Int> positions)
    {
        foreach(Vector3Int p in positions)// simple explanation : we can intercept the shadows if we place this tmap in between the other two
        {
            roofTmap.SetTile(p,invisibleTile); // set tile a billion times is slow. too bad!
        }
    }
}
