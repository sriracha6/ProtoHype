using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RNG
{
    public static class RandomMap
    {
        // DONT MENTION SEBASTIAN. I GOT FAR ENOUGH ON MY OWN BUT LAYERING IS ASS!!!!!! and seeds i had 0 clue
        public static float[,] genNoise(int width, int height, int seed, float scale, int octaves, float persistence, float lacunarity, Vector2 offset)
        {
            float[,] map = new float[width, height];

            // ------- SEED --
            System.Random prng = new System.Random(seed);
            Vector2[] octaveOffsets = new Vector2[octaves];

            for(int i = 0; i < octaves; i++) 
            {
                float offsetX = prng.Next(-100000, 100000) + offset.x;
                float offsetY = prng.Next(-100000, 100000) + offset.y;

                octaveOffsets[i] = new Vector2(offsetX, offsetY);
            }
            // --------------
            float maxNoiseHeight = float.MinValue;
            float minNoiseHeight = float.MaxValue;

            float halfW = width / 2f;
            float halfH = height / 2f;

            if(scale<=0)
            {
                scale = 0.00001f;
            }

            for(int y = 0;y<height;y++)
            {
                for(int x=0;x<width;x++)
                {
                    float amplitude = 1f;
                    float frequency = 1f;
                    float noiseHeight = 0f;

                    for(int i = 0; i < octaves; i++)
                    {
                        float sampleX = (x-halfW) / scale * frequency + octaveOffsets[i].x;
                        float sampleY = (y-halfH) / scale * frequency + octaveOffsets[i].y;

                        float perlin = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                        noiseHeight += perlin * amplitude;

                        amplitude *= persistence;
                        frequency *= lacunarity;
                    }

                    if(noiseHeight > maxNoiseHeight)
                    {
                        maxNoiseHeight = noiseHeight;
                    } 
                    else if(noiseHeight < minNoiseHeight)
                    {
                        minNoiseHeight = noiseHeight;
                    }
                    map[x,y] = noiseHeight;
                }
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    map[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, map[x,y]);
                }
            }

                    return map;
        }
    }
}
