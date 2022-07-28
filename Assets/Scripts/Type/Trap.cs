using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Buildings
{
    public class Trap : Build
    {
        [XMLItem("Building Type")] public BuildingType buildingType;
        [XMLItem("Damage")] public int damage;
        [XMLItem("Cover Quality")] public int coverQuality;
        [XMLItem("Lean To Use")] public bool isLean;
        [XMLItem("Is One Use")] public bool oneUse;

        public Trap(string name, string description, string sourcefile, int hitpoints, int flammability, int damage, bool isSpecialPlace, bool hasRubble, RubbleType rubbleType, int coverQuality, bool isLean, bool isOneUse)
            :base(name, description, sourcefile, isSpecialPlace,hasRubble,rubbleType,hitpoints,flammability)
        {
            this.buildingType = BuildingType.Trap;
            this.damage = damage;
            this.coverQuality = coverQuality;
            this.isLean = isLean;
        }

        public static List<Trap> List = new List<Trap>();

        public static Trap Create(string sourcefile, string name, string description, int hitpoints, int flammability, int damage, bool isSpecialPlace, bool hasRubble, RubbleType rubbleType, int coverQuality, bool isLean, bool isOneUse)
        {
            if (!List.Any(x => x.Name == name))
            {
                Trap c = new Trap(name, description, sourcefile, hitpoints, flammability, damage, isSpecialPlace, hasRubble, rubbleType, coverQuality, isLean, isOneUse);
                List.Add(c);
                return c;
            }
            else
            {
                //Debug.Log("Tried to create multiple of: "+name);
                return null;
            }
        }
        public static Trap Get(string name)
        {
            try
            {
                return List.Find(x => x.Name == name);
            }
            catch (NullReferenceException)
            {
                //Create(name);
                DB.Attention($"Couldn't find Trap of name {name}");
                return null;
                //return CountryList.Find(x => x.Name == name);
            }
        }
    }
}
