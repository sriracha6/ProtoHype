using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PawnFunctions;
using System;
using Attacks;
using Body;

namespace Armors
{
    public enum Layer
    {
        skin,
        middle,
        outer
    }
    public static class ArmorManager
    {
        public static List<Armor> ArmorList = new List<Armor>();

        public static Armor Create(string filepath, string name, string desc, int healthpoints, float sharppot, float bluntpot, float movespeedeffect, Layer laye, bool isUtil, List<Bodypart> coverList) // creates if it DOESNT exist
        {
            Armor c = new Armor(filepath, name, desc, healthpoints, sharppot, bluntpot, movespeedeffect, laye, isUtil, coverList);
            ArmorList.Add(c);
            return c;
        }
        public static Armor Get(int id)
        {
            try
            {
                return ArmorList.Find(x => x.ID == id);
            }
            catch (NullReferenceException)
            {
                //Create(name);
                return null; //lol
            }
        }
        public static Armor Get(string name)
        {
            try
            {
                return ArmorList.Find(x => x.Name == name);
            }
            catch (NullReferenceException)
            {
                //Create(name);
                return null; //lol
            }
        }
    }
    public class Armor : Item
    {
        public float sharpProtection;
        public float bluntProtection;
        public float MovementSpeedAffect;
        public Layer layer;
        public int hitPoints;
        public List<Bodypart> covers = new List<Bodypart>();
        public bool isUtility;

        public Armor(string filepath, string name, string desc, int healthpoints, float sharppot, float bluntpot, float movespeedeffect, Layer laye, bool isUtil, List<Bodypart> coverList)
            : base(name, desc, filepath)
        {
            hitPoints = healthpoints;
            sharpProtection = sharppot;
            bluntProtection = bluntpot;
            MovementSpeedAffect = movespeedeffect;
            layer = laye;
            isUtility = isUtil;
            covers = coverList;
        }

        public float getProtection(DamageType type)
        {
            if (type == DamageType.Sharp)
                return sharpProtection;
            else if (type == DamageType.Blunt)
                return bluntProtection;
            else
                throw new NullReferenceException();
        }
    }
}