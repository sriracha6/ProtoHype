using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static MapGenerator;
using Buildings;
using Nature;

public class TilemapPlace : MonoBehaviour
{
    public static TilemapPlace I;
    public static TerrainType[,] tilemap;
    public static Build[,] buildings;
    public static Floor[,] floors;

    [SerializeField] AstarPath pfinder;
    [SerializeField] Transform treeParent;

    protected void Awake()
    {
        if(I == null)
            I = this;
        else
        {
            I.pfinder = pfinder;
            I.treeParent = treeParent;
        }
    }

    public static void SetFloor(Floor f, int x, int y)
    {
        floors[x,y] = f;
        WCMngr.I.groundTilemap.SetTile(new Vector3Int(x,y,0), f.tile);
    }    

    public static void SetBuild(Build f, int x, int y)
    {
        DB.Null(buildings);
        DB.Null(f);
        buildings[x,y] = f;
        WCMngr.I.solidTilemap.SetTile(new Vector3Int(x, y, 0), f.tile);
    }

    public static void SetDoor(Door d, int x, int y, float rotation)
    {
        SetBuild(d, x, y);
        Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f,0f,rotation), Vector3.one);
        WCMngr.I.solidTilemap.SetTransformMatrix(new Vector3Int(x,y,0), matrix);
    }

    public static int GetNumberOfTiles(Tilemap tilemap)
    {
        tilemap.CompressBounds();
        TileBase[] tiles = tilemap.GetTilesBlock(tilemap.cellBounds);
        return tiles.Where(x => x != null).ToArray().Length;
    }

    public static void UpdateTilemap(float[,] noiseMap, TerrainType[] tTypesUnsorted, bool place, Biome biome)
    {
        TerrainType[] tTypes = tTypesUnsorted.OrderBy(x => x.height).ToArray(); // this line of code makes the entire game good, i'll make it a puzzle! figure out why! ;)
        
        tilemap = new TerrainType[noiseMap.GetLength(0), noiseMap.GetLength(1)];
        for (int x = 0; x < noiseMap.GetLength(0); x++)
        {
            for (int y = 0; y < noiseMap.GetLength(1); y++)
            {
                for (int i = 0; i < tTypes.Length; i++)
                {
                    if (noiseMap[x, y] <= tTypes[i].height)
                    {
                        /* no*/switch (tTypes[i].type)
                        {
                            case SpecialType.None:
                                TerrainType ttype = tTypes[i];
                                if(place)
                                    WCMngr.I.groundTilemap.SetTile(new Vector3Int(x, y, 0), tTypes[i].tile);
                                tilemap[x, y] = tTypes[i];
                                break;
                            case SpecialType.Mountain:
                                if(place)
                                    WCMngr.I.solidTilemap.SetTile(new Vector3Int(x, y, 0), tTypes[i].thisIsVeryBadSpaghettiButImOutOfIdeas);
                                tilemap[x, y] = tTypes[i];
                                break;
                            case SpecialType.Water:
                                tilemap[x, y] = tTypes[i];
                                break;
                            default:
                                Debug.Log($"WTF?");
                                break;
                        }
                        break;
                    }
                }
            }
        }
        buildings = new Build[MapGenerator.I.mapWidth, MapGenerator.I.mapHeight];
        floors = new Floor[MapGenerator.I.mapWidth, MapGenerator.I.mapHeight];

        if(place)
        {
            WCMngr.I.solidTilemap.RefreshAllTiles();
            WCMngr.I.groundTilemap.RefreshAllTiles();
            I.pfinder.Scan();
        }
    }

    public static void DestroyBuilding (Vector2 position)
    {
        buildings[(int)position.x, (int)position.y] = null;
        WCMngr.I.groundTilemap.SetTile(Vector3Int.FloorToInt(position), null);
        // TODO : RUBBLE EFFECT
    }

/*    public void placeTrees(List<Vector3Int> points, List<Buildings.Nature> flora,System.Random rand, GameObject treeFab)
    {
        // --- generate trees to place at the points
        Buildings.Nature[] choices = flora.ToArray();
        Buildings.Nature[] treeAtPoint = new Buildings.Nature[points.Count];

        for(int i = 0; i<points.Count;i++)
        {
            treeAtPoint[i] = choices[rand.Next(0,choices.Length)];
        }

        int j = 0;

        foreach(Transform t in treeParent.transform)
        {
            Destroy(t.gameObject);
        }

        foreach(Vector3Int point in points)       // TODO: object pooling
        {
            try
            {
                if (tilemap[point.x, point.y].supportsNature && tilemap[point.x, point.y].type!=SpecialType.Mountain) // not water or solid & supports nature
                {
                    var x = Instantiate(treeFab, Instance.treeParent);
                    var PENIS = treeAtPoint[j].sprites[rand.Next(0, treeAtPoint[j].sprites.Count)];
                    x.GetComponent<SpriteRenderer>().sprite = PENIS;
                    x.transform.position = point;
                }
                j++;
            }
            catch(System.NullReferenceException e)
            {
                Debug.DrawLine(Vector2.zero, new Vector2(point.x,point.y), Color.magenta, 10f);
                break;
            }
        }
    }*/
}
