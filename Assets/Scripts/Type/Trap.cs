using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Buildings
{
    public class Trap : Build
    {
        public BuildingType buildingType;
        public int damage;
        public int coverQuality;
        public bool isLean;

        public Trap(string name, int hitpoints, int flammability, int damage, bool isSpecialPlace, bool hasRubble, RubbleType rubbleType, int coverQuality, bool isLean)
            :base(name,"",isSpecialPlace,hasRubble,rubbleType,hitpoints,flammability)
        {
            this.buildingType = BuildingType.Trap;
            this.damage = damage;
            this.coverQuality = coverQuality;
            this.isLean = isLean;
        }

        public static List<Trap> List = new List<Trap>();

        public static Trap Create(string name, int hitpoints, int flammability, int damage, bool isSpecialPlace, bool hasRubble, RubbleType rubbleType, int coverQuality, bool isLean)
        {
            if (!List.Any(x => x.Name == name))
            {
                Trap c = new Trap(name, hitpoints, flammability, damage, isSpecialPlace, hasRubble, rubbleType, coverQuality, isLean);
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
