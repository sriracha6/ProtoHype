using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using XMLLoader;
using Armors;
using Animals;

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
        public Shields.Shield id;

        public RenderedShield(Sprite sprite, Shields.Shield id)
        {
            this.sprite = sprite;
            this.id = id;
        }
    }
    public struct RenderedWeapon
    {
        public Sprite sprite;
        public Weapons.Weapon id;

        public RenderedWeapon(Sprite sprite, Weapons.Weapon id)
        {
            this.sprite = sprite;
            this.id = id;
        }
    }
    public struct RenderedArmor
    {
        public Sprite sprite;
        public Armors.Armor id;

        public RenderedArmor(Sprite sprite, Armors.Armor id)
        {
            this.sprite = sprite;
            this.id = id;
        }
    }
    public struct RenderedProjectile
    {
        public Sprite sprite;
        public Projectiles.Projectile id;

        public RenderedProjectile(Sprite sprite, Projectiles.Projectile id)
        {
            this.sprite = sprite;
            this.id = id;
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
        public Countries.Country id;

        public RenderedCountry(Sprite flag, Countries.Country id)
        {
            this.image = flag;
            this.id = id;
        }
    }
    public struct RenderedAnimal
    {
        public List<AnimalArmor> animalArmor;
        public Sprite animalSprite; // taken from all the ones in the file
        public Animal id;
        public Sprite finalSprite;

        public RenderedAnimal(List<AnimalArmor> animalArmor, Sprite animalSprite, Animal id, Sprite finalSprite)
        {
            this.animalArmor = animalArmor;
            this.animalSprite = animalSprite;
            this.id = id;
            this.finalSprite = finalSprite;
        }
    }   
    public struct RenderedAnimalArmor
    {
        public AnimalArmor animalArmor;
        public Sprite spr;

        public RenderedAnimalArmor(AnimalArmor animalArmor, Sprite spr)
        {
            this.animalArmor = animalArmor;
            this.spr = spr;
        }
    }
    public struct RenderedAnimalPick
    {
        public List<Sprite> picks;
        public Animal animal;

        public RenderedAnimalPick(List<Sprite> picks, Animal a)
        {
            this.picks = picks;
            this.animal = a;
        }
    }
    public struct RenderedTroopType
    {
        public Texture2D texture;
        public string name;
        
        public RenderedTroopType(Texture2D texture, string name)
        {
            this.texture = texture;
            this.name = name;
        }
    }
    public struct RenderedWall
    {
        public int ID;
        public Sprite[] sprites;

        public RenderedWall(int iD, Sprite[] sprites)
        {
            this.ID = iD;
            this.sprites = sprites;
        }
    }

    public static List<RenderedWeapon> renderedWeapons = new List<RenderedWeapon>();
    public static List<RenderedShield> renderedShields = new List<RenderedShield>();
    public static List<RenderedArmor> renderedArmors = new List<RenderedArmor>();
    public static List<RenderedPawn> renderedPawns = new List<RenderedPawn>();
    public static List<RenderedProjectile> renderedProjectiles = new List<RenderedProjectile>();
    public static List<RenderedFlora> renderedFlora = new List<RenderedFlora>();
    public static List<RenderedCountry> renderedCountries = new List<RenderedCountry>();
    public static List<RenderedAnimal> renderedAnimals = new List<RenderedAnimal>();
    public static List<RenderedAnimalArmor> renderedAnimalArmors = new List<RenderedAnimalArmor>();
    public static List<RenderedAnimalPick> renderedAnimalPicks = new List<RenderedAnimalPick>();
    public static List<RenderedTroopType> renderedTroopTypes = new List<RenderedTroopType>();
    public static List<RenderedWall> renderedWalls = new List<RenderedWall>();

    public static List<Sprite> bloodSplatters = new List<Sprite>();
    public static List<string> firstNames = new List<string>();
    public static List<string> surnames = new List<string>();

    public static string RandomName
    {
        get
        {
            return firstNames.randomElement().stripNewlines() + " " + surnames.randomElement().stripNewlines();
        }
    }
}
