using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PawnFunctions
{
    /// <summary>
    /// Stores an array of the size of the tilemap. Each position can be a 0 or 1. This is only set when the pathfind is 
    /// done (i think). 1 = pawn is there. 0 = free. This should be pretty fast. TODO: OPTIMIZE!
    /// TODO: span?
    /// ALSO, INTS ARE 4 BYTES AND BOOLS ARE 1 SO DONT BE PROUD
    /// TODO: binary search instead of .find
    /// </summary>
    public struct Pos
    {
        public int x;
        public int y;

        public int Taken;

        public static implicit operator Vector2Int(Pos p)
        {
            return new Vector2Int(p.x, p.y);
        }

        public Pos(int q, int e, int taken)
        {
            x = q; y = e; Taken = taken;
        }
        public Pos(int q, int e)
        { x = q; y = e; Taken = 0; }
    }
    public class PathfindExtra
    {
        //public static int[][] tileUsed = new int[0][];
        //public static Tiles<int> tileUsed = new Tiles<int>();
        //public static Dictionary<int,Pos> tileUsed = new Dictionary<int,Pos>();
        public static List<Pos> tileUsed = new List<Pos>();

        public static int sizeX;
        public static int sizeY;

        private int negativePointX;
        private int negativePointY;
        
        private static int Get(int x, int y)
        {
            return tileUsed.FindIndex(0,tileUsed.Count,c=>c.x==x&&c.y==y);
        }

        public static void SetUsed(int x, int y)
        {
            tileUsed[Get(x, y)] = new Pos(x,y,1);
        }
        public static void SetFree(int x, int y)
        {
            tileUsed[Get(x,y)] = new Pos(x, y, 0);
        }

        /// <summary>
        /// ok so just put in the check position and the list of points. (the current selected tiles since it dies after every selection)<br></br>
        /// </summary>
        /// <returns>a picture of a weewee</returns>
        public static Pos FindNearest(Vector2Int check)
        {
            // in full honestly, if this works, this solution was thought up by an AI. i'm so scared. it's smarter than me.
            Pos pos = tileUsed.OrderBy(p => Math.Abs(p.x - check.x) + Math.Abs(p.y - check.y)).FirstOrDefault(p => !(p.Taken==1));
            return pos;
        }

        public static bool PresentAt(int x, int y)
        {
            if (tileUsed[Get(x,y)].Taken == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /*public static int[][] BoundsToMap(BoundsInt bounds)
        {
            // TODO: USE SPAN FOR THIS?? https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/tutorials/ranges-indexes
            var xRange = tileUsed.Skip(bounds.xMin).Take(bounds.xMax);
            int[][] yRange = xRange.Skip(bounds.yMin).Take(bounds.yMax).ToArray();
                          
            return yRange;
        }*/

        public PathfindExtra() // we need this so this class can remain static
        {                                                                           // !!!!!!!!
                                                                                    // ALL OBJSTACLES (MOUNTAIN, WALL, FURNITURE) ARE 1!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            sizeX = MapGenerator.mapW;
            sizeY = MapGenerator.mapH;

            for (int tx = 0; tx < MapGenerator.mapW; tx++)
            {
                for (int ty = 0; ty < MapGenerator.mapH; ty++)
                {
                    if(GameManager2D.Instance.groundTilemap.GetTile(new Vector3Int(tx,ty,0)) != null)
                        tileUsed.Add(new Pos(tx, ty, 0));
                    else
                        tileUsed.Add(new Pos(tx, ty,1));    // stupidly ez way to prevent from going into walls
                }
            }
        }
    }
}