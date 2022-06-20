using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PawnFunctions;
using Weapons;
using Armors;

/// <summary>
/// Not sure if this creates a shit ton of extra memory. Too bad!
/// </summary>
public class Gear
{
    public List<Armor> armor = new List<Armor>();
    public Item weapon;

    public Gear(List<Armor> armo, Item wpn)
    {
        armor = armo;
        weapon = wpn;
    }
}