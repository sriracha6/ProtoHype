using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static TilemapPlace;

namespace Nature
{
    public enum FlatnessPreference { None, Flat, Mountainous }
    public struct LocationData
    {
        public float averageTemperature;
        public FlatnessPreference flatnessPreference;

        public LocationData(float averageTemperature, FlatnessPreference flatnessPreference)
        {
            this.averageTemperature = averageTemperature;
            this.flatnessPreference = flatnessPreference;
        }
    }

    public struct Weather // for name           :       use nameof()
    {
        public WeatherManager.WeatherType type;
        public float frequency;

        public Weather(WeatherManager.WeatherType type,float frequency)
        {
            this.type = type;
            this.frequency = frequency;
        }
    }

    public struct TerrainFrequencies
    {
        public List<TerrainType> terrain;
        public List<float> frequencies;

        public TerrainFrequencies(List<TerrainType> terrain, List<float> frequencies)
        {
            this.terrain = terrain;
            this.frequencies = frequencies;
        }
    }

    public static class BiomeManager
    {
        public static List<Biome> BiomeList = new List<Biome>();

        public static Biome Create(string name, string description, LocationData locationData, List<Weather> weatherFrequencies,
            TerrainFrequencies terrainFrequencies, List<Buildings.Nature> flora, Color color, float plantDensity)
        {
            if(!BiomeList.Any(x => x.Name == name))
            {
                Biome c = new Biome(name, description, locationData, weatherFrequencies, terrainFrequencies, flora, color, plantDensity);
                BiomeList.Add(c);
                return c;
            }
            else
            {
                Debug.Log("Tried to create multiple of: "+name);
                return null;
            }
        }
        public static Biome Get(string name) // does this really need ids because if you have 2 different of the same it's not like it matters anyway
        {
            if(BiomeList.Exists(x => x.Name == name))
            {
                return BiomeList.Find(x => x.Name == name);
            }
            else
            {
                //Create(name);
                Debug.Log("ERROR : : : : : : : : : : : : : : : : : : : : : : : : : : : : NULL");
                    Debug.Log(BiomeList.Count);
                foreach(var c in BiomeList)
                {
                }
                Debug.Log(": : : : : : : : : : : : : : : : : : : : : : : : : : : : : : :");
                return null;
                //return CountryList.Find(x => x.Name == name);
            }
        }
    }
    public class Biome : Item
    {
        public LocationData locationData;
        public List<Weather> weatherFrequencies;
        public TerrainFrequencies terrainFrequencies;
        public List<Buildings.Nature> flora;

        public Color color;
        public float plantDensity;

        public Biome(string name, string description, LocationData locationData, List<Weather> weatherFrequencies,
            TerrainFrequencies terrainFrequencies, List<Buildings.Nature> flora, Color color, float plantDensity) : base(name, description, "")
        {
            this.locationData = locationData;
            this.weatherFrequencies = weatherFrequencies;
            this.terrainFrequencies = terrainFrequencies;
            this.flora = flora;
            this.color = color;
            this.plantDensity = plantDensity;
        }
    }
}