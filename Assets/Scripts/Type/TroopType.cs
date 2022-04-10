using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weapons;
using Armors;
using Shields;
using Countries;

namespace TroopTypes
{
    public static class TroopTypeManager
    {
        public static List<TroopType> TroopTypeList = new List<TroopType>();

        public static void Create(string name, Country country, List<Weapon> weapons, List<Weapon> sidearms, List<Armor> armor, List<Shield> shields, int Mskillmin, int Mskillmax, int Rskillmin, int Rskillmax) // creates if it DOESNT exist
        {
            TroopType c = new TroopType(name, country, weapons, sidearms, armor, shields, Mskillmin, Mskillmax, Rskillmin, Rskillmax);
            TroopTypeList.Add(c);
        }
        public static TroopType Get(string name)
        {
            try
            {
                return TroopTypeList.Find(x => x.Name == name);
            }
            catch (NullReferenceException)
            {
                Debug.Log("UFCK coudlnt get the troop type lol");
                return null;
            }
        }
    }
    public class TroopType
    {
        public string Name;
        public Country country;
        public List<Weapon> weapons = new List<Weapon>(); // make this the weapon type because im not making another fucking type file
        public List<Weapon> sidearms = new List<Weapon>();
        public List<Armor> armor = new List<Armor>();
        public List<Shield> shields = new List<Shield>();

        public int meleeSkillMin;
        public int meleeSkillMax;

        public int rangeSkillMin;
        public int rangeSkillMax;

        public TroopType(string name, Country countr, List<Weapon> wea, List<Weapon> sidearmz, List<Armor> armo, List<Shield> shelds, int Mskillmin, int Mskillmax, int rskillmin, int rskillmax) // same for this
        {
            if (wea.Count > 0)
            {
                foreach (Weapon w in wea)
                {
                    weapons.Add(w); // shut up
                }
            }
            if (armo.Count > 0)
            {
                foreach (Armor a in armo)
                {
                    armor.Add(a);
                }
            }
            if (shelds.Count > 0)
            {
                foreach (Shield s in shelds)
                {
                    shields.Add(s);
                }
            }
            if (sidearmz.Count > 0)
            {
                foreach (Weapon w in sidearmz)
                {
                    sidearms.Add(w); // shut up
                }
            }

            Name = name;
            country = countr;
            meleeSkillMin = Mskillmin;
            meleeSkillMax = Mskillmax;
            rangeSkillMin = rskillmin;
            rangeSkillMax = rskillmax;
        }

        /*public string ToString(this TroopType i)
        {
            return i.Name;
        }*/
    }
}
