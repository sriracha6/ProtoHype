using Attacks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weapons;

namespace Projectiles
{
    [ImageList(typeof(List<CachedItems.RenderedProjectile>))]
    public class Projectile : Item
    {
        [XMLItem("For Weapon Class")] public string forWeaponClass;
        [XMLItem("Damage (multiplied)")] public float damage;
        [XMLItem("Damage Type")] public DamageType damageType;
        [XMLItem("Accuracy Effect")] public float accuracyEffect;
        [XMLItem("Has Fire")] public bool hasFire;

        public Projectile(string name, string desc, string sourcefile, string wclass, float dmg, DamageType dmgType, float accEffect, bool fire) // same for this
            : base(name, desc, sourcefile)
        {
            forWeaponClass = wclass;
            damage = dmg;
            damageType = dmgType;
            accuracyEffect = accEffect;
            hasFire = fire;
        }

        public static List<Projectile> List = new List<Projectile>();

        public static Projectile Create(string name, string desc, string sourcefile, string wclass, float dmg, DamageType dmgType, float accEffect, bool fire) // creates if it DOESNT exist
        {
            Projectile c = new Projectile(name, desc, sourcefile, wclass, dmg, dmgType, accEffect, fire);
            List.Add(c);
            return c;
        }
        public static Projectile Get(string name)
        {
            if (List.Exists(x => x.Name == name))
                return List.Find(x => x.Name == name);
            else
                DB.Attention($"Couldn't find Projectile of name {name}");
            return null;
        }
        public static Projectile Get(int id)
        {
            try
            {
                return List.Find(x => x.ID == id);
            }
            catch (NullReferenceException)
            {
                DB.Attention($"Couldn't find Projectile of id {id}");
                return null;
            }
        }
    }
}
