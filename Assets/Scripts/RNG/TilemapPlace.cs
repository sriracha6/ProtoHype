using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static MapGenerator;
using Buildings;
using Nature;
using Pathfinding;
using PawnFunctions;

public class TilemapPlace : MonoBehaviour
{
    public static TilemapPlace I;
    public static TerrainType[,] tilemap;

    public static Building[,] buildings;
    public static Floor[,] floors;
    public static Trap[,] traps;
    public static Door[,] doors;
    public static Building[,] specials;
    public static Roof[,] rooves;
    public static Furniture[,] furnitures;
    public static List<(Furniture furn, Vector2Int pos)> furnitureInfo = new List<(Furniture, Vector2Int)>();

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
        if (MapGenerator.I.drawMode == DrawMode.Place)
            WCMngr.I.groundTilemap.SetTile(new Vector3Int(x,y,0), f.tile);
    }    

    public static List<Build> BuildsAt(int x, int y)
    {
        List<Build> list = new List<Build>();
        list.Add(buildings[x,y]);
        list.Add(floors[x,y]);
        list.Add(traps[x,y]);
        list.Add(doors[x,y]);
        list.Add(specials[x,y]);
        list.Add(furnitures[x,y]);
        list.Add(rooves[x,y]);
        return list;
    }

    public static bool AnyBuildingAt(int x, int y)
    {
        return !(floors[x, y] == null && doors[x, y] == null && buildings[x, y] == null && traps[x, y] == null && furnitures[x,y] == null);
    }

    public static void SetAll(List<Build> builds, int x, int y)
    {
        foreach(Build build in builds)
        {
            if (build is null) continue;
            if (build is Building) SetWall((Building)build, x, y);
            if (build is Floor) SetFloor((Floor)build, x, y);
            if (build is Roof) RoofPlacer.I.PlaceRoof((Roof)build, x, y);
            if (build is Building && ((Building)build).isSpecialPlace) SetWall((Building)build, x, y);
            if (build is Trap) SetTrap((Trap)build, x, y);
            if (build is Door) SetDoor((Door)build, x, y);
            if (build is Furniture) SetFurniture((Furniture)build, x, y);
        }
    }

    public static void SetWall(Building f, int x, int y)
    {
        ResetTFM(x, y);
        if (!f.isSpecialPlace) buildings[x, y] = f;
        else specials[x, y] = f;
        PathfindExtra.SetUsed(x, y);
        if (MapGenerator.I.drawMode == DrawMode.Place)
            if (!f.isSpecialPlace)                                 WCMngr.I.solidTilemap.SetTile(new Vector3Int(x, y, 0), f.tile);
            else if (f.isSpecialPlace && buildings[x,y] != null)   WCMngr.I.specialTilemap.SetTile(new Vector3Int(x,y,0), f.tile);
    }

    public static void SetTrap(Trap trap,  int x, int y)
    {
        ResetTFM(x,y);
        traps[x, y] = trap;
    }

    public static void RemoveWall(int x, int y)
    {
        buildings[x, y] = null;
        PathfindExtra.SetFree(x, y);
    }

    static void RotateTile(int x, int y, float rotation)
    {
        Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, rotation), Vector3.one);
        WCMngr.I.solidTilemap.SetTransformMatrix(new Vector3Int(x, y, 0), matrix);
    }

    public static void SetFurniture(Furniture f, int x, int y)
    {
        // for loading files: Loader.currentRotation. it sucks but it works
        float rotation = 0;
        if(Menus.I.inSC) rotation = Rotator.Rotation;
        Debug.Log($"{rotation}");

        Tile[,] tiles = f.tile;
        tiles = Rotate(tiles, rotation);

        for (int i = 0; i < tiles.GetLength(0); i++)
            for (int j = 0; j < tiles.GetLength(1); j++)
            {
                furnitures[x + i, y + j] = f;
                if (MapGenerator.I.drawMode == DrawMode.Place)
                {
                    RotateTile(x + i, y + j, rotation);
                    WCMngr.I.solidTilemap.SetTile(new Vector3Int(x+i, y+j, 0), tiles[i, j]);
                }
            }
    }

    static Tile[,] ReverseRows(Tile[,] input)
    {
        Tile[,] arr = input;
        for (int i = 0; i < arr.GetLength(0); i++)
        {

            // Initialise start and end index
            int start = 0;
            int end = arr.GetLength(1) - 1;

            // Till start < end, swap the element
            // at start and end index
            while (start < end)
            {

                // Swap the element
                Tile temp = arr[i, start];
                arr[i, start] = arr[i, end];
                arr[i, end] = temp;

                // Increment start and decrement
                // end for next pair of swapping
                start++;
                end--;
            }
        }
        return arr;
    }
    private static Tile[,] swap(Tile[,] arr, int start, int i, int end, int j)
    {

        Tile temp = arr[start, i];
        arr[start, i] = arr[end, j];
        arr[end, j] = temp;
        return arr;
    }
    static Tile[,] ReverseCols(Tile[,] input)
    {
        Tile[,] arr = input;
        for (int i = 0; i < input.GetLength(0); i++)
        {
            // Initialise start and end index
            int start = 0;
            int end = arr.GetLength(1) - 1;

            // Till start < end, swap the
            // element at start and end index
            while (start < end)
            {

                // Swap the element
                arr = swap(arr, start, i, end, i);

                // Increment start and decrement
                // end for next pair of swapping
                start++;
                end--;
            }
        }
        return arr;
    }

    static Tile[,] Rotate(Tile[,] array, float degrees)
    {
        Tile[,] output = new Tile[array.GetLength(0), array.GetLength(1)];

        switch(degrees)
        {
            case 0:
            case 360:
                return array;
            case 180:
                output = ReverseCols(array);
                break;
            case 90:
                output = new Tile[array.GetLength(1), array.GetLength(0)];
                for(int i = 0; i < array.GetLength(0); i++)
                    for(int j = 0; j < array.GetLength(1); j++)
                        output[j, i] = array[i, j];
                break;
            case 270:
                output = new Tile[array.GetLength(1), array.GetLength(0)];
                for (int i = 0; i < array.GetLength(0); i++)
                    for (int j = 0; j < array.GetLength(1); j++)
                        output[j, i] = array[i, j];
                output = ReverseRows(output);
                break;
        }

        return output;
    }

    public static void SetDoor(Door d, int x, int y)
    {
        doors[x, y] = d; // DO NOT SET DOORS AS USED!! YOU CAN GO THROUGH THEM!!
        float rotation;
        if (y + 1 < MapGenerator.I.mapHeight && y - 1 >= 0 && buildings[x, y + 1] != null && buildings[x, y - 1] != null) rotation = 90;
        else rotation = 0;

        Tile[,] tiles = Rotate(d.tile, rotation);

        for(int i = 0; i < tiles.GetLength(0); i++)
            for(int j = 0; j < tiles.GetLength(1); j++)
            {
                doors[x+i, y+j] = d;
                if (MapGenerator.I.drawMode == DrawMode.Place)
                {
                    RotateTile(x,y, rotation);
                    WCMngr.I.solidTilemap.SetTile(new Vector3Int(x+i,y+j,0), tiles[i,j]);
                }
            }
    }

    static void ResetTFM(int x, int y)
    {
        Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, 0), Vector3.one);
        WCMngr.I.solidTilemap.SetTransformMatrix(new Vector3Int(x, y, 0), matrix);
    }

    public static int GetNumberOfTiles(Tilemap tilemap)
    {
        //tilemap.CompressBounds();
        TileBase[] tiles = tilemap.GetTilesBlock(tilemap.cellBounds);
        return tiles.Where(x => x != null).ToArray().Length;
    }

    public static void UpdateBuildings()
    {
        buildings = new Building[MapGenerator.I.mapWidth, MapGenerator.I.mapHeight];
        floors = new Floor[MapGenerator.I.mapWidth, MapGenerator.I.mapHeight];
        doors = new Door[MapGenerator.I.mapWidth, MapGenerator.I.mapHeight]; // this really sucks
        traps = new Trap[MapGenerator.I.mapWidth, MapGenerator.I.mapHeight];
        specials = new Building[MapGenerator.I.mapWidth, MapGenerator.I.mapHeight];
        rooves = new Roof[MapGenerator.I.mapWidth, MapGenerator.I.mapHeight];
        furnitures = new Furniture[MapGenerator.I.mapWidth, MapGenerator.I.mapHeight];
    }

    public static void UpdateTilemap(float[,] noiseMap, TerrainType[] tTypesUnsorted, bool place)
    {
        TerrainType[] tTypes = tTypesUnsorted.OrderBy(x => x.height).ToArray(); // this line of code makes the entire game, i'll make it a puzzle! figure out why! ;)
        tilemap = new TerrainType[noiseMap.GetLength(0), noiseMap.GetLength(1)];

        for (int x = 0; x < noiseMap.GetLength(0); x++)
        {
            for (int y = 0; y < noiseMap.GetLength(1); y++)
            {
                for (int i = 0; i < tTypes.Length; i++)
                {
                    if (noiseMap[x, y] <= BiomeArea.waterHeight + tTypes[i].height)
                    {
                        /* no*/switch (tTypes[i].type)
                        {
                            case SpecialType.None:
                                TerrainType ttype = tTypes[i].height <= BiomeArea.DEFAULT_WATER_HEIGHT ? MapGenerator.I.currentBiome.WaterClampTT : tTypes[i];
                                if(place)
                                    WCMngr.I.groundTilemap.SetTile(new Vector3Int(x, y, 0), tTypes[i].tile);
                                tilemap[x, y] = tTypes[i];
                                break;
                            case SpecialType.Mountain:
                               // TileBase tile = !isMountainless ? tTypes[i].thisIsVeryBadSpaghettiButImOutOfIdeas : MapGenerator.I.currentBiome.WaterClampTT.tile;
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
        if(place && !Menus.I.inSC)
        {
        //    WCMngr.I.solidTilemap.RefreshAllTiles();
        //    WCMngr.I.groundTilemap.RefreshAllTiles();
            I.pfinder.Scan();
        }
    }                                                           // TODO: when placing builds in this array, if one is damaged, are all?

    public static void DestroyBuilding (Vector2 position)
    {
        var build = buildings[(int)position.x, (int)position.y];
        Destroy(build.damageObject);
        
        if(build.rubbleType != RubbleType.None)
        {
            var go = Instantiate(WCMngr.I.rubblePrefab);
            go.transform.position = new Vector3(position.x, position.y, 0);

            Sprite spr = build.rubbleType switch
            {
                RubbleType.woodrubble => WCMngr.I.woodrubbleTex,
                RubbleType.stonerubble => WCMngr.I.stonerubbleTex,
                RubbleType.miscrubble => WCMngr.I.miscrubbleTex,
                _ => WCMngr.I.miscrubbleTex,
            };
            go.GetComponent<SpriteRenderer>().sprite = spr;
            go.transform.Rotate(0,0,Random.Range(0f,361f));
        }

        buildings[(int)position.x, (int)position.y] = null;
        WCMngr.I.groundTilemap.SetTile(Vector3Int.FloorToInt(position), null);
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
