using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static CachedItems;

namespace Buildings
{
    public class Plant : Build
    {
        [XMLItem("Building Type")] public BuildingType buildingType;
        [XMLItem("Cover Quality")] public int coverQuality;
        [XMLItem("Lean to Use")] public bool isLean;
        public List<Sprite> sprites;

        public Plant(string name, int hitpoints, int flammability, int coverQuality, bool isLean, List<Sprite> sprites)
            : base(name,"",false,false,RubbleType.None,hitpoints,flammability)
        {
            this.buildingType = BuildingType.Nature;
            this.coverQuality = coverQuality;
            this.isLean = isLean;
            this.sprites = sprites;
        }

        public static List<Plant> List = new List<Plant>();

        public static Plant Create(string name, int hitpoints, int flammability, int coverQuality, bool isLean, List<Sprite> tbase)
        {
            if (!List.Any(x => x.Name == name))
            {
                Plant c = new Plant(name, hitpoints, flammability, coverQuality, isLean, tbase);
                List.Add(c);
                return c;
            }
            else
            {
                //Debug.Log("Tried to create multiple of: "+name);
                return null;
            }
        }
        public static Plant Get(string name)
        {
            try
            {
                return List.Find(x => x.Name == name);
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
}