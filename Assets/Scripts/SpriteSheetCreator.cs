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
    public Sprite unknownRule;

    protected void Awake()
    {
        if(I == null)
            I = this;
    }
    // im a genius create tile blending by making the edges 50% transparent and make the ppu too small
    public FuckBitchTile createTerrainTileFromSheet(byte[] image)
    {
        //t.tbase = Instantiate<RandomTile>(ScriptableObject.CreateInstance<RandomTile>());
        //t.tbase = ScriptableObject.CreateInstance<FuckBitchTile>();//
        Texture2D sheet = new Texture2D(1, 1)
        {
            filterMode = FilterMode.Point
        };
        sheet.LoadImage(image);
        sheet.Apply();

        //int count = (int)(sheet.width/256) + (int)(sheet.height/256);

        FuckBitchTile bitchBase = Instantiate(ScriptableObject.CreateInstance<FuckBitchTile>());
        for (int i = 0; i < sheet.width / 256; i++)
        {
            for(int j = 0; j < sheet.height / 256; j++)
            {
                Texture2D tex = new Texture2D(256, 256, TextureFormat.RGB24, true)
                {
                    filterMode = FilterMode.Point
                };
                tex.wrapMode = tex.wrapModeU = tex.wrapModeV = tex.wrapModeW = TextureWrapMode.Clamp;
#pragma warning disable UNT0017
                tex.SetPixels(sheet.GetPixels(i*256, j*256, 256, 256));
#pragma warning restore UNT0017
                /*
                for(int x = 0; x < 256; x++)
                {
                    for(int y = 0; y < 256; y++)
                    {
                        int checkCount = 0;
                        if (x <= 13) checkCount++;
                        if (x >= 256 - 13) checkCount++;
                        if (y <= 13) checkCount++;
                        if (y >= 256 - 13) checkCount++;

                        if (checkCount == 1)
                        {
                            float multipler = 1f / 13f;
                            var c = tex.GetPixel(x, y);
                            tex.SetPixel(x, y, new Color(c.r*0.4f, c.g*0.4f, c.b*0.4f, 0.4f * multipler));
                        }
                        else if (checkCount != 0)
                        {
                            tex.SetPixel(x, y, new Color(0,0,0,0f));
                        }
                    }
                }*/

                tex.Apply();
                                                                                                            // 230 /2
                Sprite spr = Sprite.Create(tex, new Rect(Vector2.zero, new Vector2(256,256)), Vector2.zero, 256 / 2, 16); // 2: size of tilemap cells
                bitchBase.m_Sprites.Add(spr);
            }
        }
        
        return bitchBase;
    }

    public static List<Sprite> createSpritesFromSheet(byte[] image, int size, int ppu)
    {
        Texture2D sheet = new Texture2D(1, 1);
        sheet.LoadImage(image);

        List<Sprite> penisList = new List<Sprite>();

        for (int i = 0; i < (int)(sheet.width / size); i++) // size: 256 
        {
            for (int j = 0; j < (int)(sheet.height / size); j++)
            {
                Texture2D tex = new Texture2D(size, size);
#pragma warning disable UNT0017
                tex.SetPixels(sheet.GetPixels(i * size, j * size, size, size));
#pragma warning restore UNT0017
                tex.Apply();
                
                Sprite spr = Sprite.Create(tex, new Rect(Vector2.zero, new Vector2(size, size)), Vector2.zero, ppu);
                penisList.Add(spr); // null
            }
        }
        return penisList; // this line of code here returns the penis list by returning the penis list. THe penis list is then returned
    }
    
    public static Tile[,] createMutliTile(Texture2D tex)
    {
        var asd = new Tile[tex.width/512, tex.height/512];
        
        for(int i = 0; i < asd.GetLength(0); i++)
        {
            for(int j = 0; j < asd.GetLength(1); j++)
            {
                var tile = Instantiate(ScriptableObject.CreateInstance<Tile>());
                Texture2D a = new Texture2D(512,512);
                a.SetPixels(tex.GetPixels(i * 512, j * 512, 512, 512));
                a.Apply();
                tile.sprite = Sprite.Create(a, new Rect(0, 0, 512, 512), Vector2.zero, 512 / 2, 16); ;
                asd[i, j] = tile;
            }
        }

        return asd;
    }

    public RuleTile createRuleTile(Buildings.Building building)
    {
        var x = Instantiate(defaultRuleTile);
        for (int i = 0; i < x.m_TilingRules.Count - 1; i++)
            x.m_TilingRules[i].m_Sprites = new Sprite[] { CachedItems.renderedWalls.Find(x => x.ID == building.ID).sprites[i] };
        x.m_TilingRules[x.m_TilingRules.Count - 1].m_Sprites = new Sprite[] { unknownRule };
        return x;
    }
}
