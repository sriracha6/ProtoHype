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
        woodrubble,
        stonerubble,
        miscrubble,
        None
    }

    public class Furniture
    {
        public string Name;
        public BuildingType buildingType;
        public bool isTileable;
        public int hitpoints; // dont add max hitpoints and hitpoints. no. thats not how it works
        public bool hasRubble;
        public RubbleType rubbleType;
        public float flammability; // percent
        public int coverQuality;
        public bool leanToUse;

        public Furniture(string name, bool isTileable, int hitpoints, bool hasRubble, RubbleType rubbleType, int flammability, int coverQuality, bool leanToUse)
        {
            this.Name = name;
            this.buildingType = BuildingType.Prop;
            this.isTileable = isTileable;
            this.hitpoints = hitpoints;
            this.hasRubble = hasRubble;
            this.rubbleType = rubbleType;
            this.flammability = flammability;
            this.coverQuality = coverQuality;
            this.leanToUse = leanToUse;
        }

        public static List<Furniture> List = new List<Furniture>();

        public static Furniture Create(string name, bool isTileable, int hitpoints, bool hasRubble, RubbleType rubbleType, int flammability, int coverQuality, bool leanToUse)
        {
            if (!List.Any(x => x.Name == name))
            {
                Furniture c = new Furniture(name, isTileable, hitpoints, hasRubble, rubbleType, flammability, coverQuality, leanToUse);
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