using Buildings;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TargetLayer { Solid, Water, Terrain, Roof }
public enum Brush { Pencil, Brush, Box, Bucket }
public class Placer : MonoBehaviour
{
    public static Placer I;
    public static Brush BrushType;
    public static int SideLength = 2;

    protected void Awake() => I = this;

    public static Item[,] currentMap;
    public static Item currentItem;
    public static TileBase PlacedItem;
    public static TargetLayer targetLayer;

    public static bool furnitureMode;

    Vector3Int startLinePos;
    Vector3Int endLinePos;
    Tilemap tmap;

    TerrainType floodTType;
    Build floodBType;
    public static bool canPlace;

    bool specialsFlag; // THIS SUCKS

    protected void Update()
    {
        if (canPlace && Input.GetMouseButtonDown(Keybinds.LeftMouse) && !UIManager.mouseOverUI)
        {
            UndoRedo.changedValues.Insert(0,new List<(Vector2Int, List<Build>)>());
            tmap = targetLayer switch
            {
                TargetLayer.Solid => WCMngr.I.solidTilemap,
                TargetLayer.Roof => WCMngr.I.roofTilemap,
                TargetLayer.Terrain => WCMngr.I.groundTilemap,
                TargetLayer.Water => null,
                _ => null
            };

            specialsFlag = (currentItem is Build && ((Build)currentItem).isSpecialPlace);

            var p = WCMngr.I.mainCam.ScreenToWorldPoint(Input.mousePosition) / 2;
            if (BrushType == Brush.Box)
                startLinePos = new Vector3Int((int)p.x, (int)p.y, 0);
            else if (BrushType == Brush.Bucket)
            {
                var s = new Vector3Int((int)p.x, (int)p.y, 0);
                floodTType = TilemapPlace.tilemap[s.x, s.y]; // todo: support for flooding buildings
                FloodFill(s);
            }
        }

        if (canPlace && Input.GetMouseButton(Keybinds.LeftMouse) && !UIManager.mouseOverUI)
        {
            var p = WCMngr.I.mainCam.ScreenToWorldPoint(Input.mousePosition) / 2;
            Vector3Int pos = new Vector3Int((int)p.x, (int)p.y, 0);

            switch(BrushType)
            {
                case Brush.Pencil:
                    Set(pos);
                    break;
                case Brush.Brush:
                    for(int x = 0; x < SideLength; x++)
                        for(int y = 0; y < SideLength; y++)
                            Set((pos + new Vector3Int(x, y, 0)).clampVector());
                    break;
                case Brush.Box:
                    endLinePos = pos;
                    break;
            }
        }

        if(canPlace && Input.GetMouseButtonUp(Keybinds.LeftMouse) && !UIManager.mouseOverUI)
        {
            for(int x = startLinePos.x; x <= endLinePos.x; x++)
                for(int y = startLinePos.y; y <= endLinePos.y; y++)
                {
                    if(x == startLinePos.x || x == endLinePos.x || y == startLinePos.y || y == endLinePos.y)
                        Set(new Vector3Int(x,y,0));
                }
            UndoRedo.currentIndex++;
        }

        if (Input.GetKey(Keybinds.SubtractSelection))
            SideLength += (int)(Input.GetAxis("Mouse ScrollWheel") * 7.5f);

    }

    void FloodFill(Vector3Int position)
    {
        // todo: Span Filling
        if (position.y < 0 || position.y >= MapGenerator.I.mapHeight || position.x < 0 || position.x >= MapGenerator.I.mapWidth) return;
        if (TilemapPlace.tilemap[position.x, position.y] == floodTType && TilemapPlace.tilemap[position.x, position.y].tile != PlacedItem)
        {
            Set(position);
            FloodFill(new Vector3Int(position.x,position.y--,0)); // south
            FloodFill(new Vector3Int(position.x,position.y++,0)); // north
            FloodFill(new Vector3Int(position.x--,position.y,0)); // west
            FloodFill(new Vector3Int(position.x++,position.y,0)); // east
        }
    }

    void Set(Vector3Int position)
    {
        if(!furnitureMode)
        {
            if (specialsFlag)
            {
                currentMap = TilemapPlace.specials;
                tmap = MapGenerator.I.specialTilemap;
            }
            if (tmap != null)
            {
                currentMap[position.x, position.y] = currentItem;
                if (tmap != WCMngr.I.roofTilemap)
                    tmap.SetTile(position, PlacedItem);
                else RoofPlacer.I.PlaceRoof((Roof)currentItem, position.x, position.y);
            }
            else
            {
                TilemapPlace.tilemap[position.x, position.y] = null;
                WCMngr.I.groundTilemap.SetTile(position, null);
                WCMngr.I.solidTilemap.SetTile(position, null);
            }
        }
        else
        {
            if (currentItem is Furniture) TilemapPlace.SetFurniture((Furniture)currentItem, position.x, position.y);
            if (currentItem is Door) TilemapPlace.SetDoor((Door)currentItem, position.x, position.y);
        }

        if (UndoRedo.currentIndex <= UndoRedo.changedValues.Count - 1)
        {
            UndoRedo.changedValues[UndoRedo.changedValues.Count - 1].Add(((Vector2Int)position, TilemapPlace.BuildsAt(position.x, position.y)));
            UndoRedo.currentIndex++;
        }
        else
        {
            UndoRedo.changedValues.Clear();
            UndoRedo.currentIndex = 0;
        }
    }
}
