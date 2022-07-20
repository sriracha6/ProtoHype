using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weapons;
using Armors;
using Animals;
using Shields;
using Countries;

namespace TroopTypes
{
    public enum PreferSpawn { InsideBase, AroundBase, OutsideBase=0}

    public struct ArmorInfo
    {
        public List<List<Armor>> pickFrom;
        public List<Armor> required;

        public ArmorInfo (List<List<Armor>> armor, List<Armor> required)
        {
            this.pickFrom = armor;
            this.required = required;
        }
    }
    [ImageList(typeof(List<CachedItems.RenderedTroopType>))]
    public class TroopType : Item
    {
        [XMLLinkList("Weapons")] public List<Weapon> weapons = new List<Weapon>();// make this the weapon type because im not making another fucking type file
        [XMLLinkList("Sidearms")] public List<Weapon> sidearms = new List<Weapon>();
        [XMLRequiredPickFrom("Armor")] public List<List<Armor>> armor  = new List<List<Armor>>();
        [XMLLinkList("Shields")] public List<Shield> shields = new List<Shield>();

        [XMLItem("Melee Skill Max")] public int meleeSkillMax;
        [XMLItem("Melee Skill Min")] public int meleeSkillMin;

        [XMLItem("Range Skill Max")] public int rangeSkillMax;
        [XMLItem("Range Skill Min")] public int rangeSkillMin;
        public string Icon;

        [XMLItem("Is Riding Animal")] public bool ridingAnimal;
        [XMLItemLink("Ridden Animal", typeof(Animal))] public Animal riddenAnimal;
        [XMLLinkList("Animal Armor")] public List<AnimalArmor> animalArmor; // todo: chances of having and required and pick from groups and sex and FUCk
        [XMLItem("Prefer Spawn")] public PreferSpawn preferSpawn;
        [XMLItem("Country")] public Country country;

        public override string ToString()
        {
            return Name + " | " + country.Name;
        }

        public TroopType(string name, string description, string sourcefile, Country countr, List<Weapon> wea, List<Weapon> sidearmz, List<List<Armor>> armo, List<Shield> shelds, int Mskillmin, int Mskillmax, int rskillmin, int rskillmax, 
            bool ridingAnimal, Animal riddenAnimal, List<AnimalArmor> animalArmor, string Icon, PreferSpawn preferSpawn) // same for this
            : base(name, description, sourcefile)
        {
            /*if (wea != null)
                foreach(Weapon w in wea)
                    if(w != null)
                        weapons.Add(w);
                    else
                        DB.Attention("Null Primary Weapon attempted to be added to trooptype");
            */
            weapons.AddRange(wea);
            /*if (armo != null)
                foreach (List<Armor> a in armo)
                    foreach(Armor a2 in a)
                    if (a2 != null)
                        armor.Add(a2);
                    else
                        DB.Attention("Null Armor");*/
            armor = armo;
            /*if (shelds != null)
                foreach(Shield s in shelds)
                    if(s != null)
                        shields.Add(s);
                    else
                        DB.Attention("Null Shield");
            */
            if(shelds != null)
            shields.AddRange(shelds);
            /*
            if (sidearmz != null)
                foreach(Weapon w in sidearmz)
                    if(w!=null)
                        sidearms.Add(w); // shut up
                    else
                        DB.Attention("Null Sidearm");
            */
            if(sidearmz != null)
            sidearms.AddRange(sidearmz);

            if (riddenAnimal != null)
                this.riddenAnimal = riddenAnimal;
            else
                DB.Attention("Null animal");
                
            this.preferSpawn = preferSpawn;
            country = countr;
            meleeSkillMin = Mskillmin;
            meleeSkillMax = Mskillmax;
            rangeSkillMin = rskillmin;
            rangeSkillMax = rskillmax;
            this.ridingAnimal = ridingAnimal;
            this.animalArmor = animalArmor;
            this.Icon = Icon;
        }

        public static List<TroopType> List = new List<TroopType>();

        public static TroopType Create(string name, string description, string sourcefile, Country country, List<Weapon> weapons, List<Weapon> sidearms, List<List<Armor>> armor, List<Shield> shields, int Mskillmin, int Mskillmax, int Rskillmin, int Rskillmax, bool ridingAnimal, Animal animal, List<AnimalArmor> animalarmor, string Icon, PreferSpawn preferSpawn) // creates if it DOESNT exist
        {
            TroopType c = new TroopType(name, description, sourcefile, country, weapons, sidearms, armor, shields, Mskillmin, Mskillmax, Rskillmin, Rskillmax, ridingAnimal, animal, animalarmor, Icon, preferSpawn);
            List.Add(c);
            return c;
        }
        public static TroopType Get(string name)
        {
            if(List.Exists(x=>x.Name == name))
                return List.Find(x => x.Name == name);
            else
                DB.Attention($"Couldn't find TroopType of name {name}");
                return null;
        }
        public static TroopType Get(string name, Country country)
        {
            if (List.Exists(x => x.Name == name && x.country == country))
                return List.Find(x => x.Name == name && x.country == country);
            else
                DB.Attention($"Couldn't find TroopType of name {name} and country {country.Name}");
                return null;
        }

        public static TroopType Get(int id)
        {
            if(List.Exists(x=>x.ID == id))
                return List.Find(x => x.ID == id);
            else
                DB.Attention($"Couldn't find TroopType of id {id}");
                return null;
        }

        /*public string ToString(this TroopType i)
        {
            return i.Name;
        }*/
    }
}
