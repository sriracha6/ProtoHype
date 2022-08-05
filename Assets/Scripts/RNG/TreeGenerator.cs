using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Color = UnityEngine.Color;

public class TreeGenerator : MonoBehaviour
{
    public bool autoUpdate;
    [Header("Current Tree")]
    public string Name;
    [Multiline]
    public string Description;
    [Space]
    [Header("Branch Setting")]
    public int minBranches;
    public int maxBranches;
    public int branchDepth;
    public int branchThickness;
    [Range(0f,360f)] public int angle;
    [Header("Trunk")]
    public int trunkHeight;
    public int trunkThickness;
    public int trunkOffset;
    [Range(0f,512f)]public int trunkAngle; // make this sometimes negative to reverse ti
    [Space]
    [Header("Colors")]
    [ColorUsage(false)] public Color trunkColor;
    [ColorUsage(false)] public Color branchColor;
    public Color leafColor;

    [HideInInspector] public Texture2D currentTexture;

    // instead of doing the dumb math tree, we can do a splot of leaves in the
    // correct branch areas. now we only need to do branches
    public void Generate()
    {
        Texture2D result = new Texture2D(512,512);

        Color[] trunk = new Color[512 * 512]; //Color[] trunk = new Color[(trunkThickness*trunkAngle)* trunkHeight];
        // ---- draw trunk ---- 
        // we can just offset each one by an angle up to trunkAngle times. lol ez bezier curve deez nutz lmao shitty ass algrorithm get some bitchs on yo dick then we can talk 
        for (int i = 0; i < trunk.Length; i++) { trunk[i] = Color.clear; }

/*        float offsetPerPixel = trunkHeight / trunkAngle;
        Debug.Log(offsetPerPixel);
        
        for (int x = 0; x < 512; x++)
        {
            for (int y = 0; y < 512; y++)
            {
                trunk[x + y] = trunkColor;
                trunk[(int)( x+(offsetPerPixel*y)
                            +y)] = trunkColor;
            }
        }
        result.SetPixels( ((512/2) - (trunkThickness+trunkAngle) / trunkAngle*2) + trunkOffset, 0,  // pos
                         trunkThickness+trunkAngle,trunkHeight,trunk);                      // thickness + color*/

        // ---- draw branches ----
        
        
        // ----
        result.Apply();
        currentTexture = result;
    }

    protected void OnValidate()
    {
        if(minBranches > maxBranches)
            minBranches = maxBranches;
        if(branchDepth < 1)
            branchDepth = 1;
        //if (trunkAngle + trunkOffset + trunkThickness > 512)
        //    trunkAngle = trunkAngle + trunkThickness + trunkOffset;
    }
}
