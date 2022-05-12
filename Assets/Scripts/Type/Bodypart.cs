using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Body
{
    public enum PartType
    {
        VitalOrgan,
        Organ,
        Bone,
        Skin
    }
    public enum CountType
    {
        False,
        Sides,
        Numbered
    }
    public enum VitalSystem
    {
        None,
        Dexterity,
        BloodPumping,
        Breathing,
        Sight,
        Moving,
        Conciousness
    }
    public enum EffectAmount
    {
        Normal,
        Minor
    }
    public enum HitChance
    {
        Normal,                                                                               
        Elevated                                                                              
    }                                                                                         
                                                                                              
    public static class BodypartManager                                                       
    {                                                                                         
        public static List<Bodypart> BodypartList { get; private set; } = new List<Bodypart>();

        public static Bodypart Create(string name, float hp, PartType pt, Bodypart parent, float pfactor, float bfactor,
            float dfactor, int cunt, VitalSystem effectz, EffectAmount eamont, HitChance hchance, CountType counttype)
        {
            if (!BodypartList.Any(x => x.Name == name))
            {
                Bodypart c = new Bodypart(name, hp, pt, parent, pfactor, bfactor, dfactor, cunt, effectz, eamont, hchance, counttype);
                BodypartList.Add(c);
                return c;
            }
            else
            {
                //Debug.Log("Tried to create multiple of: "+name);
                return null;
            }
        }
        public static Bodypart Get(string name)
        {
            try
            {
                return new Bodypart(BodypartList.Find(x => x.Name == name)); // so no modifiying bc no readonly classes
            }
            catch (NullReferenceException)
            {
                //Create(name);
                return null;
                //return CountryList.Find(x => x.Name == name);
            }
        }
    }
    public class Bodypart
    {
        public string Name;
        public float TotalHP { get; private set; }
        public float HP;
        public PartType type;
        public Bodypart partOf;
        public List<Wound> wounds = new List<Wound>();

        public float painFactor;
        public float bleedingFactor;
        public float damageMultiplier;
        public float effectiveness; // percent 0.85*x

        public int count;
        public VitalSystem effects;
        public EffectAmount effectAmount;

        public HitChance hitChance;
        public CountType countType;

        public Bodypart(string name, float hp, PartType pt, Bodypart parent, float pfactor, float bfactor, 
            float dfactor, int cunt, VitalSystem effectz, EffectAmount eamont, HitChance hchance, CountType ct)
        {
            Name = name;
            HP = hp;
            TotalHP = hp;
            effectiveness = hp;
            type = pt;
            partOf = parent;
            painFactor = pfactor;
            bleedingFactor = bfactor;
            damageMultiplier = dfactor;
            count = cunt; // lol
            effects = effectz;
            effectAmount = eamont;
            hitChance = hchance;
            countType = ct;
        }

        public Bodypart(Bodypart bp)
        {
            Name = bp.Name;
            HP = bp.HP;
            TotalHP = bp.TotalHP;
            effectiveness = bp.effectiveness;
            type = bp.type;
            partOf = bp.partOf;
            painFactor = bp.painFactor;
            bleedingFactor = bp.bleedingFactor;
            damageMultiplier = bp.damageMultiplier;
            count = bp.count;
            effects = bp.effects;
            effectAmount = bp.effectAmount;
            hitChance = bp.hitChance;
            countType = bp.countType;
        }
    }
}