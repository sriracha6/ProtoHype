using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PawnFunctions;
using System;
using Attacks;

/// <summary>
/// I'm fairly proud of the other manager classes, but this is just bad. split this into melee+range files? or at least, two structs for melee+range
/// NOPE!!! YOu'RE NOT GETTING IT!! FUTURE ME SPEAKING AND I TRIED AND IT SUCKS SO MUCH ASS AND I JSUT WASTED AN HOUR ON IT. FUCK OOP!!!
/// </summary>
namespace Weapons
{
    public enum WeaponType
    {
        Melee,
        Ranged,
        Projectile
    }

    public struct MeleeRange
    {
        public const float Short = 0.9144f; // 3 feet
        public const float Medium = 1.524f; // 5 feet
        public const float Far = 2.4384f; // 8 feet
        public const float VeryFar = 5.1816f; // 17 feet

        public float value;

        public MeleeRange(float value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            if (value == Short) return "Short";
            if (value == Medium) return "Medium";
            if (value == Far) return "Far";
            if (value == VeryFar) return "Very Far";
            else
                return "Idk";
        }

        public MeleeRange getByName(string name)
        {
            string n = name.ToLower();
            // new string[] {"medium", "med", ...}.Contains(n)
            if (n == "short" || n == "s") this.value = Short;
            if (n == "medium" || n == "med" || n == "m") this.value = Medium;
            if (n == "far" || n == "long" || n == "l") this.value = Far;
            if (n == "vfar" || n == "veryfar") this.value = VeryFar;
            
            if (this.value == 0) DB.Attention($"Unknown melee range: \"{name}\"");

            return this;
        }
    }
    public enum RangeType
    {
        Shooter,
        Thrown
    }

    [ImageList(typeof(CachedItems.RenderedWeapon))]
    public class Weapon : Item
    {
        [XMLItem("Weapon Type")] [MeleeAttribute] [RangedAttribute] public WeaponType Type { get; }
        [XMLItem("Weapon Class")] [MeleeAttribute] [RangedAttribute] public string weaponClass { get; }
        [XMLItem("Melee Range")] [MeleeAttribute] public MeleeRange meleeRange { get; }
        [XMLItem("Has Warmup")] [MeleeAttribute] public bool hasWarmup { get; }
        [XMLItem("Armor Pen Sharp")] [MeleeAttribute] public int armorPenSharp { get; }
        [XMLItem("Armor Pen Blunt")] [MeleeAttribute] public int armorPenBlunt { get; }
        [XMLItem("Size")] [MeleeAttribute] [RangedAttribute] public float size { get; } // in comparison to a 6ft person
        [XMLItemList("Attacks")] [MeleeAttribute] public List<Attack> attacks { get; } = new List<Attack>();

        [XMLItem("Short Accuracy")] [RangedAttribute] public float shortAccuracy { get; }
        [XMLItem("Medium Accuracy")] [RangedAttribute] public float mediumAccuracy { get; }
        [XMLItem("Long Accuracy")] [RangedAttribute] public float longAccuracy { get; }

        [XMLItem("Has Ranged Melee Damage")] [RangedAttribute] public bool enableRangedMeleeDamage { get;}
        [XMLItem("Melee Damage Type")] [RangedAttribute] public string meleeDamageType { get;}
        [XMLItem("Ranged Melee Damage")] [RangedAttribute] public float rangedMeleeDamage { get; }

        [XMLItem("Ranged Damage")] [RangedAttribute] public int rangedDamage { get; }
        [XMLItem("Range Warmup Time")] [RangedAttribute] public float rangeWarmupTime { get; }
        [XMLItem("Range Armor Penetration")] [RangedAttribute] public float rangeArmorPen { get; }

        [XMLItem("Range")] [RangedAttribute] public int range { get; }
        [XMLItem("Range Type")] [RangedAttribute] public RangeType rangeType { get; }

        public Weapon(string sourcefile, string name, WeaponType type, string description, string wc, MeleeRange mrange, bool warmup, int armorpens,
                      int armorpenb, float Size, List<Attack> attks) : base(name, description, sourcefile)
        {
            Type = type;
            meleeRange = mrange;
            armorPenSharp = armorpens;
            armorPenBlunt = armorpenb;
            size = Size;
            attacks = attks;
        }

        public Weapon(string sourcefile, string name, string description, WeaponType type, string wc, int r, float armopen, RangeType rt, float meleeDmg,
            float warmupTime, string meleeDmgType, int dmg, float siz,
            float lAccuracy, float mAccuracy, float sAccuracy) : base(name, description, sourcefile)
        {
            Type = type;
            range = r;
            rangeType = rt;
            rangeWarmupTime = warmupTime;
            if (meleeDmg > 0)
                enableRangedMeleeDamage = true;
            else
                enableRangedMeleeDamage = false;
            rangeArmorPen = armopen;
            rangedMeleeDamage = meleeDmg;
            meleeDamageType = meleeDmgType;
            rangedDamage = dmg;
            size = siz;
            longAccuracy = lAccuracy;
            mediumAccuracy = mAccuracy;
            shortAccuracy = sAccuracy;
        }

        public static List<Weapon> List = new List<Weapon>();

        public static Weapon CreateMelee(string sourcefile, string name, WeaponType type, string weaponclass, string desc, MeleeRange mrange, bool warmup,
            int armorpens, int armorpenb, float Size, List<Attack> attks)
        {
            Weapon c = new Weapon(sourcefile, name, type, weaponclass, desc, mrange, warmup, armorpens, armorpenb, Size, attks);
            List.Add(c);
            return c;
        }

        public static Weapon CreateRanged(string sourcefile, string name, string desc, WeaponType type, string weaponclass, int range, float armorPen, RangeType rt, float meleeDamage, float warmupTime,
            string meleeDamageType, int dmg, float size,
            float longAccuracy, float mediumAccuracy, float shortAccuracy)
        {
            Weapon c = new Weapon(sourcefile, name, desc, type, weaponclass, range, armorPen, rt, meleeDamage, warmupTime, meleeDamageType, dmg, size, longAccuracy, mediumAccuracy, shortAccuracy);
            List.Add(c);
            return c;
        }

        public static Weapon Get(int id)
        {
            if(List.Exists(x=>x.ID==id))
            {
                return List.Find(x => x.ID == id);
            }
            else
                DB.Attention($"Couldn't find Weapon of id {id}");
                return null; //lol
        }

        public static Weapon Get(string name)
        {
            if (List.Exists(x => x.Name == name))
                return List.Find(x => x.Name == name);
            else
                DB.Attention($"Couldn't find Weapon of name {name}");
            return null; //lol
        }
    }
}