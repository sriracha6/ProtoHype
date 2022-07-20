using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Animals
{
    [ImageList(typeof(List<CachedItems.RenderedAnimalArmor>))]
    public class AnimalArmor : Item
    {
        [XMLItemLink("For Animal", typeof(Animal))] public Animal forAnimal;
        [XMLItem("Protection")] public int protection;
        [XMLItem("Move Speed Affect")] public float moveSpeedEffect;

        public AnimalArmor(string Name, string Description, string Sourcefile, int protection, Animal forAnimal, float moveSpeedEffect)
            : base(Name, Description, Sourcefile)
        {
            this.protection = protection;
            this.moveSpeedEffect = moveSpeedEffect;
            this.forAnimal = forAnimal;
        }

        public static List<AnimalArmor> List = new List<AnimalArmor>();

        public static AnimalArmor Create(string Name, string desc, string sourcefile, int protection, Animal forAnimal, float moveSpeedEffect)
        {
            //if(!CountryList.Exists(x => x.Name == name))
            //{
            AnimalArmor c = new AnimalArmor(Name, desc, sourcefile, protection, forAnimal, moveSpeedEffect);
            List.Add(c);
            return c;
            //}
            //else
            //{
            //    return null;
            //}
        }
        public static AnimalArmor Get(string name)
        {
            try
            {
                return List.Find(x => x.Name == name);
            }
            catch (NullReferenceException)
            {
                //Create(name);
                DB.Attention($"Couldn't find AnimalArmor of name {name}");
                return null;
                //return CountryList.Find(x => x.Name == name);
            }
        }
    }
}