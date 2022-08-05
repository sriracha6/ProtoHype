using System.Linq;
using System.Reflection;
using UnityEngine;
using Haze;
using System.Collections.Generic;

public class TilemapShadow : MonoBehaviour
{
    [Space]
    public float height;
    public MeshFilter targetMesh;
    public MeshRenderer targetRenderer;

    public void Generate()
    {
        DestroyAllChildren();

        //Vector3[] pathVertices = new Vector3[tilemapCollider.GetPathPointCount(i)];
        //Vector2[] pathVertices2D = new Vector2[tilemapCollider.GetPathPointCount(i)];
        //int s = tilemapCollider.GetPath(i, pathVertices);
        //GameObject shadowCaster = new GameObject("shadow_" + i);
        //shadowCaster.transform.parent = gameObject.transform;
        //MeshFilter mesh = shadowCaster.GetComponent<MeshFilter>();

        /*List<Triangulator.Triangle> triangles = Triangulator.Triangulate(pathVertices2D.ToList());
        List<Vector3> verts = new List<Vector3>();//pathVertices.ToList();
        List<int> indices = new List<int>();
        //for(int b = 0; b < verts.Count; b++) { indices.Add((int)verts[b].x); }
        //indices.Reverse();

        //Triangulator.AddTrianglesToMesh(ref verts,ref indices,triangles,0,false); 
        */

        // Debug.Log("Generate");

    }

    /*public void DrawMesh(MapMeshWH mdata, int mapW, int mapH)
    {
        targetMesh.gameObject.transform.position = new Vector3(mapW / 2, mapH / 2, Player.mountainTmap.transform.position.z-0.1f);
        targetMesh.sharedMesh = mdata.CreateMesh();
    }*/

    public void DestroyAllChildren()
    {

        var tempList = transform.Cast<Transform>().ToList();
        foreach (var child in tempList)
        {
            //DestroyImmediate(child.gameObject);
            Destroy(child.gameObject);
        }

    }

}