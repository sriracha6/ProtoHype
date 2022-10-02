using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buildings;
using Structures;

[System.Flags]
enum DoorSide : int { Left, Top, Right, Bottom }

public class RoomGenerator : MonoBehaviour
{
    /*
    public static RoomGenerator I { get; private set; } = null;
    // Start is called before the first frame update
    protected void Awake()
    {
        I = this;
        DontDestroyOnLoad(gameObject);
    }
                                       //     V        V          V   from structure
    public List<Vector2Int> GenerateRoom(Room room, Vector2Int Pos, (int width, int height) size, Structure structure, List<(int x, int y)> doorPoints, System.Random rng, List<Vector2Int> ignorePoints, bool cornerRoom=false, Building cornerWall=null)
    {
        List<Vector2Int> roomPoints = new List<Vector2Int>();
        Floor floor = room.possibleFloors[rng.Next(0, room.possibleFloors.Count)];
        Door door = structure.Doors[rng.Next(0, structure.Doors.Count)];
        Building wall = cornerRoom ? cornerWall : structure.InteriorWalls;

        for (int x = Pos.x; x <= Pos.x + size.width; x++)
        {
            for (int y = Pos.y; y <= Pos.y + size.height; y++)
            {
                if (ignorePoints.Contains(new Vector2Int(x, y)))
                    continue;
                if(cornerRoom)
                {
                    TilemapPlace.RemoveWall(x, y);
                    TilemapPlace.SetFloor(null, x, y);
                    TilemapPlace.SetDoor(null, x, y);
                    RoofPlacer.I.PlaceRoof(null, x, y);
                }
                if (x == Pos.x || y == Pos.y || y == Pos.y + size.height || x == Pos.x + size.width 
                    && TilemapPlace.buildings[x,y] == null)
                    TilemapPlace.SetWall(wall, x, y);
                else
                   TilemapPlace.SetFloor(floor, x, y);

                RoofPlacer.I.PlaceRoof(structure.Roof, x, y);
                roomPoints.Add(new Vector2Int(x,y));

                if(doorPoints.Contains((x,y)))
                    TilemapPlace.SetDoor(door, x, y);
            }
        }
        return roomPoints;
    }
    */
}
