using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Buildings
{
    public enum BuildingType
    {
        Prop,
        Trap,
        Roof,
        Building,
        Nature,
        Floor
    }
    public enum RubbleType
    {
        woodrubble,  // [x]
        stonerubble, // [x]
        miscrubble, //  [x]
        None=0
    }

    public class Furniture : Build
    {
        [XMLItem("Building Type")] public BuildingType buildingType;
        [XMLItem("Is Tileable")] public bool isTileable;
        [XMLItem("Cover Quality")] public int coverQuality;
        [XMLItem("Lean To Use")] public bool leanToUse;
        public bool PrefersTouchingWall;
        public List<Furniture> PrefersTouchingFurnitures;
        public bool isCarpet;

        public Furniture(string name, bool isTileable, int hitpoints, bool hasRubble, RubbleType rubbleType, int flammability, int coverQuality, bool leanToUse, bool PrefersTouchingWall, List<Furniture> PrefersTouchingFurniture, bool isCarpet)
            : base(name, "", false, hasRubble, rubbleType, hitpoints, flammability)
        {
            this.buildingType = BuildingType.Prop;
            this.isTileable = isTileable;
            this.hitpoints = hitpoints;
            this.hasRubble = hasRubble;
            this.rubbleType = rubbleType;
            this.flammability = flammability;
            this.coverQuality = coverQuality;
            this.leanToUse = leanToUse;
            this.PrefersTouchingFurnitures = PrefersTouchingFurniture;
            this.PrefersTouchingWall = PrefersTouchingWall;
            this.isCarpet = isCarpet;
        }

        public static List<Furniture> List = new List<Furniture>();

        public static Furniture Create(string name, bool isTileable, int hitpoints, bool hasRubble, RubbleType rubbleType, int flammability, int coverQuality, bool leanToUse, bool prefertouchwall, List<Furniture> preferTouchFurniture, bool isCarpet)
        {
            if (!List.Any(x => x.Name == name))
            {
                Furniture c = new Furniture(name, isTileable, hitpoints, hasRubble, rubbleType, flammability, coverQuality, leanToUse, prefertouchwall, preferTouchFurniture, isCarpet);
                List.Add(c);
                return c;
            }
            else
            {
                //Debug.Log("Tried to create multiple of: "+name);
                return null;
            }
        }
        public static Furniture Get(string name)
        {
            try
            {
                return List.Find(x => x.Name == name);
            }
            catch (NullReferenceException)
            {
                //Create(name);
                DB.Attention($"Couldn't find Furniture of name {name}");
                return null;
                //return CountryList.Find(x => x.Name == name);
            }
        }
    }
}