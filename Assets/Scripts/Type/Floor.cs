using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Buildings
{
    public static class FloorManager
    {
        public static List<Floor> FloorList = new List<Floor>();

        public static Floor Create(string name, int hitpoints, int flammability)
        {
            if (!FloorList.Any(x => x.Name == name))
            {
                Floor c = new Floor(name, hitpoints, flammability);
                FloorList.Add(c);
                return c;
            }
            else
            {
                //Debug.Log("Tried to create multiple of: "+name);
                return null;
            }
        }
        public static Floor Get(string name)
        {
            try
            {
                return FloorList.Find(x => x.Name == name);
            }
            catch (NullReferenceException)
            {
                //Create(name);
                return null;
                //return CountryList.Find(x => x.Name == name);
            }
        }
        public static Floor Get(int id)
        {
            try
            {
                return FloorList.Find(x => x.ID == id);
            }
            catch (NullReferenceException)
            {
                //Create(name);
                return null;
                //return CountryList.Find(x => x.Name == name);
            }
        }
    }
    public class Floor : Build
    {
        public BuildingType buildingType;

        public Floor(string name, int hitpoints, int flammability) : base(name, "", false,false,RubbleType.None,hitpoints,flammability)
        {
            this.buildingType = BuildingType.Floor;
        }
    }
}
