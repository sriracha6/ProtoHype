using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Buildings
{
    public class Floor : Build
    {
        public BuildingType buildingType { get; }

        public Floor(string name, int hitpoints, int flammability) : base(name, "", false,false,RubbleType.None,hitpoints,flammability)
        {
            this.buildingType = BuildingType.Floor;
        }

        public static List<Floor> List = new List<Floor>();

        public static Floor Create(string name, int hitpoints, int flammability)
        {
            if (!List.Any(x => x.Name == name))
            {
                Floor c = new Floor(name, hitpoints, flammability);
                List.Add(c);
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
                return List.Find(x => x.Name == name);
            }
            catch (NullReferenceException)
            {
                //Create(name);
                DB.Attention($"Couldn't find Floor of name {name}");
                return null;
                //return CountryList.Find(x => x.Name == name);
            }
        }
        public static Floor Get(int id)
        {
            try
            {
                return List.Find(x => x.ID == id);
            }
            catch (NullReferenceException)
            {
                //Create(name);
                DB.Attention($"Couldn't find Floor of id {id}");
                return null;
                //return CountryList.Find(x => x.Name == name);
            }
        }
    }
}
