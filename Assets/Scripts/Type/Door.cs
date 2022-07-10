using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Buildings
{
    public class Door : Build
    {
        [XMLItem("Opening Speed")] public float OpeningSpeed;
        
        public Door(string name, RubbleType rubbletype, int hp, int flammability, float openingSpeed) 
            : base(name, "", false, true, rubbletype, hp, flammability)
        {
            this.OpeningSpeed = openingSpeed;
        }

        public static List<Door> List = new List<Door>();

        public static Door Create(string name, RubbleType rubbletype, int hp, int flammability, float openingSpeed)
        {
            if (!List.Exists(x => x.Name == name))
            {
                Door c = new Door(name, rubbletype, hp, flammability, openingSpeed);
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