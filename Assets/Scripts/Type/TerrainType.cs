using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum SpecialType { None, Water, Solid }

[System.Serializable]
public class TerrainType
{
    public float height;
    public Color color;
    public string name;
    public RandomTile tbase;
    public SpecialType type;
    public bool supportsNature;

    public TerrainType(string name, float height, Color color, RandomTile tbase, SpecialType special, bool supportedNature)
    {
        this.height = height;
        this.color = color;
        this.name = name;
        this.type = special;
        this.supportsNature = supportedNature;
    }
}

public static class TerrainTypeManager // todo: id this?
{
    public static List<TerrainType> TerrainTypeList = new List<TerrainType>();  

    public static TerrainType Create(string name, float height, Color color, RandomTile tbase, SpecialType special, bool supportedNature)
    {
        if (!TerrainTypeList.Any(x => x.name == name))
        {
            TerrainType c = new TerrainType(name, height, color, tbase, special, supportedNature);
            TerrainTypeList.Add(c);
            return c;
        }
        else
        {
            //Debug.Log("Tried to create multiple of: "+name);
            return null;
        }
    }
    public static TerrainType Get(string name)
    {
        try
        {
            return TerrainTypeList.Find(x => x.name == name);
        }
        catch (NullReferenceException)
        {
            //Create(name);
            return null;
            //return CountryList.Find(x => x.Name == name);
        }
    }
}