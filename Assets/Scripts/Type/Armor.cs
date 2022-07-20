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
        skin = 0,
        middle = 1,
        outer = 2
    }

    [ImageList(typeof(List<CachedItems.RenderedArmor>))]
    public class Armor : Item
    {
        [XMLItem("Sharp Protection")] public float sharpProtection;
        [XMLItem("Blunt Protection")] public float bluntProtection;
        [XMLItem("Move Speed Effect")] public float MovementSpeedAffect;
        [XMLItem("Layer")] public Layer layer;
        [XMLItem("Hitpoints")] public int hitPoints;
        [XMLItemList("Covers")] public List<Bodypart> covers = new List<Bodypart>();
        [XMLItem("Is Utility")] public bool isUtility;

        public override string ToString() { return Name; }

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

        public static List<Armor> List = new List<Armor>();

        public static Armor Create(string filepath, string name, string desc, int healthpoints, float sharppot, float bluntpot, float movespeedeffect, Layer laye, bool isUtil, List<Bodypart> coverList) // creates if it DOESNT exist
        {
            Armor c = new Armor(filepath, name, desc, healthpoints, sharppot, bluntpot, movespeedeffect, laye, isUtil, coverList);
            List.Add(c);
            return c;
        }
        public static Armor Get(int id)
        {
            try
            {
                return List.Find(x => x.ID == id);
            }
            catch (Exception e)
            {
                //Create(name);
                DB.Attention($"Couldn't find Armor of id {id}");
                return null; //lol
            }
        }
        public static Armor Get(string name)
        {
            if (List.Exists(x => x.Name == name))
                return List.Find(x => x.Name == name);
            else
                DB.Attention($"Couldn't find Armor of name {name}");
            return null;
        }
    }
}