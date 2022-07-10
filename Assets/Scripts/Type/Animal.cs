using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Animals
{
    [ImageList(typeof(CachedItems.RenderedAnimal))]
    public class Animal : Item
    {
        [XMLItem("Is Ridable")] public bool ridable;
        [XMLItem("Affect to Speed")] public float speedEffect { get; }
        [XMLItem("Hitpoints")] public int hitpoints { get; }
        [XMLItem("Hit Chance")] public int hitChance;
        public Hash128 spriteHash { get; set; }

        public Animal(string Name, string Description, string Sourcefile, bool ridable, float speedEffect, int hitpoints, int hitChance)
            : base(Name, Description, Sourcefile)
        {
            this.ridable = ridable;
            this.speedEffect = speedEffect;
            this.hitpoints = hitpoints;
            this.hitChance = hitChance;
        }

        public static List<Animal> List = new List<Animal>();

        public static Animal Create(string Name, string Description, string Sourcefile, bool ridable, float speedEffect, int hitpoints, int hitChance)
        {
            //if(!CountryList.Exists(x => x.Name == name))
            //{
            Animal c = new Animal(Name, Description, Sourcefile, ridable, speedEffect, hitpoints, hitChance);
            List.Add(c);
            return c;
            //}
            //else
            //{
            //    return null;
            //}
        }
        public static Animal Get(string name)
        {
            try
            {
                return List.Find(x => x.Name == name);
            }
            catch (NullReferenceException)
            {
                //Create(name);
                DB.Attention($"Couldn't find Animal of name {name}");
                return null;
                //return CountryList.Find(x => x.Name == name);
            }
        }
    }
}