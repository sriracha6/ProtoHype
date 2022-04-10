using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Load all 256x256 areas. Add them to possible's from tile. because yea
/// </summary>
public class SpriteSheetCreator : MonoBehaviour
{
    public static SpriteSheetCreator Instance;
    public Texture2D TEMP_;

    protected void Awake()
    {
        Instance = this;
    }

    // the singleton approach is so unbelievably sexy
    public void createTerrainTileFromSheet(byte[] image, ref TerrainType t, string name)
    {
        t.tbase = Instantiate<RandomTile>(ScriptableObject.CreateInstance<RandomTile>());
        Texture2D sheet = new Texture2D(1,1);
        sheet.LoadImage(image);

        for(int i = 0; i < (int)(sheet.width / 256); i++)
        {
            for(int j = 0; j < (int)(sheet.height / 256); j++)
            {
                Texture2D tex = new Texture2D(256, 256);
                tex.SetPixels(sheet.GetPixels(i*256, j*256, 256, 256));
                tex.Apply();

                Sprite spr = Sprite.Create(tex, new Rect(Vector2.zero, new Vector2(256,256)), Vector2.zero);
                t.tbase.m_Sprites.Add(spr); // null
            }
        }
        Debug.Log($"peeny ween: {t.tbase.m_Sprites.Count} NAME:{name}");
    }

    public static List<Sprite> createSpritesFromSheet(byte[] image)
    {
        Texture2D sheet = new Texture2D(1, 1);
        sheet.LoadImage(image);

        List<Sprite> penisList = new List<Sprite>();

        for (int i = 0; i < (int)(sheet.width / 256); i++)
        {
            for (int j = 0; j < (int)(sheet.height / 256); j++)
            {
                Texture2D tex = new Texture2D(256, 256);
                tex.SetPixels(sheet.GetPixels(i * 256, j * 256, 256, 256));
                tex.Apply();
                
                Sprite spr = Sprite.Create(tex, new Rect(Vector2.zero, new Vector2(256, 256)), Vector2.zero);
                penisList.Add(spr); // null
            }
        }

        return penisList; // this line of code here returns the penis list by returning the penis list. THe penis list is then returned
    }
}
