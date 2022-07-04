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
    public const int ROOMSIZE = 16;
    public static void PlaceStructure(Structure s, System.Random rng, SliderInt mapSizeSlider)
    {
        // set the mapgenerator.structurepos and size
        // messages.add if too big, let the player know the map is being resized
    }
    [System.Obsolete]
    /// <summary>This is a relic from when I want procedurally generated structures. It's here in case I need it sometime.</summary>
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
        List<List<bool>> roomMap = new List<List<bool>>();// use chunks of 16x16 rooms scaled up in some way. makes perfect rooms. looks sexy
        for (int i = 0; i < 25; i++) // start off with some size. bc no one would ever want a 320x320 room
        {
            roomMap.Add(new List<bool>());
            for (int j = 0; j < 25; j++)
                roomMap[i].Add(false);
        }
        foreach (RoomInfo r in rooms)
        {// we can resize rooms to fill in the missing space bc different length rooms

            (int x, int y, int width, int height) roomPos = roomMap.FindClosestOfSize(r.room.possibleSizes, s.RoomScale);
            int width = roomPos.width;
            int height = roomPos.height;

            roomMap.FillUp(width, height, roomPos.x, roomPos.y);
            rowLengths[currentRow] += width;
            //if(rowLengths[currentRow] > SumUpTo(rowLengths, currentRow))
            //    structureWidth += width;
            if (roomIndex % destinedRowCount == 0)
            {
                rowLengths.Add(0);
                currentRow++;
                curY += currentHighest * ROOMSIZE;
                curX = 0;
                structureHeight += currentHighest * ROOMSIZE;
                currentHighest = 0;
            }

            if (height > currentHighest)
                currentHighest = height;
            sizes.Add((width, height));
            roomOrder.Add(r.room);
            positions.Add(new Vector2Int(curX, curY));
            curX += width * ROOMSIZE;
            roomIndex++;
        }

        structureWidth = rowLengths.Max() * ROOMSIZE;

        Vector2Int cornerRoomSize = s.CornerRoom != null ? s.CornerRoom.possibleSizes.randomElement(rng) : Vector2Int.zero;
        (int x, int y) structurePosition = (rng.Next(cornerRoomSize.x*ROOMSIZE+1, cornerRoomSize.x*ROOMSIZE+1 + Mathf.Abs(MapGenerator.I.mapWidth - structureWidth)),
                                            rng.Next(cornerRoomSize.y*ROOMSIZE+1, cornerRoomSize.y*ROOMSIZE+1 + Mathf.Abs(MapGenerator.I.mapHeight - structureHeight)));
        if (structurePosition.x + structureWidth + cornerRoomSize.x * ROOMSIZE >= MapGenerator.I.mapWidth - 20 
            || structurePosition.y + structureHeight + cornerRoomSize.y * ROOMSIZE >= MapGenerator.I.mapHeight - 20)
        {
            Debug.Log($"I feel the need to resize the map.");
            MapGenerator.I.ResizeMapToFit(new Vector2Int(structurePosition.x + structureWidth + (cornerRoomSize.x * ROOMSIZE), structurePosition.y + structureHeight + (cornerRoomSize.y * ROOMSIZE)));
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
            Vector2Int pos = new Vector2Int(structurePosition.x - (cornerRoomSize.x * (ROOMSIZE / 2)), structurePosition.y - (cornerRoomSize.y * (ROOMSIZE / 2)));
            cornerRoomPoints.AddRange(RoomGenerator.I.GenerateRoom(s.CornerRoom, pos, (cornerRoomSize.x * ROOMSIZE, cornerRoomSize.y * ROOMSIZE), s, new List<(int, int)>(), rng, new List<Vector2Int>(), true, exteriorWall));
            // bottom right
            Vector2Int pos2 = new Vector2Int(structurePosition.x + structureWidth - (cornerRoomSize.x * (ROOMSIZE/2)), structurePosition.y - (cornerRoomSize.y * (ROOMSIZE / 2)));
            cornerRoomPoints.AddRange(RoomGenerator.I.GenerateRoom(s.CornerRoom, pos2, (cornerRoomSize.x * ROOMSIZE, cornerRoomSize.y * ROOMSIZE), s, new List<(int, int)>(), rng, new List<Vector2Int>(), true, exteriorWall));
            // top left
            Vector2Int pos3 = new Vector2Int(structurePosition.x - (cornerRoomSize.x * (ROOMSIZE / 2)), structurePosition.y + structureHeight - (cornerRoomSize.y * (ROOMSIZE / 2)));
            cornerRoomPoints.AddRange(RoomGenerator.I.GenerateRoom(s.CornerRoom, pos3, (cornerRoomSize.x * ROOMSIZE, cornerRoomSize.y * ROOMSIZE), s, new List<(int, int)>(), rng, new List<Vector2Int>(), true, exteriorWall));
            // top right
            Vector2Int pos4 = new Vector2Int(structurePosition.x + structureWidth - (cornerRoomSize.x * (ROOMSIZE / 2)), structurePosition.y + structureHeight - (cornerRoomSize.y * 8));
            cornerRoomPoints.AddRange(RoomGenerator.I.GenerateRoom(s.CornerRoom, pos4, (cornerRoomSize.x * ROOMSIZE, cornerRoomSize.y * ROOMSIZE), s, new List<(int, int)>(), rng, new List<Vector2Int>(), true, exteriorWall));
        }
        cornerRoomPoints.AddRange(exteriorWallPoints);
        foreach (Room ri in roomOrder)
        {
            int findex = roomOrder.IndexOf(ri);
            
            List<(int x, int y)> doorPoints = new List<(int x, int y)>();

            var size = (sizes[findex].width * 16, sizes[findex].height * 16);
            Vector2Int pos = new Vector2Int(positions[findex].x + structurePosition.x, positions[findex].y + structurePosition.y);
            RoomGenerator.I.GenerateRoom(ri, pos, size, s, doorPoints, rng, cornerRoomPoints);
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

    public static void FillUp(this List<List<bool>> list, int width, int height, int x, int y)
    {
        if (x == int.MinValue || y == int.MinValue)
            return;
        for (int i = x; i < x+width; i++)
        {
            for (int j = y; j < y + height; j++)
            {
                if (i >= list.Count)
                    for(int asd = 0; asd < i - list.Count; asd++)
                        list.Add(new List<bool>());
                if (j >= list[i].Count)
                    for(int asd = 0; asd < j - list[i].Count; asd++)
                        list[i].Add(false);

                Debug.Log($"{i}, {j}");
                list[i][j] = true;
            }
        }
    }

    public static (int x, int y, int width, int height) FindClosestOfSize(this List<List<bool>> list, List<Vector2Int> possibleSizes, float RoomScale)
    {
        (int x, int y, int width, int height) pos = (int.MinValue, int.MinValue, 0, 0);
        for (int D = 0; D < possibleSizes.Count; D++)
        {
            Vector2 sizeT = (Vector2)possibleSizes[D] * RoomScale;

            int consecutiveX = 0;
            int consecutiveY = 0;
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < list[i].Count; j++)
                {
                    if (list[i][j])
                    {
                        if (consecutiveX >= Mathf.Ceil(sizeT.x) && consecutiveY >= Mathf.Ceil(sizeT.y))
                        {
                            pos.width = Mathf.CeilToInt(sizeT.x);
                            pos.height = Mathf.CeilToInt(sizeT.y);
                            return pos;
                        }
                        consecutiveX = 0;
                        consecutiveY = 0;
                        pos = (int.MinValue, int.MinValue, 0, 0);
                        continue;
                    }
                    if (consecutiveX == 0)
                    {
                        pos.x = i;
                        pos.y = j;
                    }

                    if (j == pos.y)
                        consecutiveY++;
                    consecutiveX++;
                }
            }
            Debug.Log($"{consecutiveX}, {consecutiveY}   ::      {sizeT}");
            if (consecutiveX >= Mathf.Ceil(sizeT.x) && consecutiveY >= Mathf.Ceil(sizeT.y))
            {
                pos.width = Mathf.CeilToInt(sizeT.x);
                pos.height = Mathf.CeilToInt(sizeT.y);
                return pos;
            }
        }
        DB.Attention("Returning default room. Error");
        return pos;
    }
}