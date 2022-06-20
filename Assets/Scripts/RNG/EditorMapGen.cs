using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class EditorMapGen : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator mapGen = (MapGenerator)target;

        if (DrawDefaultInspector())
        {
            if(mapGen.autoUpdate)
                mapGen.GenMap();
        }

        Texture2D theText = AssetPreview.GetAssetPreview(mapGen.currentTexture);
        GUILayout.Label(theText);

        if (GUILayout.Button("Generate"))
        {
            mapGen.seed = Random.Range(int.MinValue, int.MaxValue).ToString();
            mapGen.GenMap();
        }
    }
}
