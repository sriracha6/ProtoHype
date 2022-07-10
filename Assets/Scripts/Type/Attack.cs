using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PawnFunctions;
using System;

namespace Attacks
{
    public enum DamageType
    {
        Sharp,
        Blunt,
        None
    }
    public class Attack
    {
        public string Name;
        public DamageType damageType; 
        public bool isRare;
        public float Damage;

        public Attack(string name, DamageType dtype, bool rare, float dmg)
        {
            Name = name;
            damageType = dtype;
            isRare = rare;
            Damage = dmg;
        }

        public override string ToString()
        {
            return $"{Name} | {(isRare ? "Rare" : "")} {damageType.ToString()} : {Damage} damage";
        }
    }
}