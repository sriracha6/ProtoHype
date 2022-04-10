using Attacks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weapons;

namespace Projectiles
{
    public static class ProjectileManager
    {
        public static List<Projectile> ProjectileList = new List<Projectile>();

        public static Projectile Create(string name, string desc, string wclass, float dmg, DamageType dmgType, float accEffect, bool fire) // creates if it DOESNT exist
        {
            Projectile c = new Projectile(name, desc, wclass, dmg, dmgType, accEffect, fire);
            ProjectileList.Add(c);
            return c;
        }
        public static Projectile Get(string name)
        {
            try
            {
                return ProjectileList.Find(x => x.Name == name);
            }
            catch (NullReferenceException)
            {
                Debug.Log("UFCK coudlnt get the projectile lol");
                return null;
            }
        }
    }
    public class Projectile
    {
        public string Name;
        public string description;
        public string forWeaponClass;
        public float damage;
        public DamageType damageType;
        public float accuracyEffect;
        public bool hasFire;

        public Projectile(string name, string desc, string wclass, float dmg, DamageType dmgType, float accEffect, bool fire) // same for this
        {
            Name = name;
            description = desc;
            forWeaponClass = wclass;
            damage = dmg;
            damageType = dmgType;
            accuracyEffect = accEffect;
            hasFire = fire;
        }
    }
}
