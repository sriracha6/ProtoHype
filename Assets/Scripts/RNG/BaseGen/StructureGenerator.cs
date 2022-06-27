using System;
using UnityEngine;
using UnityEngine.UIElements;
using Structures;
using System.Collections.Generic;
using XMLLoader;
using Buildings;

public static class StructureGenerator
{

    public static void GenerateStructure(Structure s, System.Random rng, SliderInt mapSizeSlider)
    {
        List<RoomInfo> rooms = new List<RoomInfo>();
        List<(int, int)> sizes = new List<(int, int)>();

        foreach(RoomInfo r in s.RoomInfo)
        {
            if (rooms.Count >= s.RoomCount.Max)
                continue;
            rooms.Add(r);
        }
        if (rooms.Count < s.RoomCount.Min)
            for (int i = 0; i < s.RoomCount.Min - rooms.Count; i++)
                rooms.Add(s.RoomInfo.randomElement());

        int structureWidth = 0;
        int structureHeight = 0;

        List<Room> roomOrder = new List<Room>();

        foreach (RoomInfo r in rooms)
        {
            Vector2Int sizeT = r.room.possibleSizes.randomElement();
            
            int width = sizeT.x;
            int height = sizeT.y;

            structureWidth += width;
            structureHeight += height;
            sizes.Add((width, height));
            roomOrder.Add(r.room);
        }

        Vector2Int cornerRoomSize = s.CornerRoom != null ? s.CornerRoom.possibleSizes.randomElement() : Vector2Int.zero;
        (int x, int y) structurePosition = (rng.Next(0, Mathf.Abs(MapGenerator.I.mapWidth - structureWidth) + cornerRoomSize.x),
                                            rng.Next(0, Mathf.Abs(MapGenerator.I.mapHeight - structureHeight) + cornerRoomSize.y));
        
        if (structurePosition.x + structureWidth + cornerRoomSize.x >= MapGenerator.I.mapWidth - 20 
            || structurePosition.y + structureHeight + cornerRoomSize.y >= MapGenerator.I.mapHeight - 20)
        {
            MapGenerator.I.ResizeMapToFit(new Vector2Int(structurePosition.x + structureWidth + cornerRoomSize.x, structurePosition.y + structureHeight + cornerRoomSize.y));
            mapSizeSlider.value = MapGenerator.I.mapWidth;
            mapSizeSlider.lowValue = MapGenerator.I.mapWidth;
        }

        
        List<Vector2Int> exteriorWallPoints = new List<Vector2Int>();
        Building exteriorWall = s.ExteriorWalls.randomElement();
        Debug.Log($"STRUCTURE: {structureWidth},{structureHeight} @ {structurePosition.x},{structurePosition.y}");

        for (int x = structurePosition.x; x <= 1 + structurePosition.x + structureWidth; x++) // i have no fucking idea why i have to add 1 to these. but it works. this caused me a lot of pain
        {
            for (int y = structurePosition.y; y <= 1 + structurePosition.y + structureHeight; y++) // seriously wtf if i have <= why tf wtf???
            {
                if (x == structurePosition.x || y == structurePosition.y 
                    || x == structurePosition.x + structureWidth || y == structurePosition.y + structureHeight)
                {
                    TilemapPlace.SetWall(exteriorWall, x, y);
                    exteriorWallPoints.Add(new Vector2Int(x, y));
                }
            }
        }

        if(s.CornerRoom != null)
        {
            Debug.Log($"makign corner rooms");
            // bottom left
            Vector2Int pos = new Vector2Int(structurePosition.x - (cornerRoomSize.x / 2), structurePosition.y - (cornerRoomSize.y / 2)); 
            RoomGenerator.I.GenerateRoom(s.CornerRoom, pos, (cornerRoomSize.x, cornerRoomSize.y), s, new List<(int, int)>(), rng, true);
            // bottom right
            Vector2Int pos2 = new Vector2Int(structurePosition.x + structureWidth - (cornerRoomSize.x / 2), structurePosition.y - (cornerRoomSize.y / 2));
            RoomGenerator.I.GenerateRoom(s.CornerRoom, pos2, (cornerRoomSize.x, cornerRoomSize.y), s, new List<(int, int)>(), rng, true);
            // top left
            Vector2Int pos3 = new Vector2Int(structurePosition.x - (cornerRoomSize.x / 2), structurePosition.y + structureHeight - (cornerRoomSize.y / 2));
            RoomGenerator.I.GenerateRoom(s.CornerRoom, pos3, (cornerRoomSize.x, cornerRoomSize.y), s, new List<(int, int)>(), rng, true);
            // top right
            Vector2Int pos4 = new Vector2Int(structurePosition.x + structureWidth - (cornerRoomSize.x / 2), structurePosition.y + structureHeight - (cornerRoomSize.y / 2));
            RoomGenerator.I.GenerateRoom(s.CornerRoom, pos4, (cornerRoomSize.x, cornerRoomSize.y), s, new List<(int, int)>(), rng, true);
        }
        

        foreach (Room ri in roomOrder)
        {
            var room = ri;
            
            int findex = roomOrder.IndexOf(ri);
            
            int width = sizes[findex].Item1;
            int height = sizes[findex].Item2;

            List<(int x, int y)> doorPoints = new List<(int x, int y)>();

            /*if ((doors & (int)Side.Top) == (int)Side.Top)          doorPoints.Add((Pos.x + rng.Next(1, width), Pos.y + height - 1));
            if ((doors & (int)Side.Bottom) == (int)Side.Bottom)    doorPoints.Add((Pos.x + rng.Next(1, width), Pos.y));
            if ((doors & (int)Side.Left) == (int)Side.Left)        doorPoints.Add((Pos.x, Pos.y + rng.Next(1, height)));
            if ((doors & (int)Side.Right) == (int)Side.Right)      doorPoints.Add((Pos.x + width, Pos.y + rng.Next(1, height)));


            RoomGenerator.I.GenerateRoom(room, SystemException, SystemException, (width, height), s, doorPoints, rng);
        */
        }
    }

    public static List<int> Factor(int number)
    {
        var factors = new List<int>();
        int max = (int)Mathf.Sqrt(number);  // Round down. uh oh sqrt

        for (int factor = 1; factor <= max; ++factor) // Test from 1 to the square root, or the int below it, inclusive.
        {
            if (number % factor == 0)
            {
                factors.Add(factor);
                if (factor != number / factor) // Don't add the square root twice!  Thanks Jon
                    factors.Add(number / factor);
            }
        }
        return factors;
    }
}