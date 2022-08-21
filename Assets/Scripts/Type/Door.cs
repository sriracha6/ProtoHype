using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Buildings
{
    public class Door : Build
    {
        [XMLItem("Opening Speed")] public float OpeningSpeed;
        public new Tile[,] tile;

        public Door(string name, string description, string sourcefile, RubbleType rubbletype, int hp, int flammability, float openingSpeed) 
            : base(name, description, sourcefile, false, true, rubbletype, hp, flammability)
        {
            this.OpeningSpeed = openingSpeed;
        }

        public static List<Door> List = new List<Door>();

        public static Door Create(string sourcefile, string name, string description, RubbleType rubbletype, int hp, int flammability, float openingSpeed)
        {
            if (!List.Exists(x => x.Name == name))
            {
                Door c = new Door(name, description, sourcefile, rubbletype, hp, flammability, openingSpeed);
                List.Add(c);
                return c;
            }
            else
            {
                //Debug.Log("Tried to create multiple of: "+name);
                return null;
            }
        }
        public static Door Get(string name)
        {
            if(List.Exists(x=>x.Name==name))
            {
                return List.Find(x => x.Name == name);
            }
            else
            {
                //Create(name);
                DB.Attention($"Couldn't find Door of name \"{name}\"");
                return null;
                //return CountryList.Find(x => x.Name == name);
            }
        }
    }
}