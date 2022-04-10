using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static MapGenerator;

public static class TextureGenerator
{
    public static Texture2D texturePreviewFromMap(Color[] map, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, true);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(map);
        texture.Apply();
        return texture;
    }

    public static Texture2D textureFromHeightMap(float[,] heightMap)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        Color[] colorMap = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
            }
        }

        return texturePreviewFromMap(colorMap, width, height);
    }
    //    i just realized - if you don't fill in a tile, it's transparent.
    //    instead of finding points and neighbors and going through appx. 
    //    78 for loops, you can just place one plane under. wtf. gotta
    //    make a READONLYSPAN for water positions though. (depth)
}
