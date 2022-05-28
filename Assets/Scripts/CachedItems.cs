using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Armors;
// how about we delete these when we're done placing pawns to use 50% less RAM :: TODO
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
        public int id;

        public RenderedShield(Sprite sprite, int id)
        {
            this.sprite = sprite;
            this.id = id;
        }
    }
    public struct RenderedWeapon
    {
        public Sprite sprite;
        public int id;

        public RenderedWeapon(Sprite sprite, int id)
        {
            this.sprite = sprite;
            this.id = id;
        }
    }
    public struct RenderedArmor
    {
        public Sprite sprite;
        public int id;

        public RenderedArmor(Sprite sprite, int id)
        {
            this.sprite = sprite;
            this.id = id;
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
    public struct RenderedCountry
    {
        public Sprite image;
        public string name;

        public RenderedCountry(Sprite flag, string name)
        {
            this.image = flag;
            this.name = name;
        }
    }


    public static List<RenderedWeapon> renderedWeapons = new List<RenderedWeapon>();
    public static List<RenderedShield> renderedShields = new List<RenderedShield>();
    public static List<RenderedArmor> renderedArmors = new List<RenderedArmor>();
    public static List<RenderedPawn> renderedPawns = new List<RenderedPawn>();
    public static List<RenderedProjectile> renderedProjectiles = new List<RenderedProjectile>();
    public static List<RenderedFlora> renderedFlora = new List<RenderedFlora>();
    public static List<RenderedCountry> renderedCountries = new List<RenderedCountry>();

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
