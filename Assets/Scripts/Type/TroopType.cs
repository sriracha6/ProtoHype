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
    public class TroopType : Item
    {
        public Country country;
        public List<Weapon> weapons = new List<Weapon>(); // make this the weapon type because im not making another fucking type file
        public List<Weapon> sidearms = new List<Weapon>();
        public List<List<Armor>> armor = new List<List<Armor>>();
        public List<Shield> shields = new List<Shield>();

        public int meleeSkillMin;
        public int meleeSkillMax;

        public int rangeSkillMin;
        public int rangeSkillMax;
        public string Icon;

        public bool ridingAnimal;
        public Animal riddenAnimal;
        public List<AnimalArmor> animalArmor = new List<AnimalArmor>(); // todo: chances of having and required and pick from groups and sex and FUCk

        public override string ToString()
        {
            return Name + " | " + country.Name;
        }

        public TroopType(string name, string description, string sourcefile, Country countr, List<Weapon> wea, List<Weapon> sidearmz, List<List<Armor>> armo, List<Shield> shelds, int Mskillmin, int Mskillmax, int rskillmin, int rskillmax, 
            bool ridingAnimal, Animal riddenAnimal, List<AnimalArmor> animalArmor, string Icon) // same for this
            : base(name, description, sourcefile)
        {
            if (wea != null)
                foreach(Weapon w in wea)
                    if(w != null)
                        weapons.Add(w);
                    else
                        DB.Attention("Null Prmiary Weapon");

            /*if (armo != null)
                foreach (List<Armor> a in armo)
                    foreach(Armor a2 in a)
                    if (a2 != null)
                        armor.Add(a2);
                    else
                        DB.Attention("Null Armor");*/
            armor = armo;

            if (shelds != null)
                foreach(Shield s in shelds)
                    if(s != null)
                        shields.Add(s);
                    else
                        DB.Attention("Null Shield");

            if (sidearmz != null)
                foreach(Weapon w in sidearmz)
                    if(w!=null)
                        sidearms.Add(w); // shut up
                    else
                        DB.Attention("Null Sidearm");

            if (riddenAnimal != null)
                this.riddenAnimal = riddenAnimal;
            else
                DB.Attention("Null animal");
                
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

        public static TroopType Create(string name, string description, string sourcefile, Country country, List<Weapon> weapons, List<Weapon> sidearms, List<List<Armor>> armor, List<Shield> shields, int Mskillmin, int Mskillmax, int Rskillmin, int Rskillmax, bool ridingAnimal, Animal animal, List<AnimalArmor> animalarmor, string Icon) // creates if it DOESNT exist
        {
            TroopType c = new TroopType(name, description, sourcefile, country, weapons, sidearms, armor, shields, Mskillmin, Mskillmax, Rskillmin, Rskillmax, ridingAnimal, animal, animalarmor, Icon);
            List.Add(c);
            return c;
        }
        public static TroopType Get(string name)
        {
            try
            {
                return List.Find(x => x.Name == name);
            }
            catch (NullReferenceException)
            {
                DB.Attention($"Couldn't find TroopType of name {name}");
                return null;
            }
        }
        public static TroopType Get(int id)
        {
            try
            {
                return List.Find(x => x.ID == id);
            }
            catch (NullReferenceException)
            {
                DB.Attention($"Couldn't find TroopType of id {id}");
                return null;
            }
        }

        /*public string ToString(this TroopType i)
        {
            return i.Name;
        }*/
    }
}
