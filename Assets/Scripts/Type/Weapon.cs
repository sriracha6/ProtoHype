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

        public static float getByName(string name)
        {
            string n = name.ToLower();
            // new string[] {"medium", "med", ...}.Contains(n)
            if (n == "short" || n == "s")
                return Short;
            if (n == "medium" || n == "med" || n == "m")
                return Medium;
            if (n == "far" || n == "long" || n == "l")
                return Far;
            if (n == "vfar" || n == "veryfar")
                return VeryFar;
            Debug.Log($"Unknown melee range: {name}");
            return Medium;
        }
    }
    public enum RangeType
    {
        Shooter,
        Thrown
    }

    public class Weapon : Item
    {
        public WeaponType Type { get; }
        public string weaponClass { get; }
        public float meleeRange { get; }
        public bool hasWarmup { get; }
        public int armorPenSharp { get; }
        public int armorPenBlunt { get; }
        public float size { get; } // in comparison to a 6ft person
        public List<Attack> attacks { get; } = new List<Attack>();

        public float longAccuracy { get; }
        public float shortAccuracy { get; }
        public float mediumAccuracy { get; }

        public bool enableRangedMeleeDamage { get;}
        public string meleeDamageType { get;}
        public float rangedMeleeDamage { get; }

        public int rangedDamage { get; }
        public float rangeWarmupTime { get; }
        public float rangeArmorPen { get; }

        public int range { get; }
        public RangeType rangeType { get; }

        public Weapon(string sourcefile, string name, WeaponType type, string description, string wc, float mrange, bool warmup, int armorpens,
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

        public static Weapon CreateMelee(string sourcefile, string name, WeaponType type, string weaponclass, string desc, float mrange, bool warmup,
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