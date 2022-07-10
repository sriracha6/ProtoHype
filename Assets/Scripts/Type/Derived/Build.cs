using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buildings;
using UnityEngine.Tilemaps;

static class BuildList
{
    public static List<Build> builds = new List<Build>();
}
public class Build
{
    [XMLItem("Name")] public string Name;
    [XMLItem("Description", multiline = true)] public string Description;
    public int ID { get; private set; }

    public bool isSpecialPlace;
    [XMLItem("Has Rubble")] public bool hasRubble;
    [XMLItem("Rubble Type")] public RubbleType rubbleType;

    [XMLItem("Max Hitpoint")] public int maxHitpoints;
    [XMLItem("Hitpoints")] public int hitpoints;
    [XMLItem("Flammability")] public int flammability;
    public RuleTile tile;

    public Build(string name, string description, bool isSpecialPlace, bool hasRubble, RubbleType rubbleType, int hitpoints, int flammability)
    {
        this.Name = name;
        this.Description = description;
        BuildList.builds.Add(this);
        this.ID = BuildList.builds.Count;

        this.isSpecialPlace = isSpecialPlace;
        this.hasRubble = hasRubble;
        this.rubbleType = rubbleType;
        this.maxHitpoints = hitpoints;
        this.hitpoints = maxHitpoints;
        this.flammability = flammability;
    }
}
