using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buildings;
using UnityEngine.Tilemaps;

public class Build : Item// i should really put a float rotation variable in here for everything. 
{
    public bool isSpecialPlace;
    [XMLItem("Has Rubble")] public bool hasRubble;
    [XMLItem("Rubble Type")] public RubbleType rubbleType;

    [XMLItem("Flammability")] public int flammability;
    [XMLItem("Hitpoints")] public int hitpoints;
    [XMLItem("Max Hitpoint")] public int maxHitpoints;

    public RuleTile tile;

    public override string ToString()
    {
        return Name;
    }

    public static bool operator ==(Build lhs, Build rhs)
    {
        if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
            return false;
        
        return lhs.ID == rhs.ID;
    }
    public static bool operator !=(Build lhs, Build rhs)
    {
        if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
            return false;

        return lhs.ID != rhs.ID;
    }

    public Build(string name, string description, string sourcefile, bool isSpecialPlace, bool hasRubble, RubbleType rubbleType, int hitpoints, int flammability)
        : base(name, description, sourcefile)
    {
        this.isSpecialPlace = isSpecialPlace;
        this.hasRubble = hasRubble;
        this.rubbleType = rubbleType;
        this.maxHitpoints = hitpoints;
        this.hitpoints = maxHitpoints;
        this.flammability = flammability;
    }
}
