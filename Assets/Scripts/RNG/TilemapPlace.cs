using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static MapGenerator;
using Buildings;

public class TilemapPlace : MonoBehaviour
{
    public static TilemapPlace Instance;
    public static TerrainType[,] tilemap;
    [SerializeField] AstarPath pfinder;
    [SerializeField] Transform treeParent;

    protected void Awake()
    {
        Instance = this;
    }

    public static int GetNumberOfTiles(Tilemap tilemap)
    {
        tilemap.CompressBounds();
        TileBase[] tiles = tilemap.GetTilesBlock(tilemap.cellBounds);
        return tiles.Where(x => x != null).ToArray().Length;
    }

    private static void resetAllTmaps()
    {
        GameManager2D.Instance.groundTilemap.ClearAllTiles();
        GameManager2D.Instance.solidTilemap.ClearAllTiles();
    }

    public static void placeTiles(float[,] noiseMap, TerrainType[] tTypes) 
    {                                                                     
        //resetAllTmaps();
        tilemap = new TerrainType[noiseMap.GetLength(0), noiseMap.GetLength(1)];

        for(int x = 0; x < noiseMap.GetLength(0); x++)
        {
            for(int y = 0; y < noiseMap.GetLength(1); y++)
            {
                for(int i = 0; i < tTypes.Length; i++)
                {
                    if(noiseMap[x,y] <= tTypes[i].height)
                    {
                        switch(tTypes[i].type)
                        {
                            case SpecialType.None:
                                GameManager2D.Instance.groundTilemap.SetTile(new Vector3Int(x, y, 0), tTypes[i].tbase);
                                tilemap[x,y] = tTypes[i];
                                break;
                            case SpecialType.Solid:
                                GameManager2D.Instance.solidTilemap.SetTile(new Vector3Int(x,y,0), tTypes[i].tbase);
                                tilemap[x,y] = tTypes[i];
                                break;
                            case SpecialType.Water:
                                tilemap[x, y] = tTypes[i];
                                break;
                        }
                        break;
                    }
                }
            }
        }

        Instance.pfinder.Scan();
    }

    public void placeTrees(List<Vector3Int> points, List<Buildings.Nature> flora,System.Random rand, GameObject treeFab)
    {
        // --- generate trees to place at the points --- //
        Buildings.Nature[] choices = flora.ToArray();
        Buildings.Nature[] treeAtPoint = new Buildings.Nature[points.Count];

        for(int i = 0; i<points.Count;i++)
        {
            treeAtPoint[i] = choices[rand.Next(0,choices.Length)];
        }

        // --- place --- //
        // I NEED A ARM AND A LEG
        // CARTI IN LOVE WITH THE BREAD
        // IN 2020 THERE WILL BE A GLOBAL PANDEMIC THAT WILL SHUT DOWN THE WORLD
        int j = 0;

        foreach(Transform t in treeParent.transform)
        {
            Destroy(t.gameObject);
        }

        foreach(Vector3Int point in points)       // TODO: object pooling
        {
            //Player.tmap.GetTile(point) != null &&
            //Debug.Log(tilemap[point.x,point.y
            if (tilemap[point.x, point.y].supportsNature) // not water or solid & supports nature
            {
                //GameManager2D.Instance.solidTilemap.SetTile(point, treeAtPoint[j].tbase);
                //Debug.Log(treeAtPoint[j].Name);
                var x = Instantiate(treeFab, Instance.treeParent);
                var PENIS = treeAtPoint[j].sprites[rand.Next(0, treeAtPoint[j].sprites.Count)];
                x.GetComponent<SpriteRenderer>().sprite = PENIS;
                x.transform.position = point;
            }
            j++;
        }
    }
}
