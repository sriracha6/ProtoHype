using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Buildings
{
    public class Floor : Build
    {
        public BuildingType buildingType { get; }
        public FuckBitchTile tile;

        public Floor(string name, int hitpoints, int flammability) : base(name, "", false,false,RubbleType.None,hitpoints,flammability)
        {
            this.buildingType = BuildingType.Floor;
        }

        public static List<Floor> List = new List<Floor>();

        public static Floor Create(string name, int hitpoints, int flammability)
        {
            Floor c = new Floor(name, hitpoints, flammability);
            List.Add(c);
            return c;
        }
        public static Floor Get(string name)
        {
            if(List.Exists(x => x.Name == name))
                return List.Find(x => x.Name == name);
            else
                DB.Attention($"Couldn't find Floor of name {name}");
                return null;
        }
        public static Floor Get(int id)
        {
            if(List.Exists(x=>x.ID==id))
                return List.Find(x => x.ID == id);
            else
                DB.Attention($"Couldn't find Floor of id {id}");
                return null;
        }
    }
}
