using System;
using System.Linq;
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
        List<(int width, int height)> sizes = new List<(int, int)>();
        List<Vector2Int> positions = new List<Vector2Int>();

        foreach(RoomInfo r in s.RoomInfo)
        {
            if (rooms.Count >= s.RoomCount.Max)
                break;
            rooms.Add(r);
        }
        if (rooms.Count < s.RoomCount.Min)
            for (int i = 0; i < s.RoomCount.Min - rooms.Count; i++)
                rooms.Add(s.RoomInfo.randomElement(rng));

        int structureWidth = 0;
        int structureHeight = 0;

        List<Room> roomOrder = new List<Room>();
        int destinedRowCount = rng.Next(3,5); // todo make this in xml or something
        int roomIndex = 0;
        int currentRow = 0;
        int currentHighest = 0;
        int curX = 0;
        int curY = 0;
        List<int> rowLengths = new List<int>();
        rowLengths.Add(0);
        foreach (RoomInfo r in rooms)
        {// we can resize rooms to fill in the missing space bc different length rooms
            Vector2 sizeT = (Vector2)r.room.possibleSizes.randomElement(rng) * s.RoomScale;
        
            int width = (int)sizeT.x;
            int height = (int)sizeT.y;
            rowLengths[currentRow] += width;

            if(rowLengths[currentRow] > SumUpTo(rowLengths, currentRow))
                structureWidth += width;
            if (roomIndex % destinedRowCount == 0)
            {
                rowLengths.Add(0);
                currentRow++;
                curY += currentHighest;
                curX = 0;
                structureHeight += currentHighest;
                currentHighest = 0;
            }

            if (height > currentHighest)
                currentHighest = height;
            sizes.Add((width, height));
            roomOrder.Add(r.room);
            positions.Add(new Vector2Int(curX, curY));
            curX += width;
            roomIndex++;
        }

        Vector2Int cornerRoomSize = s.CornerRoom != null ? s.CornerRoom.possibleSizes.randomElement(rng) : Vector2Int.zero;
        (int x, int y) structurePosition = (rng.Next(cornerRoomSize.x+1, cornerRoomSize.x+1 + Mathf.Abs(MapGenerator.I.mapWidth - structureWidth)),
                                            rng.Next(cornerRoomSize.y+1, cornerRoomSize.y+1 + Mathf.Abs(MapGenerator.I.mapHeight - structureHeight)));
        if (structurePosition.x + structureWidth + cornerRoomSize.x >= MapGenerator.I.mapWidth - 20 
            || structurePosition.y + structureHeight + cornerRoomSize.y >= MapGenerator.I.mapHeight - 20)
        {
            Debug.Log($"I feel the need to resize the map.");
            MapGenerator.I.ResizeMapToFit(new Vector2Int(structurePosition.x + structureWidth + cornerRoomSize.x, structurePosition.y + structureHeight + cornerRoomSize.y));
            mapSizeSlider.value = MapGenerator.I.mapWidth;
            mapSizeSlider.lowValue = MapGenerator.I.mapWidth;
        }

        List<Vector2Int> exteriorWallPoints = new List<Vector2Int>();
        Building exteriorWall = s.ExteriorWalls.randomElement(rng);
        Debug.Log($"STRUCTURE: {structureWidth},{structureHeight} @ {structurePosition.x},{structurePosition.y}");

        for (int x = structurePosition.x; x <= structurePosition.x + structureWidth; x++)
        {
            for (int y = structurePosition.y; y <= structurePosition.y + structureHeight; y++) 
            {
                if (x == structurePosition.x || y == structurePosition.y 
                    || x == structurePosition.x + structureWidth || y == structurePosition.y + structureHeight)
                {
                    TilemapPlace.SetWall(exteriorWall, x, y);
                    exteriorWallPoints.Add(new Vector2Int(x, y));
                }
            }
        }
        List<Vector2Int> cornerRoomPoints = new List<Vector2Int>();
        if(s.CornerRoom != null)
        {
            // bottom left
            Vector2Int pos = new Vector2Int(structurePosition.x - (cornerRoomSize.x / 2), structurePosition.y - (cornerRoomSize.y / 2));
            cornerRoomPoints.AddRange(RoomGenerator.I.GenerateRoom(s.CornerRoom, pos, (cornerRoomSize.x, cornerRoomSize.y), s, new List<(int, int)>(), rng, new List<Vector2Int>(), true, exteriorWall));
            // bottom right
            Vector2Int pos2 = new Vector2Int(structurePosition.x + structureWidth - (cornerRoomSize.x / 2), structurePosition.y - (cornerRoomSize.y / 2));
            cornerRoomPoints.AddRange(RoomGenerator.I.GenerateRoom(s.CornerRoom, pos2, (cornerRoomSize.x, cornerRoomSize.y), s, new List<(int, int)>(), rng, new List<Vector2Int>(), true, exteriorWall));
            // top left
            Vector2Int pos3 = new Vector2Int(structurePosition.x - (cornerRoomSize.x / 2), structurePosition.y + structureHeight - (cornerRoomSize.y / 2));
            cornerRoomPoints.AddRange(RoomGenerator.I.GenerateRoom(s.CornerRoom, pos3, (cornerRoomSize.x, cornerRoomSize.y), s, new List<(int, int)>(), rng, new List<Vector2Int>(), true, exteriorWall));
            // top right
            Vector2Int pos4 = new Vector2Int(structurePosition.x + structureWidth - (cornerRoomSize.x / 2), structurePosition.y + structureHeight - (cornerRoomSize.y / 2));
            cornerRoomPoints.AddRange(RoomGenerator.I.GenerateRoom(s.CornerRoom, pos4, (cornerRoomSize.x, cornerRoomSize.y), s, new List<(int, int)>(), rng, new List<Vector2Int>(), true, exteriorWall));
        }
        cornerRoomPoints.AddRange(exteriorWallPoints);
        foreach (Room ri in roomOrder)
        {
            int findex = roomOrder.IndexOf(ri);
            
            List<(int x, int y)> doorPoints = new List<(int x, int y)>();

            /*if ((doors & (int)Side.Top) == (int)Side.Top)          doorPoints.Add((Pos.x + rng.Next(1, width), Pos.y + height - 1));
            if ((doors & (int)Side.Bottom) == (int)Side.Bottom)    doorPoints.Add((Pos.x + rng.Next(1, width), Pos.y));
            if ((doors & (int)Side.Left) == (int)Side.Left)        doorPoints.Add((Pos.x, Pos.y + rng.Next(1, height)));
            if ((doors & (int)Side.Right) == (int)Side.Right)      doorPoints.Add((Pos.x + width, Pos.y + rng.Next(1, height)));


            RoomGenerator.I.GenerateRoom(room, SystemException, SystemException, (width, height), s, doorPoints, rng);
        */
            Vector2Int pos = new Vector2Int(positions[findex].x + structurePosition.x, positions[findex].y + structurePosition.y);
            RoomGenerator.I.GenerateRoom(ri, pos, sizes[findex], s, doorPoints, rng, cornerRoomPoints);
        }
    }

    public static int SumUpTo(List<int> list, int index)
    {
        int s = 0;
        for(int i = 0; i < index; i++)
            s += list[i];
        return s;
    }

    public static int SumUp(List<int> list)
    {
        int s = 0;
        for (int i = 0; i < list.Count; i++)
            s += list[i];
        return s;
    }
}