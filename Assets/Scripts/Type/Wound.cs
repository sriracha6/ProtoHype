using Attacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weapons;

public class Wound
{
    public string type;
    public float bleedRate;
    public Weapon sourceWeapon;
    public Attack sourceAttack;
    public float damage;

    public Wound(string t, Weapon fromWeapon, float dmg,Attack attack,float brate=0f)
    {
        type = t;
        bleedRate = brate;
        damage = dmg;
        sourceAttack = attack;
        sourceWeapon = fromWeapon;
    }
}
