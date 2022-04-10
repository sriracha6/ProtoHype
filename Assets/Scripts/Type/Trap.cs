using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Buildings
{
    public static class TrapManager
    {
        public static List<Trap> TrapList = new List<Trap>();

        public static Trap Create(string name, int hitpoints, int flammability, int damage, bool isSpecialPlace, bool hasRubble, RubbleType rubbleType, int coverQuality, bool isLean)
        {
            if (!TrapList.Any(x => x.Name == name))
            {
                Trap c = new Trap(name, hitpoints, flammability, damage, isSpecialPlace, hasRubble, rubbleType, coverQuality, isLean);
                TrapList.Add(c);
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
                return TrapList.Find(x => x.Name == name);
            }
            catch (NullReferenceException)
            {
                //Create(name);
                return null;
                //return CountryList.Find(x => x.Name == name);
            }
        }
    }
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
    }
}
