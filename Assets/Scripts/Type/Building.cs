using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Buildings
{
    public static class BuildingManager
    {
        public static List<Building> BuildingList = new List<Building>();

        public static Building Create(string name, int hitpoints, int flammability, int coverQuality, bool isLean, bool isSpecialPlace, bool rubble, RubbleType rubbleType)
        {
            if (!BuildingList.Any(x => x.Name == name))
            {
                Building c = new Building(name, hitpoints, flammability, coverQuality, isLean, isSpecialPlace, rubble, rubbleType);
                BuildingList.Add(c);
                return c;
            }
            else
            {
                //Debug.Log("Tried to create multiple of: "+name);
                return null;
            }
        }
        public static Building Get(int id)
        {
            try
            {
                return BuildingList.Find(x => x.ID == id);
            }
            catch (NullReferenceException)
            {
                //Create(name);
                DB.Attention($"Couldn't find Building of id {id}");
                return null;
                //return CountryList.Find(x => x.Name == name);
            }
        }
        public static Building Get(string name)
        {
            try
            {
                return BuildingList.Find(x => x.Name == name);
            }
            catch (NullReferenceException)
            {
                //Create(name);
                DB.Attention($"Couldn't find Building of name {name}");
                return null;
                //return CountryList.Find(x => x.Name == name);
            }
        }
    }
    public class Building : Build // i probably shouldve made this derive from a single building class tbh
    {
        public BuildingType buildingType;
        public int coverQuality;
        public bool isLean;

        public Building(string name, int hitpoints, int flammability, int coverQuality, bool isLean, bool isSpecialPlace, bool rubble, RubbleType rubbleType)
            : base(name,"",isSpecialPlace,rubble,rubbleType,hitpoints,flammability)
        {
            this.buildingType = BuildingType.Building;
            this.coverQuality = coverQuality;
            this.isLean = isLean;
        }
    }
}