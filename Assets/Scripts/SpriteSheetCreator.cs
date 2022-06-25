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
    public RuleTile defaultRuleTile;

    protected void Awake()
    {
        if(I == null)
            I = this;
    }

    public FuckBitchTile createTerrainTileFromSheet(byte[] image)
    {
        Sprite[] SPRITES;
        //t.tbase = Instantiate<RandomTile>(ScriptableObject.CreateInstance<RandomTile>());
        //t.tbase = ScriptableObject.CreateInstance<FuckBitchTile>();//
        Texture2D sheet = new Texture2D(1,1);
        sheet.LoadImage(image);
        int count = (int)(sheet.width/256) + (int)(sheet.height/256);
        SPRITES = new Sprite[count];

        FuckBitchTile bitchBase = Instantiate(ScriptableObject.CreateInstance<FuckBitchTile>());
        //if (t == null)
        //{
        //    DB.Attention("Null TerrainType in create terrain tile.");
        //    return null;
        //}
        //t.tile = bitchBase;
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
                bitchBase.m_Sprites.Add(spr);
                // WTF???
                //Debug.Log($"bb:{bitchBase.sprite.texture.imageContentsHash}");
            }
        }
        
        return bitchBase;
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
    
    public RuleTile createRuleTile(Buildings.Building building)
    {
        var x = Instantiate(defaultRuleTile);
        for (int i = 0; i < x.m_TilingRules.Count; i++)
            x.m_TilingRules[i].m_Sprites = new Sprite[] { CachedItems.renderedWalls.Find(x => x.ID == building.ID).sprites[i] };
        return x;
    }

    static Texture2D CombineTextures(Texture2D _textureA, Texture2D wallTex, Color groutColor)
    {
        //Create new textures
        Texture2D textureResult = new Texture2D(_textureA.width, _textureA.height, TextureFormat.ARGB32, true);
        //create clone form texture
        //textureResult.SetPixels(_textureB.GetPixels());
        //Now copy texture B in texutre A
        int co = 0;
        for (int x = 0; x < _textureA.width; x++)
        {
            for (int y = 0; y < _textureA.height; y++)
            {
                Color c = _textureA.GetPixel(x, y);
                if (c.a > 0.0f) //Is not transparent
                {
                    co++;
                    //Copy pixel color in TexturaA
                    textureResult.SetPixel(x, y, c);
                    if (c.r == 255 && c.g == 0 && c.b == 255)
                    {
                        Color ccolor;
                        ccolor = wallTex.GetPixel(x % wallTex.width, y % wallTex.height);
                        textureResult.SetPixel(x, y, ccolor);
                    }
                    if (c.r == 255 && c.g == 255 && c.b == 255)
                        textureResult.SetPixel(x, y, groutColor);
                }
            }
        }
        textureResult.Apply();
        return textureResult;
    }
}
