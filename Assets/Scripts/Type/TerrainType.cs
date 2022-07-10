using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum SpecialType { None, Water, Mountain }

[System.Serializable]
public class TerrainType
{
    [XMLItem("Name")] public string name;
    [XMLItem("Height")] public float height;
    [XMLItem("Walk Speed")] public float walkSpeed;
    public Color color;
    public SpecialType type;
    [XMLItem("Supports Nature")] public bool supportsNature;
    public FuckBitchTile tile;
    public RuleTile thisIsVeryBadSpaghettiButImOutOfIdeas;

    public TerrainType(string name, float height, Color color, FuckBitchTile tbase, SpecialType special, bool supportedNature)
    {
        this.height = height;
        this.color = color;
        //this.m_Sprites = tbase;
        this.name = name;
        this.walkSpeed = 1f-height;
        this.type = special;
        this.supportsNature = supportedNature;
    }


    public TerrainType(string name, float height, Color color, RuleTile tbase, SpecialType special, bool supportedNature)
    {
        this.height = height;
        this.color = color;
        //this.m_Sprites = tbase;
        this.thisIsVeryBadSpaghettiButImOutOfIdeas = tbase;
        this.name = name;
        this.type = special;
        this.supportsNature = supportedNature;
    }

    public static List<TerrainType> List = new List<TerrainType>();

    public static TerrainType Create(string name, float height, Color color, FuckBitchTile tbase, SpecialType special, bool supportedNature)
    {
        //if (!TerrainTypeList.Any(x => x.name == name))
        //{
        TerrainType c = new TerrainType(name, height, color, tbase, special, supportedNature);
        List.Add(c);
        return c;
        //}
        //else
        //{
        //Debug.Log("Tried to create multiple of: "+name);
        return null;
        //}
    }
    public static TerrainType Get(string name)
    {
        try
        {
            return List.Find(x => x.name == name);
        }
        catch (NullReferenceException)
        {
            //Create(name);
            DB.Attention($"Couldn't find TerrainType of name {name}");
            return null;
            //return CountryList.Find(x => x.Name == name);
        }
    }
}