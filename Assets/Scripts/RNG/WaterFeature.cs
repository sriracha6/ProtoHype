using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Side
{
    Top = 0,
    Bottom = 1,
    Left = 2,
    Right = 3
}

public static class WaterFeature
{
    // we need more than just the side to start.. we need positions

    public struct River
    {
        public Vector2 start;
        public Vector2 end;
        public int radius;

        public River(Vector2 start, Vector2 end, int radius)
        {
            this.start = start;
            this.end = end;
            this.radius = radius;
        }
    }

    public const int MIN_ITERATIONS = 30;
    public const int RIVER_CHANCE = 10;
    public const int CHANGE_SIZE_CHANCE = 20;
    public const int MAX_BREADTH = 7;
    public const int MIN_BREADTH = 3;

    public static float[,] carveRiver(this float[,] ogMap, float waterLevel, System.Random seed, float perturbation, bool always = false)
    {
        if(!always)
            if (seed.Next(0, 100) >= 100-RIVER_CHANCE)
                return ogMap;

        float[,] newMap = ogMap;

        int width = ogMap.GetLength(0);
        int height = ogMap.GetLength(1);

        Debug.Log($"{width},{height}");

        River river = GenerateRiver(seed, height, height);

        Vector2 currentPosition = river.start;
        Vector2 truedir = river.end - river.start;

        bool negate = false; // dunno why this works

        int it = 0;
        while (true) 
        {
            Vector2 point = RandomPointInCircle(seed);
            if (negate) point = -point;
            Vector2 direction = currentPosition + ((point * perturbation) + truedir.normalized).normalized;
            currentPosition = direction;
            if (currentPosition.x < 0 || currentPosition.x >= width)
            {
                if (!(it <= MIN_ITERATIONS)) // unlucky start
                {
                    Debug.Log($"Breaking x after {it} iterations. {currentPosition}");
                    break;
                }
                else
                {
                    currentPosition = new Vector2(
                        Mathf.Clamp(currentPosition.x, 0, width-1),
                        Mathf.Clamp(currentPosition.y, 0, height-1)
                    );
                    negate = true;
                }
            }
            if (currentPosition.y < 0 || currentPosition.y >= height)
            {
                if (it >= MIN_ITERATIONS) // unlucky start
                {
                    Debug.Log($"Breaking y after {it} iterations. {currentPosition}");
                    break;
                }
                else 
                { 
                    negate = true;
                    currentPosition = new Vector2(
                        Mathf.Clamp(currentPosition.x, 0, width - 1),
                        Mathf.Clamp(currentPosition.y, 0, height - 1)
                    );
                }
            }

            for(int i = 0; i < river.radius * 2; i++)
            {
                if (i % 2 == 0)
                    newMap[(int)currentPosition.x, (int)((currentPosition.y + i).ClampInMap(height))] = waterLevel - 0.01f;
                else
                    newMap[(int)currentPosition.x, (int)((currentPosition.y - i).ClampInMap(height))] = waterLevel - 0.01f;
            }

            if (seed.Next(0, 100) >= 100 - CHANGE_SIZE_CHANCE)
            {
                int x = seed.Next(0, 100);
                if (x >= 50 && river.radius <= MAX_BREADTH)
                    river.radius++;
                else if (x < 50 && river.radius > MIN_BREADTH)
                    river.radius--;
            }

            newMap[(int)currentPosition.x, (int)currentPosition.y] = waterLevel - 0.01f;
            it++;
        }

        return newMap;
    }

    public static Vector2 RandomPointInCircle(System.Random prng)
    {
        float angle = (float)(prng.NextDouble() * Mathf.PI * 2);
        float x = Mathf.Sin(angle) * 1;
        float y = Mathf.Cos(angle) * 1;

        return new Vector2(x, y);
    }

    public static int ClampInMap(this float x, int max)
    {
        return Mathf.Clamp((int)x, 0, max - 1);
    }

    public static River GenerateRiver(System.Random seed, int width, int height)
    {
        System.Random prng = seed;

        Side startSide = (Side)prng.Next(0, 4);
        Side endSide = (Side)prng.Next(0, 4);
        if (endSide == startSide)
            endSide = (Side)(4 - (int)startSide); // this is very dumb

        Vector2 st = Vector2.negativeInfinity; Vector2 end = Vector2.negativeInfinity;

        if (startSide == Side.Top) st = new Vector2(prng.Next(0, width+1), height+1);
        else if (startSide == Side.Bottom) st = new Vector2(prng.Next(0, width+1), 0);
        else if (startSide == Side.Left) st = new Vector2(0, prng.Next(0, height+1));
        else if (startSide == Side.Right) st = new Vector2(width, prng.Next(0, height+1));

        if (endSide == Side.Top) end = new Vector2(prng.Next(0, width+1), height+1);
        else if (endSide == Side.Bottom) end = new Vector2(prng.Next(0, width+1), 0);
        else if (endSide == Side.Left) end = new Vector2(0, prng.Next(0, height+1));
        else if (endSide == Side.Right) end = new Vector2(width, prng.Next(0, height+1));

        River river = new River(st, end, prng.Next(2,5));
        return river;
    }

    /// <summary>
    /// Instead of looping through nearby cells, we can just erode all values near water. <br></br>
    /// This results in no change for alerady existing cells, but rivers and waterfronts are eroded.
    /// </summary>
    /// <param name="ogMap">original map</param>
    /// <returns>eroded map</returns>
    public static float[,] erodeNearWater(this float[,] ogMap, float waterLevel, int erosionAmount)
    {
        float[,] result = ogMap;
#if false // quando rongo
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

        Debug.Log("<b>EROSION ITERATIONS:</b>"+iterations);
#endif
        return result;
    }
    // todo: this can be easily made way more natural instead of straight lines by expanding on the creating start part. then you have perlin noise to add lotsa variation to the straight line. you have a list of offset's off the line so you know how to add to the end.
    public static float[,] carveWaterBody(this float[,] ogMap, float waterLevel, System.Random rand, bool overrideRandom=false)
    {
        if (!overrideRandom && rand.Next(0,15) <= 13)
            return ogMap; // 2/15 chance for water map
        
        float[,] newMap = ogMap;

        Side side = RandomEnum<Side>(rand);
        int size = rand.Next(ogMap.GetLength(0) / 15, ogMap.GetLength(0) / 3);

        int width = side == Side.Left||side==Side.Right ? size : ogMap.GetLength(0);
        int height = side == Side.Top||side == Side.Bottom ? size : ogMap.GetLength(1);

        int startX = 0; int startY = 0;
        int endY = ogMap.GetLength(1); int endX = ogMap.GetLength(0);

        if (side == Side.Left)
            endX = width;
        else if (side == Side.Right)
            startX = ogMap.GetLength(0) - size;
        else if (side == Side.Top)
            startY = ogMap.GetLength(1) - size;
        else if (side == Side.Bottom)
            endY = height;

        for (int x = startX;x<endX;x++) // place beginning
            for(int y = startY; y < endY; y++)
                newMap[x, y] = waterLevel - 0.01f;

        float offset = rand.Offset();
        switch (side) // this is fucking terrible but fuck you deal with it 40 lines of bs
        {
            // repeat the value 3-5 times random
            case Side.Left:
                for (int y = startY; y < endY; y++)
                {
                    int i = (int)(Mathf.PerlinNoise(offset, y) * 10);
                    for (int c = 0; c <= i; c++)
                    {
                        for (int depth = 0; depth < rand.Next(3, 6); depth++)
                            newMap[size + depth, y] = waterLevel - 0.01f;
                    }
                }
                break;
            case Side.Right:
                for (int y = 0; y < endY; y++)
                {
                    int i = (int)(Mathf.PerlinNoise(offset, y)*10);
                    for (int c = 0; c <= i; c++)
                    {
                        for (int depth = 0; depth < rand.Next(3, 6); depth++)
                            newMap[startX - depth, y] = waterLevel - 0.01f;
                    }
                }
                break;
            case Side.Top:
                for (int x = 0; x < endX; x++)
                {
                    int i = (int)(Mathf.PerlinNoise(x, offset) * 10);
                    for (int c = 0; c <= i; c++)
                    {
                        for (int depth = 0; depth < rand.Next(3, 6); depth++)
                            newMap[x, startY - depth] = waterLevel - 0.01f;
                    }
                }
                break;
            case Side.Bottom:
                for (int x = startX; x < endX; x++)
                {
                    int i = (int)(Mathf.PerlinNoise(x, offset) * 10);
                    for (int c = 0; c <= i; c++)
                    {
                        for (int depth = 0; depth < rand.Next(3, 6); depth++)
                            newMap[Mathf.Clamp(x + depth,0,width-1), size + c] = waterLevel - 0.01f;
                    }
                }
                break;
        }

        return newMap;
    }

    public static float Offset(this System.Random rand)
    {
        return (float)(rand.NextDouble() * rand.Next(-100000,100000));
    }

    static T RandomEnum<T>(System.Random r)
    {
        var v = System.Enum.GetValues(typeof(T));
        return (T)v.GetValue(r.Next(v.Length));
    }
}
