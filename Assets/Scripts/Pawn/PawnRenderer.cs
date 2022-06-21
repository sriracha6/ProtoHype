using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PawnFunctions;

using System.IO;
using System.Linq;
using System;
using Armors;
using Body;
using XMLLoader;
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

    public const float SIXFEETMETERS = 1.8288f;

    void Start()
    {
        if(p.hasPrimary && !p.isFlagBearer)
            weapon.sprite =
                imageFromWeaponID(p.heldPrimary.ID, p.heldPrimary.size);
        if (p.hasSidearm)
            imageFromWeaponID(p.heldSidearm.ID, p.heldSidearm.size);

        if (p.hasShield)
        {
            shield.sprite =
                imageFromShieldID(p.shield.ID, p.shield.size);
        }

        foreach(Armor a in p.armor)
            imageFromArmorName(a.ID);

        p.sprite.sprite = renderAvatar(p.armor);
        TEX = p.sprite.sprite.texture;
    } 

    /*private Sprite createSprite(Texture2D image, string backup, float size, Loadtype ltype)
    {
        try
        {
            bool exists = false;

            if (ltype == Loadtype.Weapon)
                exists = !CachedItems.renderedWeapons.Exists(x => Weapons.WeaponManager.Get(x.id).Name == backup);
            else if (ltype == Loadtype.Shield)
                exists = !CachedItems.renderedShields.Exists(x => Shields.ShieldManager.Get(x.id).Name == backup);
            else if(ltype == Loadtype.Armor)
                exists = !CachedItems.renderedArmors.Exists(x => x.id == Armors.ArmorManager.Get(backup).ID);
            if (exists)
            {
                Sprite spr = Sprite.Create(image, new Rect(Vector2.zero // weapon??
                    , new Vector2(image.width, image.height)), Vector2.zero, WeaponSize / size);

                if (ltype == Loadtype.Weapon)
                    CachedItems.renderedWeapons.Add(new RenderedWeapon(spr, Weapons.WeaponManager.Get(backup).ID));
                else if (ltype == Loadtype.Shield)
                    CachedItems.renderedShields.Add(new RenderedShield(spr, Shields.ShieldManager.Get(backup).ID));
                else
                    CachedItems.renderedArmors.Add(new RenderedArmor(spr, Armors.ArmorManager.Get(backup).ID));
                return spr;
            }
            else
            {
                if (ltype == Loadtype.Weapon)
                    return CachedItems.renderedWeapons.Find(x => Weapons.WeaponManager.Get(x.id).Name == backup).sprite;
                else if (ltype == Loadtype.Shield)
                    return CachedItems.renderedShields.Find(x=>Shields.ShieldManager.Get(x.id).Name==backup).sprite;
                else 
                    return CachedItems.renderedArmors.Find(x => x.id == Armors.ArmorManager.Get(backup).ID).sprite;
            }
        }
        catch(Exception e)
        {
            Debug.LogError($"error creating sprite {backup} \n {ltype}\n {size} \n w:{image.width} h{image.height} \n {e}");
            return null;
        }
    }*/

    private Sprite imageFromWeaponID(int id, float size)
    {
        if (id == WCMngr.I.flagWeapon.ID)
            return null;
        try
        {
            if (!CachedItems.renderedWeapons.Exists(x => x.id == id))
            {
                Texture2D tex = XMLLoader.Loaders.LoadTex(Weapons.Weapon.Get(id).SourceFile);
                Sprite spr = Loaders.LoadSprite(tex, tex.height * size * SIXFEETMETERS);
                CachedItems.renderedWeapons.Add(new RenderedWeapon(spr, id));
                return spr;
            }
            else
            {
                return CachedItems.renderedWeapons.Find(x=>x.id==id).sprite;
            }
        }catch(System.IO.IOException e)
        {
            Debug.LogError("Couldn't load sidearm of id "+id+"\n"+e);
        }
        return null;
    }
    private Sprite imageFromShieldID(int id, float size)
    {
        try
        {
            if (!CachedItems.renderedShields.Exists(x => x.id == id))
            {
                Texture2D tex = Loaders.LoadTex(Shields.Shield.Get(id).SourceFile);
                Sprite spr = Loaders.LoadSprite(tex, tex.height * size * SIXFEETMETERS);
                CachedItems.renderedShields.Add(new RenderedShield(spr, id));
                return spr;
            }
            else
            {
                return CachedItems.renderedShields.Find(x=>x.id==id).sprite;
            }
        }
        catch (System.IO.IOException e)
        {
            Debug.LogError("Couldn't load shield of name " + name + "\n\n" + e);
        }
        return null;
    }
    private Sprite imageFromArmorName(int id)
    {
        try
        {
            if (!CachedItems.renderedArmors.Exists(x => x.id == id))
            {
                Texture2D tex = Loaders.LoadTex(Armors.Armor.Get(id).SourceFile);
                Sprite spr = Loaders.LoadSprite(tex, 1);
                CachedItems.renderedArmors.Add(new RenderedArmor(spr, id));
                return spr;
            }
            else
            {
                return CachedItems.renderedArmors.Find(x=>x.id==id).sprite;
            }
        }
        catch (System.IO.IOException e)
        {
            Debug.LogError("Couldn't load armor of name " + name + "\n\n" + e);
        }
        return null;
    }

    public static Sprite getProjectile(int id)
    {
        try
        {
            if (CachedItems.renderedProjectiles.Exists(x => x.id == id))
                return CachedItems.renderedProjectiles.Find(x=>x.id==id).sprite;
            else
            {
                Debug.Log($"{Projectiles.Projectile.Get(id).Name}");
                Texture2D tex = Loaders.LoadTex(Projectiles.Projectile.Get(id).SourceFile);
                Sprite spr = Loaders.LoadSprite(tex, 1);
                CachedItems.renderedProjectiles.Add(new RenderedProjectile(spr, id));
                return spr;
            }
        }
        catch (System.IO.IOException e)
        {
            Debug.LogError("Couldn't load projectile '" + id + "'\n\n" + e);
            return null;
        }
    }

    // make this a compute shader??
    private Sprite renderAvatar(List<Armor> armor)
    {
        if (armor.Count == 0) 
            return Sprite.Create(WCMngr.I.defaultPawnTexture, new Rect(Vector2.zero, new Vector2(512, 512)), Vector2.zero, 512);

        if (CachedItems.renderedPawns.Exists(x => x.armors == armor))
            return CachedItems.renderedPawns.Find(x => x.armors == armor).sprite;

        else
        {
            Texture2D final = new Texture2D(512, 512, TextureFormat.ARGB32, true);
            final.SetPixels(WCMngr.I.defaultPawnTexture.GetPixels());
            //List<Armor> sortedArmors = armor.OrderBy(x=>x.layer).ToList(); // sort the list low to high
            List<Armor> sortedArmors = armor.OrderBy(o=>o.layer).ToList();

            for(int i=0;i<armor.Count;i++)
            {
                Texture2D source = renderedArmors.Find(x=>x.id==armor[i].ID).sprite.texture;
                final = CombineTextures(source,final, p.country);
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

    // this is O(1) because always 512x512
    public static Texture2D CombineTextures(Texture2D _textureA, Texture2D _textureB, Countries.Country country)
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
                    if(c.r == 255 && c.g == 0 && c.b == 255)
                    { // wrap this around thats how we make it repeat sexpenis
                        Color ccolor;
                        var cimage = renderedCountries.Find(x => x.name == country.Name).image;
                        ccolor = cimage.texture.GetPixel(x % cimage.texture.width, y % cimage.texture.height);
                        textureResult.SetPixel(x, y, ccolor);
                    }
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

    public static Texture2D CombineTextures(Texture2D _textureA, Texture2D _textureB)
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

    private enum Loadtype
    {
        Weapon,
        Shield,
        Armor,
        Pawn
    }
}
