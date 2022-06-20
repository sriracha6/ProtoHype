using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Direction
{
    public static Vector2 None = new Vector2(0, 0);
    public static Vector2 N = new Vector2(0, -1);
    public static Vector2 NE = new Vector2(1, -1);
    public static Vector2 E = new Vector2(1, 0);
    public static Vector2 SE = new Vector2(1, 1);
    public static Vector2 S = new Vector2(0, 1);
    public static Vector2 SW = new Vector2(-1, 1);
    public static Vector2 W = new Vector2(-1, 0);
    public static Vector2 NW = new Vector2(-1, -1);

    private static Vector2[] allDirs = { N, NE, E, SE, S, SW, W, NW };
    public static List<Vector2> All = new List<Vector2>(allDirs);
}
