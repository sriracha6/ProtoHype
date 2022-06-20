using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// kill me please
/// </summary>
public static class MeshGenerator
{
    /*  things i've tried:
     *      finding clumps in the array and making GOs from it
     *      creating one mesh
     *      submeshes
     *      making certain vertexes transparent
     *      making meshes for each layer
     *      and now...
     *          let's try UVs... it means one draw call but it 
     *          means i have to make a texture atlas which isn't
     *          a bad thing it's just that i'm lazy and that's hard
     *          i'll just have to do mountains later. aaaaaaaaaaaaa.
     *          and water... aaaaaaaaa... that's if this even works
     */
/*    public static List<MapMeshWH> GenerateTerrainMesh(float[,] heightMap, List<TerrainType> ttypes)//, float heightMultiplier)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        List<TerrainType> types = new List<TerrainType>(ttypes);
        types = types.OrderBy(x=>x.height).ToList();

        List<MapMeshWH> meshes = new List<MapMeshWH>();

        foreach (TerrainType t in types)
        {
            MapMeshWH cur = new MapMeshWH(width, height, t.index);
            meshes.Add(cur);                                                                    
        }

        float topLeftX = (width - 1) / -2f;////
        float topLeftZ = (height - 1) / 2f;

        int vertexIndex = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float hght = 0;// ttypes.Find(f => f.height <= heightMap[x, y]).index;
                for (int i = 0; i < types.Count; i++) // for some ungodly reason i have to use this instead of .find
                {                                     // thats prolly fine bc system namespace functions are shitty AF.
                    if (heightMap[x,y] <= types[i].height)
                    {
                        hght = types[i].index;
                        break;
                    }
                }
                
                MapMeshWH meshdata = meshes.Find(f => f.index == hght);

                meshdata.verts[vertexIndex] = new Vector3(topLeftX + x, topLeftZ - y, 0);
                meshdata.uvs[vertexIndex] = new Vector2((float)x / width, (float)y / height);

                if (x < width - 1 && y < height - 1)
                {
                    meshdata.AddTri(vertexIndex, vertexIndex + width + 1, vertexIndex + width);
                    meshdata.AddTri(vertexIndex + width + 1, vertexIndex, vertexIndex + 1);
                }
                vertexIndex++;
                //Debug.Log(hght+" = "+meshdata.index + " <= " + heightMap[x,y]);

                //meshdata.transparents[x, y] = false;
            }
        }
        return meshes;
    }

    public class MapMeshWH
    {
        public Vector3[] verts;
        public int[] triangles;
        public Vector2[] uvs;
        //public bool[,] transparents; // trans parents

        public float index;

        public List<MapMeshWH> submeshes = new List<MapMeshWH>();

        public int width;
        public int height;

        int triangleIndex;

        public MapMeshWH(int width, int height, float index)
        {
            this.width = width;
            this.height = height;

            verts = new Vector3[width * height];
            uvs = new Vector2[width * height];
            triangles = new int[(width * height) * 6];
            /*transparents = new bool[width, height];
            for(int i = 0; i<width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    transparents[i,j] = true;
                }
            }*/
/*            this.index = index;
        }

        public void AddTri(int a, int b, int c)
        {
            triangles[triangleIndex] = a;
            triangles[triangleIndex + 1] = b;
            triangles[triangleIndex + 2] = c;
            triangleIndex += 3;
        }

        public Mesh CreateMesh() // todo: set stuff to null after done, for garbage collector
        { // vertex color doesnt work FUCk 
            Mesh mesh = new Mesh();
            mesh.indexFormat = IndexFormat.UInt32; // if you have 4 fucking billion vertexes you have other problems

            mesh.vertices = verts;
            mesh.triangles = triangles;
            mesh.uv = uvs;

            mesh.RecalculateNormals();
            mesh.Optimize(); // ?
            return mesh;
        }
    }*/
}