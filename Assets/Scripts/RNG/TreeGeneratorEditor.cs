using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(TreeGenerator))]
public class TreeGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TreeGenerator treeGen = (TreeGenerator)target;

        if (DrawDefaultInspector())
        {
            if (treeGen.autoUpdate)
                treeGen.Generate();
        }

        Texture2D theText = AssetPreview.GetAssetPreview(treeGen.currentTexture);
        GUILayout.Label(theText);

        if (GUILayout.Button("Generate"))
        {
            treeGen.Generate();
        }
        if (GUILayout.Button("Save Config"))
        {
            //treeGen.Generate();
        }
    }
}
#endif