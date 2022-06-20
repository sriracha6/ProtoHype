using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static CachedItems;

namespace Buildings
{
    public static class NatureManager
    {
        public static List<Nature> NatureList = new List<Nature>();

        public static Nature Create(string name, int hitpoints, int flammability, int coverQuality, bool isLean, List<Sprite> tbase)
        {
            if (!NatureList.Any(x => x.Name == name))
            {
                Nature c = new Nature(name, hitpoints, flammability, coverQuality, isLean, tbase);
                NatureList.Add(c);
                return c;
            }
            else
            {
                //Debug.Log("Tried to create multiple of: "+name);
                return null;
            }
        }
        public static Nature Get(string name)
        {
            try
            {
                return NatureList.Find(x => x.Name == name);
            }
            catch (NullReferenceException)
            {
                //Create(name);
                DB.Attention($"Couldn't find Nature(building) of name {name}");
                return null;
                //return CountryList.Find(x => x.Name == name);
            }
        }
    }
    public class Nature : Build
    {
        public BuildingType buildingType;
        public int coverQuality;
        public bool isLean;
        public List<Sprite> sprites;

        public Nature(string name, int hitpoints, int flammability, int coverQuality, bool isLean, List<Sprite> sprites)
            : base(name,"",false,false,RubbleType.None,hitpoints,flammability)
        {
            this.buildingType = BuildingType.Nature;
            this.coverQuality = coverQuality;
            this.isLean = isLean;
            this.sprites = sprites;
        }
    }
}