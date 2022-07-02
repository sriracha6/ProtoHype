using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PawnFunctions
{
    /// <summary>
    /// Stores an array of the size of the tilemap. Each position can be a 0 or 1. This is only set when the pathfind is 
    /// </summary>
    public struct Pos
    {
        public int x { get; }
        public int y { get; }

        public bool Taken;

        public static implicit operator Vector2Int(Pos p)
        {
            return new Vector2Int(p.x, p.y);
        }
        public static implicit operator Vector2(Pos p)
        {
            return new Vector2(p.x, p.y);
        }
        public static implicit operator Vector3(Pos p)
        {
            return new Vector3(p.x, p.y, 0);
        }

        public Pos(int q, int e, bool taken)
        {
            x = q; y = e; Taken = taken;
        }

        public override string ToString()
        {
            return Taken ? "#" : " ";
        }
    }
    public class PathfindExtra
    {
        public static List<Pos> tiles;

        public static int sizeX;
        public static int sizeY;
        
        public static void SetUsed(int x, int y)
        {
            tiles[x + y] = new Pos(x, y, true); 
        }
        public static void SetFree(int x, int y)
        {
            tiles[x+y] = new Pos(x, y, false);
        }

        /// <returns>a picture of a weewee</returns>
        public static Pos FindNearest(Vector2Int check, List<Vector2Int> extraGayIgnore=null)
        {
            Pos fuckingPoint;
                fuckingPoint = PathfindExtra.tiles.AsReadOnly().OrderBy(p => Math.Abs(p.x - check.x) + Math.Abs(p.y - check.y)).FirstOrDefault(p => p.Taken == false);
            // ASREADONLY IS O(1) FUCK YES IM GONNA KISS SOMEONE THIS IS SO SEXY SEXY SEXY SEXY SEXY SEXY
            if(extraGayIgnore != null)
                fuckingPoint = PathfindExtra.tiles.AsReadOnly().OrderBy(p => Math.Abs(p.x - check.x) + Math.Abs(p.y - check.y)).FirstOrDefault(p => p.Taken == false && !extraGayIgnore.Contains(new Vector2Int(p.x, p.y)));
            return fuckingPoint;
        }
        public static bool PresentAt(int x, int y)
        {
            return PathfindExtra.tiles[x + y].Taken;
        }

        public PathfindExtra()
        {                                                                           // !!!!!!!!
                                                                                    // ALL OBJSTACLES (MOUNTAIN, WALL, FURNITURE) ARE 1!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            sizeX = MapGenerator.I.mapWidth;
            sizeY = MapGenerator.I.mapHeight;

            tiles = new List<Pos>();

            for (int tx = 0; tx < sizeX; tx++)
                for (int ty = 0; ty < sizeY; ty++)
                    tiles.Add(new Pos(tx, ty, false));

            if (MapGenerator.I.drawMode != MapGenerator.DrawMode.Place) return;

            for (int tx = 0; tx < sizeX; tx++)
            {
                for (int ty = 0; ty < sizeY; ty++)
                {
                    if (TilemapPlace.tilemap[tx, ty].type == SpecialType.None)
                        tiles.Add(new Pos(tx, ty, false));
                    else if (TilemapPlace.tilemap[tx, ty].type == SpecialType.Mountain) 
                        tiles.Add(new Pos(tx, ty, true)); // you can walk over water.
                }
            }
        }
    }
}