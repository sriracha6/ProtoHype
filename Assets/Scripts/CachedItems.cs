using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Armors;

public class CachedItems
{
    public struct RenderedPawn 
    {
        public Sprite sprite;
        public List<Armor> armors;

        public RenderedPawn(Sprite sprite, List<Armor> armors)
        {
            this.sprite = sprite;
            this.armors = armors;
        }
    }
    public struct RenderedShield
    {
        public Sprite sprite;
        public string name;

        public RenderedShield(Sprite sprite, string name)
        {
            this.sprite = sprite;
            this.name = name;
        }
    }
    public struct RenderedWeapon
    {
        public Sprite sprite;
        public string name;

        public RenderedWeapon(Sprite sprite, string name)
        {
            this.sprite = sprite;
            this.name = name;
        }
    }
    public struct RenderedArmor
    {
        public Sprite sprite;
        public string name;

        public RenderedArmor(Sprite sprite, string name)
        {
            this.sprite = sprite;
            this.name = name;
        }
    }
    public struct RenderedProjectile
    {
        public Sprite sprite;
        public string name;

        public RenderedProjectile(Sprite sprite, string name)
        {
            this.sprite = sprite;
            this.name = name;
        }
    }
    public struct RenderedFlora
    {
        public List<Sprite> image;
        public string name;

        public RenderedFlora(List<Sprite> image, string name)
        {
            this.image = image;
            this.name = name;
        }
    }


    public static List<RenderedWeapon> renderedWeapons = new List<RenderedWeapon>();
    public static List<RenderedShield> renderedShields = new List<RenderedShield>();
    public static List<RenderedArmor> renderedArmors = new List<RenderedArmor>();
    public static List<RenderedPawn> renderedPawns = new List<RenderedPawn>();
    public static List<RenderedProjectile> renderedProjectiles = new List<RenderedProjectile>();
    public static List<RenderedFlora> renderedFlora = new List<RenderedFlora>();


    public static List<Sprite> bloodSplatters = new List<Sprite>();
    public static List<string> firstNames = new List<string>();
    public static List<string> surnames = new List<string>();

    public static string RandomName
    {
        get
        {
            return firstNames[Random.Range(0, CachedItems.firstNames.Count)] + " " + surnames[Random.Range(0, CachedItems.surnames.Count)];
        }
    }
}
