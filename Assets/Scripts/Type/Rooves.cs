using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Buildings
{
    public struct RoofStats
    {
        public int SmallProjectileBlockChance;
        public int LargeProjectileBlockChance;
        
        public RoofStats(int Small, int Large)
        {
            SmallProjectileBlockChance = Small;
            LargeProjectileBlockChance = Large;
        }
    }
    public class Roof : Build
    {
        public BuildingType buildingType;
        public RoofStats roofStats;

        public Roof(string name, int hitpoints, int flammability, RoofStats roofStats) 
            : base(name,"",false,false,RubbleType.None,hitpoints,flammability)
        {
            this.buildingType = BuildingType.Roof;
            this.roofStats = roofStats;
        }

        public static List<Roof> List = new List<Roof>();

        public static Roof Create(string name, int hitpoints, int flammability, RoofStats roofStats)
        {
            if (!List.Any(x => x.Name == name))
            {
                Roof c = new Roof(name, hitpoints, flammability, roofStats);
                List.Add(c);
                return c;
            }
            else
            {
                //Debug.Log("Tried to create multiple of: "+name);
                return null;
            }
        }
        public static Roof Get(string name)
        {
            try
            {
                return List.Find(x => x.Name == name);
            }
            catch (NullReferenceException)
            {
                //Create(name);
                DB.Attention($"Couldn't find Roof of name {name}");
                return null;
                //return CountryList.Find(x => x.Name == name);
            }
        }
    }
}