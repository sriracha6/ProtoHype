using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PawnFunctions;

using System.IO;
using System.Linq;
using System;
using Armors;
using Body;
//using System.Drawing;

//using Graphics = System.Drawing.Graphics;
// WE CANT USE THIS BECAUSE UNITY IS SEXIST

using static CachedItems;

/// <summary>
/// How the armor renderer works:
///     Step 1.
///     Take the default pawn texture.
///     Add armor.
///     Armor is just armor laid over default pawn texture so that it's infinitely extensible and bug-free. (it's the user's fault)
///     Render.
///     Step 2. ????
///     Step 3. Profit
///     
///     also prolly should've just cached everything used in save file right away but thats for later me ig
/// </summary>
public class PawnRenderer : MonoBehaviour
{
    [Header("Settings")]
    public int WeaponSize = 512;
    [Space]
    [Header("Components")]
    public Pawn p;   
    public SpriteRenderer weapon;
    public SpriteRenderer shield;
    public Texture2D TEX;

    void Start()
    {

        if(p.hasPrimary)
            weapon.sprite = createSprite(
                imageFromWeaponName(p.heldPrimary.Name,p.heldPrimary.Type),p.heldPrimary.Name, p.heldPrimary.size, Loadtype.Weapon);
        if (p.hasSidearm)
            createSprite(
                imageFromWeaponName(p.heldSidearm.Name, p.heldSidearm.Type), p.heldSidearm.Name, p.heldSidearm.size, Loadtype.Weapon);

        if (p.hasShield)
            shield.sprite = 
                createSprite(imageFromShieldName(p.shield.Name), p.shield.Name, p.shield.size, Loadtype.Shield);

        foreach(Armor a in p.armor) // todo : please try and get rid of this
        {
            createSprite(imageFromArmorName(a.Name), a.Name, 1, Loadtype.Armor);
        }

        p.sprite.sprite = renderAvatar(p.armor);
        TEX = p.sprite.sprite.texture;
    } 

    private Sprite createSprite(Texture2D image, string backup, float size, Loadtype ltype)
    {
        try
        {
            bool exists = false;

            if (ltype == Loadtype.Weapon)
                exists = !CachedItems.renderedWeapons.Exists(x => x.name == backup);
            else if (ltype == Loadtype.Shield)
                exists = !CachedItems.renderedShields.Exists(x => x.name == backup);
            else if(ltype == Loadtype.Armor)
                exists = !CachedItems.renderedArmors.Exists(x => x.name == backup);
            if (exists)
            {
                Sprite spr = Sprite.Create(image, new Rect(Vector2.zero // weapon??
                    , new Vector2(image.width, image.height)), Vector2.zero, WeaponSize / size);

                if (ltype == Loadtype.Weapon)
                {
                    CachedItems.renderedWeapons.Add(new RenderedWeapon(spr, backup));
                }
                else if (ltype == Loadtype.Shield)
                {
                    CachedItems.renderedShields.Add(new RenderedShield(spr, backup));
                }
                else
                {
                    CachedItems.renderedArmors.Add(new RenderedArmor(spr, backup));
                }
                return spr;
            }
            else
            {
                if (ltype == Loadtype.Weapon)
                    return CachedItems.renderedWeapons.Find(x => x.name == backup).sprite;
                else if (ltype == Loadtype.Shield)
                    return CachedItems.renderedShields.Find(x=>x.name==backup).sprite;
                else 
                    return CachedItems.renderedArmors.Find(x => x.name == backup).sprite;
            }
        }
        catch(Exception e)
        {
            Debug.LogError($"error creating sprite {backup} \n {ltype}\n {size} \n w:{image.width} h{image.height} \n {e}");
            return null;
        }
    }
    private Texture2D imageFromWeaponName(string name, Weapons.WeaponType type)
    {
        //                          TODO: MAKE THIS DYNAMC V AND NOT LOAD EVERY TIME YOU MAKE A NEW PAWN!!!!!!!!@!!@!@!@!
        try
        {
            if (!CachedItems.renderedWeapons.Exists(x => x.name == name))
            {
                var rawData = System.IO.File.ReadAllBytes($"C:\\Users\\frenz\\Music\\assets\\weapons\\{type}\\{name}.png"); // todo: it does this every time you load a pawn? how about no.
                Texture2D tex = new Texture2D(512, 512);
                tex.LoadImage(rawData);
                //tex.Resize(tex.width, tex.height); // what a devilish line of code! we don't need to resize if we already set the size that EVERY FILE is!
                tex.Apply();
                return tex;
            }
            else
            {
                return CachedItems.renderedWeapons.Find(x=>x.name==name).sprite.texture;
            }
        }catch(System.IO.IOException e)
        {
            Debug.LogError("Couldn't load weapon of name "+name+"\n\n"+e);
        }
        return null;
    }
    private Texture2D imageFromShieldName(string name)
    {
        //                          TODO: MAKE THIS DYNAMC V AND NOT LOAD EVERY TIME YOU MAKE A NEW PAWN!!!!!!!!@!!@!@!@!
        try
        {
            if (!CachedItems.renderedShields.Exists(x => x.name == name))
            {
                var rawData = System.IO.File.ReadAllBytes($"C:\\Users\\frenz\\Music\\assets\\armor\\shields\\{name}.png"); // todo: it does this every time you load a pawn? how about no.
                Texture2D tex = new Texture2D(512, 512);
                tex.LoadImage(rawData);
                //tex.Resize(tex.width, tex.height); // what a devilish line of code! we don't need to resize if we already set the size that EVERY FILE is!
                tex.Apply();
                return tex;
            }
            else
            {
                return CachedItems.renderedShields.Find(x=>x.name==name).sprite.texture;
            }
        }
        catch (System.IO.IOException e)
        {
            Debug.LogError("Couldn't load shield of name " + name + "\n\n" + e);
        }
        return null;
    }
    private Texture2D imageFromArmorName(string name)
    {
        //                          TODO: MAKE THIS DYNAMC V AND NOT LOAD EVERY TIME YOU MAKE A NEW PAWN!!!!!!!!@!!@!@!@!
        try
        {
            if (!CachedItems.renderedArmors.Exists(x => x.name == name))
            {
                var rawData = System.IO.File.ReadAllBytes($"C:\\Users\\frenz\\Music\\assets\\armor\\armor\\{name}.png"); // todo: it does this every time you load a pawn? how about no.
                Texture2D tex = new Texture2D(512, 512, TextureFormat.ARGB32, true);    
                tex.LoadImage(rawData);
                //tex.Resize(tex.width, tex.height); // what a devilish line of code! we don't need to resize if we already set the size that EVERY FILE is!
                tex.Apply();
                return tex;
            }
            else
            {
                return CachedItems.renderedArmors.Find(x=>x.name==name).sprite.texture;
            }
        }
        catch (System.IO.IOException e)
        {
            Debug.LogError("Couldn't load armor of name " + name + "\n\n" + e);
        }
        return null;
    }

    public static Sprite getProjectile(string name)
    {
        //                          TODO: MAKE THIS DYNAMC V AND NOT LOAD EVERY TIME YOU MAKE A NEW PAWN!!!!!!!!@!!@!@!@!
        try
        {
            if (CachedItems.renderedProjectiles.Exists(x => x.name == name))
                return CachedItems.renderedProjectiles.Find(x=>x.name==name).sprite;
            else
            {
                var rawData = System.IO.File.ReadAllBytes($"C:\\Users\\frenz\\Music\\assets\\weapons\\Projectile\\{name}.png"); // todo: it does this every time you load a pawn? how about no.
                Texture2D tex = new Texture2D(512, 512);
                tex.LoadImage(rawData);
                //tex.Resize(tex.width, tex.height); // what a devilish line of code! we don't need to resize if we already set the size that EVERY FILE is!
                tex.Apply();

                Sprite spr = Sprite.Create(tex, new Rect(Vector2.zero
                , new Vector2(tex.width, tex.height)), Vector2.zero);
                CachedItems.renderedProjectiles.Add(new RenderedProjectile(spr, name));
                return spr;
            }
        }
        catch (System.IO.IOException e)
        {
            Debug.LogError("Couldn't load projectile '" + name + "'\n\n" + e);
            return null;
        }
    }

    // make this a compute shader??
    private Sprite renderAvatar(List<Armor> armor)
    {
        if (armor.Count == 0) 
        {
            return Sprite.Create(GameManager2D.Instance.defaultPawnTexture, new Rect(Vector2.zero, new Vector2(512, 512)), Vector2.zero, 512);
        }

        if (CachedItems.renderedPawns.Exists(x => x.armors == armor))
        {
            return CachedItems.renderedPawns.Find(x => x.armors == armor).sprite;
        }
        else
        {
            Texture2D final = new Texture2D(512, 512, TextureFormat.ARGB32, true); 
            final.SetPixels(GameManager2D.Instance.defaultPawnTexture.GetPixels());
            //List<Armor> sortedArmors = armor.OrderBy(x=>x.layer).ToList(); // sort the list low to high
            List<Armor> sortedArmors = armor.OrderBy(o=>o.layer).ToList();

            for(int i=0;i<armor.Count;i++)
            {
                Texture2D source = renderedArmors.Find(x=>x.name==armor[i].Name).sprite.texture;
                final = CombineTextures(source,final);
            }
            final.Apply();
            //try
            //{
                Sprite sprite = Sprite.Create(final, new Rect(Vector2.zero, new Vector2(512, 512)), Vector2.zero, 512);

                renderedPawns.Add(new RenderedPawn(sprite, armor));
                return sprite;
            //}
            //catch (Exception e)
            //{
            //    Debug.LogError("Couldn't render pawn. " + e);
            //    return null;
            //}
        }
    }

    // O(n^2)
    Texture2D CombineTextures(Texture2D _textureA, Texture2D _textureB)
    {
        //Create new textures
        Texture2D textureResult = new Texture2D(_textureA.width, _textureA.height, TextureFormat.ARGB32, true);
        //create clone form texture
        textureResult.SetPixels(_textureB.GetPixels());
        //Now copy texture B in texutre A
        int co = 0;
        for (int x = 0; x < _textureB.width; x++)
        {
            for (int y = 0; y < _textureB.height; y++)
            {
                Color c = _textureA.GetPixel(x, y);
                if (c.a > 0.0f) //Is not transparent
                {
                    co++;
                    //Copy pixel color in TexturaA
                    textureResult.SetPixel(x, y, c);
                }
                //else
                //{
                //    textureResult.SetPixel(x, y, new Color(255, 255, 255, 0));
                //}
            }
        }
        //Apply colors
        textureResult.Apply();
        return textureResult;
    }
    /*public void CombineTextures(Texture2D a, List<Armor> armors)
    {
        var bitmap = new System.Drawing.Bitmap(512, 512);
        using (var g = System.Drawing.Graphics.FromImage(bitmap))
        {
            for (int i = 0; i < armors.Count; i++)
            {
                g.DrawImage(Texture2Image(a), 0, 0);
                g.DrawImage(Texture2Image(RenderedSprites.renderedArmors.Find(x=>x.name==armors[i].Name).sprite.texture), 0, 0);
            }
        }
    }

    public static System.Drawing.Image Texture2Image(Texture2D texture) // wtf? put this in some other class. wtf. todo: wtf. todo: make it do all of these at startup
    {
        System.Drawing.Image img;
        using (MemoryStream MS = new MemoryStream())
        {
            texture.EncodeToPNG();
            //Go To the  beginning of the stream.
            MS.Seek(0, SeekOrigin.Begin);
            //Create the image based on the stream.
            img = System.Drawing.Bitmap.FromStream(MS);
        }
        return img;
    }*/
    private enum Loadtype
    {
        Weapon,
        Shield,
        Armor,
        Pawn
    }
}
