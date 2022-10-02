using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum SpecialType { None, Water, Mountain }

[System.Serializable]
public class TerrainType : Item
{
    [XMLItem("Height")] public float height;
    [XMLItem("Walk Speed")] public float walkSpeed;
    public Color color;
    public SpecialType type;
    [XMLItem("Supports Nature")] public bool supportsNature;
    public FuckBitchTile tile;
    public RuleTile thisIsVeryBadSpaghettiButImOutOfIdeas;

    public TerrainType(string name, string sourcefile, float height, Color color, FuckBitchTile tbase, SpecialType special, bool supportedNature)
        : base(name, "", sourcefile)
    {
        this.height = height;
        this.color = color;
        //this.m_Sprites = tbase;
        this.walkSpeed = 1f-height;
        this.type = special;
        this.supportsNature = supportedNature;
    }


    public TerrainType(string name, string sourcefile, float height, Color color, RuleTile tbase, SpecialType special, bool supportedNature)
        : base(name, "", sourcefile)
    {
        this.height = height;
        this.color = color;
        //this.m_Sprites = tbase;
        this.thisIsVeryBadSpaghettiButImOutOfIdeas = tbase;
        this.type = special;
        this.supportsNature = supportedNature;
    }

    public static List<TerrainType> List = new List<TerrainType>();

    public static TerrainType Create(string name, string sourcefile, float height, Color color, FuckBitchTile tbase, SpecialType special, bool supportedNature)
    {
        TerrainType c = new TerrainType(name, sourcefile, height, color, tbase, special, supportedNature);
        List.Add(c);
        return c;
    }
    public static TerrainType Get(string name)
    {
        if (name == "Water") return WCMngr.WaterTT;
        if (name == "Mountain") return WCMngr.MountainTT; 
        if(List.Exists(x=>x.Name == name))
            return List.Find(x => x.Name == name);
        else    
            DB.Attention($"Couldn't find TerrainType of name {name}");
            return null;
    }
}