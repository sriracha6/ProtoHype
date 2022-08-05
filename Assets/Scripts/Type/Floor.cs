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
        [XMLItem("Building Type")] public BuildingType buildingType { get; }
        public new FuckBitchTile tile;

        public Floor(string name, string description, string sourcefile, int hitpoints, int flammability) : base(name, description, sourcefile, false,false,RubbleType.None,hitpoints,flammability)
        {
            this.buildingType = BuildingType.Floor;
        }

        public static List<Floor> List = new List<Floor>();

        public static Floor Create(string sourcefile, string name, string description, int hitpoints, int flammability)
        {
            Floor c = new Floor(name, description, sourcefile, hitpoints, flammability);
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
