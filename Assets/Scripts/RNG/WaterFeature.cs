using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class WaterFeature
{
    // we need more than just the side to start.. we need positions
    public enum Side
    {
        Top,
        Bottom,
        Left,
        Right
    }

    #region River

    public struct River
    {
        public float[,] values; // 0: no river 1: river
        public Vector2 start;
        public Vector2 end;
        public Side startSide;
        public Side endSide;

        public int size;

        public River(float[,] values, Vector2 start, Vector2 end, int size, Side startside, Side endside)
        {
            this.values = values;
            this.start = start;
            this.end = end;
            this.size = size;
            this.startSide = startside;
            this.endSide = endside;
        }
    }

    private struct StartEnd
    {
        public Vector2 start;
        public Vector2 end;
        public Side startSide;
        public Side endSide;

        public StartEnd(Vector2 start, Vector2 end, Side startSide, Side endSide)
        {
            this.start = start;
            this.end = end;
            this.startSide=startSide;
            this.endSide=endSide;
        }
    }

    public static River generate1DPerlinNoise(int seed, int mapWidth, int mapHeight, int perturbation)
    {
        float[,] noise = new float[mapWidth,mapHeight];

        System.Random prng = new System.Random(seed);

        int size = prng.Next(5,15);

        StartEnd r = genStartEnd(prng, mapWidth, mapHeight);
        noise = fakePerlinWorm(perturbation, 1,r,mapWidth,mapHeight,prng);

        return new River(noise, r.start, r.end, size, r.startSide, r.endSide);
    }

    // http://www.roguebasin.com/index.php?title=Winding_ways
    // translation of https://github.com/marukrap/ProceduralMapGenerator/blob/18b5afd2bf4278679d9363382acb2d223bbde28e/Source/Generator/Generator.cpp (harder than it sounds)

    // for each value, that's the water level * x.
    private static float[,] fakePerlinWorm(int perturbation, int radius, StartEnd startend, int mapWidth, int mapHeight, System.Random r)
    {
        float[,] map = new float[mapWidth, mapHeight];

        List<Vector2> points = bresenham((int)startend.end.x, (int)startend.end.y, (int)startend.start.x, (int)startend.start.y);

        Debug.DrawLine(startend.start, startend.end, Color.magenta, 5);

        if (points.Count >= 35)
        {
            int j = 0;
            for (int i = 0; i < points.Count;i++) 
            {
                j++;
                points[j] = points[i];

                if (i < points.Count - 5 || i >= points.Count - 1)
                    i += r.Next(2, 3);
                else if (i == points.Count - 5)
                    i += 2;
                else
                    i = points.Count - 1;
            }

            points.Resize(j);
            //Vector2 mainDir = (startend.start - startend.end);//.normalized;

            if (points.Count >= 3)
            {
                const int mind2 = 2 * 2; // mindist = 2 | mind2 = 2*2
                const int maxd2 = 10 * 5; // maxdist = 5 | max2 = 5*5
                const int mincos2 = 500; // cos^2 in 1/1000, for angles < 45 degrees

                for (int i = 0; i < j * perturbation; i++) // ++i
                {
                    int ri = 1 + r.Next(j - 2);
                    Vector2 rdir = Direction.All[r.Next(8)];// +mainDir.normalized;
                    Vector2 rpos = points[ri] + rdir;

                    int lod2 = lengthSquared(rpos - points[ri - 1]);
                    int hid2 = lengthSquared(rpos - points[ri + 1]);

                    //Debug.DrawLine(rpos - points[ri-1], rpos - points[ri + 1], Color.red, 5f);

                    if (!(WCMngr.I.groundTilemap.GetTile(Vector3Int.CeilToInt(rpos)) != null
                        || lod2 < mind2 || lod2 > maxd2 ||
                        hid2 < mind2 || hid2 > maxd2))
                        continue;

                    if (signcos2(points[ri - 1], rpos, points[ri + 1]) < mincos2)
                        continue;

                    if (ri > 1 && signcos2(points[ri - 2], points[ri - 1], rpos) < mincos2)
                        continue;

                    if (ri < points.Count -2 && signcos2(rpos, points[ri + 1], points[ri + 2]) < mincos2)
                        continue;

                    points[ri] = rpos;
                }
            }

        }
        for (int i = 0; i < points.Count - 1; i++) // ++i
        {
            List<Vector2> subline = bresenham((int)points[i].x, (int)points[i].y, (int)points[i + 1].x, (int)points[i + 1].y);
            foreach (Vector2 point in subline)
            {
                //int left = (int)Mathf.Max(0, point.x - radius);
                //int top = (int)Mathf.Max(0, point.y - radius);
                //int right = (int)Mathf.Min(point.x + radius, mapWidth - 1);
                //int bottom = (int)Mathf.Min(point.y + radius, mapHeight - 1);
                for (int y = 0; y < mapHeight ; y++) // ++y
                {
                    for (int x = 0; x < mapWidth; x++) // ++x
                    {
                        if (lengthSquared(new Vector2(x, y)-point) <= radius * radius)
                        {
                            //int offset = (int)(new Vector2(x, y)-points[y]).y;
                            map[x, y] = 1;
                        }
                        else
                        {
                            //                     V what if this is a new Y value, though
                            if(y>0 && map[x,y-1] == 1) // last spot was river
                            {
                                map[x, y] = 0.5f;
                                //Debug.Log("www.:"+0.5f*map[x-1,y-1]);
                            }
                            // pointless b/c default float value is 0
                            //else
                            //{
                            //    map[x, y] = 0f;
                            //    Debug.Log("nope");
                            //}
                        }
                    }
                }
            }
        }
        for(int x = 0; x<mapWidth;x++)
        {
            string v = "";
            for(int y = 0; y<mapHeight;y++)
            {
                v+=map[x,y].ToString();
            }
            //Debug.Log(v);
        }
        return map;
    }

    private static StartEnd genStartEnd(System.Random r, int width, int height)
    {
        Vector2 start = Vector2.zero;
        Vector2 end = Vector2.zero;

        Side startSide = (Side)r.Next(0,4);
        Side endSide = (Side)r.Next(0,4);

        Debug.Log("start:"+startSide);
        Debug.Log("end:"+endSide);

        if (endSide == startSide)
        {
            endSide = (Side)3 - (int)startSide; // invert. no need for recursion
            //Debug.Log("que pro");
        }

        switch (startSide)
        {
            case Side.Top: 
            case Side.Bottom:
                start = new Vector2(0, r.Next(0, height));
                break;
            case Side.Left:
            case Side.Right:
                start = new Vector2(r.Next(0, width), 0);
                break;
        }
        switch (endSide)
        {
            case Side.Top:
            case Side.Bottom:
                end = new Vector2(0, r.Next(0, height));
                break;
            case Side.Left:
            case Side.Right:
                end = new Vector2(r.Next(0, width), 0);
                break;
        }

        Debug.DrawLine(start, end, Color.magenta);
        return new StartEnd(start, end, startSide, endSide);
    }

    // IM GENIUS
    // JUST ADD THIS TO THE HEIGHT MAP OF THE MAP

    // O Allah, I seek Your counsel through Your knowledge and I seek Your assistance through Your might
    // and I ask You from Your immense favour, for verily You alone decree our fate while I do not, and You know
    // while I do not, and You alone possess all knowledge of the Unseen.

    public static float[,] carveRiver(this float[,] ogMap, River river, float waterLevel)
    {
        float[,] newMap = ogMap;

        int width = river.values.GetLength(0);
        int height = river.values.GetLength(1);
        
        for (int x = 0; x < width; x++) 
        {
            for (int y = 0; y < height; y++) 
            {
                if(river.values[x,y] > 0)
                    newMap[x, y] = (waterLevel - 0.01f)*river.values[x,y];
            }
        }
        return newMap;
    }
    #endregion
    #region Funcs
    public static void Resize<T>(this List<T> list, int sz, T c)
    {
        int cur = list.Count;
        if (sz < cur)
            list.RemoveRange(sz, cur - sz);
        else if (sz > cur)
        {
            if (sz > list.Capacity)//this bit is purely an optimisation, to avoid multiple automatic capacity changes.
                list.Capacity = sz;
            list.AddRange(Enumerable.Repeat(c, sz - cur)); // enumerable.repeat is 86x slower than manually adding items. too bad!
        }
    }
    public static void Resize<T>(this List<T> list, int sz) where T : new()
    {
        Resize(list, sz, new T());
    }

    private static int signcos2(Vector2 p0, Vector2 p1, Vector2 p2)
    {
        try
        {
            int sqlen01 = lengthSquared(p1 - p0);
            int sqlen12 = lengthSquared(p2 - p1)+1;
            int prod = Mathf.CeilToInt((p1.x - p0.x) * (p2.x - p1.x) + (p1.y - p0.y) * (p2.y - p1.y));
            int val = 1000 * (prod * prod / (sqlen01+1)) / sqlen12;

            return prod < 0 ? -val : val;
        }
        catch (System.DivideByZeroException e)
        {
            Debug.Log(e);
            return 1;
        }
    }
    private static int lengthSquared(Vector2 v)
    {
        return Mathf.CeilToInt(v.x * v.x + v.y * v.y);
    }
    static List<Vector2> bresenham(int x1, int y1, int x2, int y2)
    {
        int m_new = 2 * (y2 - y1);
        int slope_error_new = m_new - (x2 - x1);

        List<Vector2> points = new List<Vector2>();
        for (int x = x1, y = y1; x <= x2; x++,y--) // IT WAS THE THIS I FORGOT TO DECREMENT/INCREMENT Y AAAAAAHAHAHAHAHAHAHHA
        {
            points.Add(new Vector2(x, y));

            // Add slope to increment angle formed
            slope_error_new += m_new;

            // Slope error reached limit, time to
            // increment y and update slope error.
            if (slope_error_new >= 0)
            {
                y++;
                slope_error_new -= 2 * (x2 - x1);
            }
        }
        return points;
    }
    #endregion
    #region Waterbody
    /// <summary>
    /// Instead of looping through nearby cells, we can just erode all values near water. <br></br>
    /// This results in no change for alerady existing cells, but rivers and waterfronts are eroded.
    /// </summary>
    /// <param name="ogMap">original map</param>
    /// <returns>eroded map</returns>
    public static float[,] erodeNearWater(this float[,] ogMap, float waterLevel, int erosionAmount)
    {
        int width = ogMap.GetLength(0);
        int height = ogMap.GetLength(1);

        float[,] result = ogMap;

        int iterations = 0;
        int erosionC = 0;
        bool justFound = false;

        for (int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                    // && ogMap[x,y]>waterLevel 
                if (justFound && erosionC < erosionAmount)
                {
                    for (int ix = -1; ix <= 1; ix++)
                    {
                        for (int iy = -1; iy <= 1; iy++)
                        {
                            if (ix == 0 && iy == 0)
                                continue;
                            if(ogMap[Mathf.Clamp(x+ix,0, width-1), Mathf.Clamp(y+iy, 0, height-1)] > waterLevel)
                                result[Mathf.Clamp(x + ix,0,width-1), Mathf.Clamp(y + iy,0, height-1)] = waterLevel + 0.01f + (erosionC / 10);
                        }
                    }
                    iterations++;
                    erosionC++;
                }

                if (erosionC >= erosionAmount)
                {
                    erosionC = 0;
                    justFound = false;
                }

                if (!(ogMap[x, y] <= waterLevel))
                    continue;

                justFound = true;
            }
        }

        Debug.Log("<b><i>ITERATIONS:</i></b>"+iterations);
        return result;
    }

    public static float[,] carveWaterBody(this float[,] ogMap, float waterLevel, System.Random rand)
    {
        if (rand.Next(0,15) <= 12)
            return ogMap; // 3/15 chance for water map
        
        Side side = (Side)rand.Next(0, 4);
        int size = rand.Next(ogMap.GetLength(0) / 15, ogMap.GetLength(0) / 3);

        int width = side==Side.Left||side==Side.Right ? size : ogMap.GetLength(0);
        int height = side == Side.Top||side == Side.Bottom ? size : ogMap.GetLength(1);

        float[,] newMap = ogMap;

        for (int x = 0;x<width;x++) // place beginning
        { 
            for(int y = 0; y < height; y++)
            {
                newMap[x, y] = waterLevel - 0.01f;
            }
        }
        // this is bad but it works, no?
        switch(side) // this can be much better
        {
            // repeat the value 3-5 times random
            case Side.Left:
                for(int y = 0; y<height; y++)
                {
                    int i = (int)(Mathf.PerlinNoise((float)rand.NextDouble(), y) * 10);
                    for (int c = 0; c <= i; c++) { for (int depth = 0; depth < rand.Next(3, 6); depth++) { 
                            newMap[size + c, Mathf.Clamp(y + depth, 0, height-1)] = waterLevel - 0.01f; } }
                }
                break;
            case Side.Right:
                for (int y = 0; y < height; y++)
                {
                    // -
                    int i = (int)(Mathf.PerlinNoise((float)rand.NextDouble(), y)*10);
                    for (int c = 0; c <= i; c++)
                    {
                        for (int depth = 0; depth < rand.Next(3, 6); depth++) {
                            newMap[size + c, Mathf.Clamp(y + depth,0,height-1)] = waterLevel - 0.01f; }
                    }
                }
                break;
            case Side.Top:
                for (int x = 0; x < width; x++)
                {
                    // -
                    int i = (int)(Mathf.PerlinNoise(x, (float)rand.NextDouble()) * 10);
                    for (int c = 0; c <= i; c++)
                    {
                        for (int depth = 0; depth < rand.Next(3, 6); depth++) { 
                            newMap[Mathf.Clamp(x + depth, 0, width-1), size + c] = waterLevel - 0.01f; }
                    }
                }
                break;
            case Side.Bottom:
                for (int x = 0; x < width; x++)
                {
                    int i = (int)(Mathf.PerlinNoise(x, (float)rand.NextDouble()) * 10);
                    for (int c = 0; c <= i; c++)
                    {
                        for (int depth = 0; depth < rand.Next(3, 6); depth++) { 
                            newMap[Mathf.Clamp(x + depth,0,width-1), size + c] = waterLevel - 0.01f; }
                    }
                }
                break;
        }

        return newMap;
    }
#endregion
}
