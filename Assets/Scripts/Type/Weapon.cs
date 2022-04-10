using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PawnFunctions;
using System;
using Attacks;

/// <summary>
/// I'm fairly proud of the other manager classes, but this is just bad. TODO: split this into melee+range files? or at least, two structs for melee+range
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
            return (float)typeof(MeleeRange).GetField(name).GetValue(typeof(float));
        }
    }
    public enum RangeType
    {
        Shooter,
        Thrown
    }
    public static class WeaponManager
    {
        public static List<Weapon> WeaponList = new List<Weapon>();

        public static Weapon CreateMelee(string name, WeaponType type, string weaponclass,string desc, float mrange, bool warmup, 
            int armorpens, int armorpenb, float Size, List<Attack> attks)
        {
            Weapon c = new Weapon(name, type, weaponclass, desc, mrange, warmup, armorpens, armorpenb, Size, attks);
            WeaponList.Add(c);
            return c;
        }

        public static Weapon CreateRanged(string name, string desc, WeaponType type, string weaponclass, int range, float armorPen, RangeType rt, float meleeDamage, float warmupTime,
            string meleeDamageType, int dmg, float size,
            float longAccuracy, float mediumAccuracy, float shortAccuracy)
        {
            Weapon c = new Weapon(name, desc, type,weaponclass, range, armorPen, rt, meleeDamage, warmupTime, meleeDamageType, dmg, size, longAccuracy, mediumAccuracy, shortAccuracy);
            WeaponList.Add(c);
            return c;
        }

        public static Weapon Get(int id)
        {
            try
            {
                return WeaponList.Find(x => x.ID == id);
            }
            catch (NullReferenceException)
            {
                //Create(name);
                return null; //lol
            }
        }

        public static Weapon Get(string name)
        {
            try
            {
                return WeaponList.Find(x => x.Name == name);
            }
            catch (NullReferenceException)
            {
                //Create(name);
                return null; //lol
            }
        }
    }
    public class Weapon : Item
    {
        public WeaponType Type; 
        public string weaponClass;
        public float meleeRange; 
        public bool hasWarmup;
        public int armorPenSharp;
        public int armorPenBlunt;
        public float size; // in comparison to a 6ft person
        public List<Attack> attacks = new List<Attack>();

        public float longAccuracy;
        public float shortAccuracy;
        public float mediumAccuracy;

        public bool enableRangedMeleeDamage;
        public string meleeDamageType;
        public float rangedMeleeDamage;

        public int rangedDamage;
        public float rangeWarmupTime;
        public float rangeArmorPen;

        public int range;
        public RangeType rangeType;

        public Weapon(string name, WeaponType type, string description, string wc, float mrange, bool warmup, int armorpens,
                      int armorpenb, float Size, List<Attack> attks) : base(name, description)
        {
            Type = type;
            meleeRange = mrange;
            armorPenSharp = armorpens;
            armorPenBlunt = armorpenb;
            size = Size;
            attacks = attks;
        }

        public Weapon(string name, string description,WeaponType type, string wc, int r, float armopen, RangeType rt, float meleeDmg,
            float warmupTime, string meleeDmgType, int dmg, float siz,
            float lAccuracy, float mAccuracy, float sAccuracy) : base(name, description)
        {
            Type = type;
            range = r;
            rangeType = rt;
            rangeWarmupTime = warmupTime;
            if(meleeDmg>0)
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
    }
}