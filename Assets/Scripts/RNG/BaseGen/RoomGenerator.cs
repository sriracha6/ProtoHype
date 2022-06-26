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
    public void GenerateRoom(Room room, (int x, int y) Pos, int doors, (int width, int height) size, Structure structure, List<(int x, int y)> doorPoints, System.Random rng)
    {
        Floor floor = room.possibleFloors[rng.Next(0, room.possibleFloors.Count)];
        Door door = structure.Doors[rng.Next(0, structure.Doors.Count)];

        for (int x = Pos.x; x < size.width; x++)
        {
            for (int y = Pos.y; y < size.height; y++)
            {
                if (x == 0 || y == 0 || y == size.height - 1 || x == size.width - 1)
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
