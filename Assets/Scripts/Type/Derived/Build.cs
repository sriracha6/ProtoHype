using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buildings;

static class BuildList
{
    public static List<Build> builds = new List<Build>();
}
public class Build
{
    public string Name;
    public string Description;
    public int ID { get; private set; }

    public bool isSpecialPlace;
    public bool hasRubble;
    public RubbleType rubbleType;

    public int maxHitpoints;
    public int hitpoints;
    public int flammability;

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
