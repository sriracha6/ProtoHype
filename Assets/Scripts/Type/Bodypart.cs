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
                                                                                             
    public class Bodypart
    {
        public string Name;
        public float TotalHP { get; }
        public float HP;
        public float bleedingRate;
        public PartType type;
        public string _partOf;
        public Bodypart partOf 
        {
            get
            {
                if (_partOf != "false" && !string.IsNullOrEmpty(_partOf))
                    if (count == 1)
                        return Get(_partOf);
                    else if (Get(_partOf).countType == CountType.Numbered)
                    {
                        return Get(Name.Split(' ')[0] + " " + _partOf);
                    }
                    else return null;
                else return null; 
            } 
        }
        public List<Wound> wounds = new List<Wound>();
        public string group;

        public float painFactor;
        public float bleedingFactor { get; }
        public float damageMultiplier;
        public float effectiveness; // percent 0.85*x

        public int count { get; }
        public VitalSystem effects { get;  }
        public EffectAmount effectAmount { get; }

        public HitChance hitChance { get; }
        public CountType countType { get; }

        public Bodypart(string name, float hp, PartType pt, string parent, float pfactor, float bfactor, 
            float dfactor, int cunt, VitalSystem effectz, EffectAmount eamont, HitChance hchance, CountType ct, string group)
        {
            Name = name;
            HP = hp;
            TotalHP = hp;
            effectiveness = hp;
            type = pt;
            _partOf = parent;
            painFactor = pfactor;
            bleedingFactor = bfactor;
            damageMultiplier = dfactor;
            count = cunt; // lol
            effects = effectz;
            effectAmount = eamont;
            hitChance = hchance;
            countType = ct;
            this.group = group;
        }

        public Bodypart(Bodypart bp)
        {
            Name = bp.Name;
            HP = bp.HP;
            TotalHP = bp.TotalHP;
            effectiveness = bp.effectiveness;
            type = bp.type;
            _partOf = bp._partOf;
            painFactor = bp.painFactor;
            bleedingFactor = bp.bleedingFactor;
            damageMultiplier = bp.damageMultiplier;
            count = bp.count;
            effects = bp.effects;
            effectAmount = bp.effectAmount;
            hitChance = bp.hitChance;
            countType = bp.countType;
            this.group = bp.group;
        }

        public static List<Bodypart> List { get; private set; } = new List<Bodypart>();

        public static Bodypart Create(string name, float hp, PartType pt, string parent, float pfactor, float bfactor,
            float dfactor, int cunt, VitalSystem effectz, EffectAmount eamont, HitChance hchance, CountType counttype, string group)
        {
            //Debug.Log($"I'm making a {name}");
            if (!List.Any(x => x.Name == name))
            {
                Bodypart c = new Bodypart(name, hp, pt, parent, pfactor, bfactor, dfactor, cunt, effectz, eamont, hchance, counttype, group);
                List.Add(c);
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
                return new Bodypart(List.Find(x => x.Name == name)); // so no modifiying bc no readonly classes
            }
            catch (NullReferenceException)
            {
                //Create(name);
                DB.Attention($"Couldn't find Bodypart of name \"{name}\"");
                return null;
                //return CountryList.Find(x => x.Name == name);
            }
        }
    }
}