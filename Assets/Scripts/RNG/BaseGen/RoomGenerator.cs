using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buildings;
using Structures;

[System.Flags]
enum DoorSide : int { Left, Top, Right, Bottom }

public class RoomGenerator : MonoBehaviour
{
    public static RoomGenerator I { get; private set; } = null;
    // Start is called before the first frame update
    protected void Awake()
    {
        I = this;
        DontDestroyOnLoad(gameObject);
    }
                                       //     V        V          V   from structure
    public void GenerateRoom(Room room, Vector2Int Pos, (int width, int height) size, Structure structure, List<(int x, int y)> doorPoints, System.Random rng, bool resetArea=false)
    {
        Floor floor = room.possibleFloors[rng.Next(0, room.possibleFloors.Count)];
        Door door = structure.Doors[rng.Next(0, structure.Doors.Count)];

        for (int x = Pos.x; x < size.width; x++)
        {
            for (int y = Pos.y; y < size.height; y++)
            {
                if(resetArea)
                {
                    TilemapPlace.RemoveWall(x, y);
                    TilemapPlace.SetFloor(null, x, y);
                    TilemapPlace.SetDoor(null, x, y, 0);
                    RoofPlacer.I.PlaceRoof(null, x, y);
                }
                if (x == Pos.x || y == Pos.y || y == Pos.y + size.height || x == Pos.x + size.width)
                    TilemapPlace.SetWall(structure.InteriorWalls, x, y);
                else
                    TilemapPlace.SetFloor(floor, x, y);
                RoofPlacer.I.PlaceRoof(structure.Roof, x, y);
                if(doorPoints.Contains((x,y)))
                    TilemapPlace.SetDoor(door, x, y, 90);
            }
        }
    }
}
