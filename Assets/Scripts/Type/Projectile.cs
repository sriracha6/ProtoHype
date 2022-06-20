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

        public static Projectile Create(string name, string desc, string sourcefile, string wclass, float dmg, DamageType dmgType, float accEffect, bool fire) // creates if it DOESNT exist
        {
            Projectile c = new Projectile(name, desc, sourcefile, wclass, dmg, dmgType, accEffect, fire);
            ProjectileList.Add(c);
            return c;
        }
        public static Projectile Get(string name)
        {
            if(ProjectileList.Exists(x => x.Name == name))
                return ProjectileList.Find(x => x.Name == name);
            else
                DB.Attention($"Couldn't find Projectile of name {name}");
                return null;
        }
        public static Projectile Get(int id)
        {
            try
            {
                return ProjectileList.Find(x => x.ID == id);
            }
            catch (NullReferenceException)
            {
                DB.Attention($"Couldn't find Projectile of id {id}");
                return null;
            }
        }
    }
    public class Projectile : Item
    {
        public string forWeaponClass;
        public float damage;
        public DamageType damageType;
        public float accuracyEffect;
        public bool hasFire;

        public Projectile(string name, string desc, string sourcefile, string wclass, float dmg, DamageType dmgType, float accEffect, bool fire) // same for this
            : base(name, desc, sourcefile)
        {
            forWeaponClass = wclass;
            damage = dmg;
            damageType = dmgType;
            accuracyEffect = accEffect;
            hasFire = fire;
        }
    }
}
