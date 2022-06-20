using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Load all 256x256 areas. Add them to possible's from tile. because yea
/// </summary>
public class SpriteSheetCreator : MonoBehaviour
{
    public static SpriteSheetCreator I = null;
    public Texture2D TEMP_;

    protected void Awake()
    {
        if(I == null)
            I = this;
    }

    public Sprite[] createTerrainTileFromSheet(byte[] image, ref TerrainType t)
    {
        Sprite[] SPRITES;
        //t.tbase = Instantiate<RandomTile>(ScriptableObject.CreateInstance<RandomTile>());
        //t.tbase = ScriptableObject.CreateInstance<FuckBitchTile>();//
        Texture2D sheet = new Texture2D(1,1);
        sheet.LoadImage(image);
        int count = (int)(sheet.width/256) + (int)(sheet.height/256);
        SPRITES = new Sprite[count];

        FuckBitchTile bitchBase = Instantiate(ScriptableObject.CreateInstance<FuckBitchTile>());
        if (t == null)
        {
            DB.Attention("Null TerrainType in create terrain tile.");
            return null;
        }
        t.tile = bitchBase;
        for (int i = 0; i < (int)(sheet.width / 256); i++)
        {
            for(int j = 0; j < (int)(sheet.height / 256); j++)
            {
                Texture2D tex = new Texture2D(256, 256);
                tex.SetPixels(sheet.GetPixels(i*256, j*256, 256, 256));
                tex.Apply();

                Sprite spr = Sprite.Create(tex, new Rect(Vector2.zero, new Vector2(256,256)), Vector2.zero, 256 / 2); // 2: size of tilemap cells
                //SPRITES[i+j] = spr;
                //bitchBase.m_Sprites = SPRITES.ToList
                t.tile.m_Sprites.Add(spr);
                // WTF???
                //Debug.Log($"bb:{bitchBase.sprite.texture.imageContentsHash}");
            }
        }
        
        return SPRITES;
    }

    public static List<Sprite> createSpritesFromSheet(byte[] image, int size)
    {
        Texture2D sheet = new Texture2D(1, 1);
        sheet.LoadImage(image);

        List<Sprite> penisList = new List<Sprite>();

        for (int i = 0; i < (int)(sheet.width / size); i++) // size: 256 
        {
            for (int j = 0; j < (int)(sheet.height / size); j++)
            {
                Texture2D tex = new Texture2D(size, size);
                tex.SetPixels(sheet.GetPixels(i * size, j * size, size, size));
                tex.Apply();
                
                Sprite spr = Sprite.Create(tex, new Rect(Vector2.zero, new Vector2(size, size)), Vector2.zero);
                penisList.Add(spr); // null
            }
        }

        return penisList; // this line of code here returns the penis list by returning the penis list. THe penis list is then returned
    }
}
