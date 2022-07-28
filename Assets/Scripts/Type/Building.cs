using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Buildings
{
    public class Building : Build // i probably shouldve made this derive from a single building class tbh
    {
        [XMLItem("Building Type")] public BuildingType buildingType { get; }
        [XMLItem("Cover Quality")] public int coverQuality;
        [XMLItem("Lean To Use")] public bool isLean;
        public new RuleTile tile;

        private bool isDamaged;
        public GameObject damageObject;

        public Building(string name, string description, string sourcefile, int hitpoints, int flammability, int coverQuality, bool isLean, bool isSpecialPlace, bool rubble, RubbleType rubbleType)
            : base(name,description,sourcefile,isSpecialPlace,rubble,rubbleType,hitpoints,flammability)
        {
            this.buildingType = BuildingType.Building;
            this.coverQuality = coverQuality;
            this.isLean = isLean;
        }

        public static List<Building> List = new List<Building>();

        public static Building Create(string sourcefile, string name, string description, int hitpoints, int flammability, int coverQuality, bool isLean, bool isSpecialPlace, bool rubble, RubbleType rubbleType)
        {
            if (!List.Any(x => x.Name == name))
            {
                Building c = new Building(name, description, sourcefile, hitpoints, flammability, coverQuality, isLean, isSpecialPlace, rubble, rubbleType);
                List.Add(c);
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
                return List.Find(x => x.ID == id);
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
            if (List.Exists(x => x.Name == name))
                return List.Find(x => x.Name == name);
            else
                DB.Attention($"Couldn't find Building of name \"{name}\"");
                return null;
        }

        public void UpdateDMG(Vector3 position)
        {
            if(!isDamaged)
            {
                var s = GameObject.Instantiate(WCMngr.I.damagePrefab);
                s.transform.position = position;
                damageObject = s;
                isDamaged = true;
            }
        }
    }
}