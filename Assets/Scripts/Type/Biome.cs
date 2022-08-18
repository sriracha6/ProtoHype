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

        public override string ToString()
        {
            return $"Average Temperature: {averageTemperature}, Flatness: {flatnessPreference.ToString()}";
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

        public override string ToString()
        {
            string x = "";
            int index = 0;
            foreach(TerrainType t in terrain)
            {
                x += $"{t.Name} | {frequencies[index]}";
                x += "\n";
                index++;
            }
            return x;
        }
    }

    public class Biome : Item
    {
        [XMLItem("Location Data")] public LocationData locationData;
        [XMLItemList("Weather Frequencies")] public List<Weather> weatherFrequencies;
        [XMLItem("Terrain Frequencies")] public TerrainFrequencies terrainFrequencies;
        [XMLLinkList("Flora")] public List<Buildings.Plant> flora;
        [XMLItem("Water Comminality")] public float waterComminality;
        public TerrainType WaterClampTT;

        public Color color;
        public float plantDensity;

        public Biome(string name, string description, LocationData locationData, List<Weather> weatherFrequencies,
            TerrainFrequencies terrainFrequencies, List<Buildings.Plant> flora, Color color, float plantDensity, float waterComminality, TerrainType waterClampTT) : base(name, description, "")
        {
            this.locationData = locationData;
            this.weatherFrequencies = weatherFrequencies;
            this.terrainFrequencies = terrainFrequencies;
            this.flora = flora;
            this.color = color;
            this.plantDensity = plantDensity;
            this.waterComminality = waterComminality;
            this.WaterClampTT = waterClampTT;
        }

        public static List<Biome> List = new List<Biome>();

        public static Biome Create(string name, string description, LocationData locationData, List<Weather> weatherFrequencies,
            TerrainFrequencies terrainFrequencies, List<Buildings.Plant> flora, Color color, float plantDensity, float waterComminality, TerrainType WaterClampTT)
        {
            //if(!BiomeList.Any(x => x.Name == name))
            //{
            Biome c = new Biome(name, description, locationData, weatherFrequencies, terrainFrequencies, flora, color, plantDensity, waterComminality, WaterClampTT);
            List.Add(c);
            return c;
            //}
            //else
            //{
            //    Debug.Log("Tried to create multiple of: "+name);
            //    return null;
            //}
        }
        public static Biome Get(string name) // does this really need ids because if you have 2 different of the same it's not like it matters anyway
        {
            if (List.Exists(x => x.Name == name))
            {
                return List.Find(x => x.Name == name);
            }
            else
            {
                DB.Attention($"Couldn't find Biome of name {name}");
                return null;
            }
        }
    }
}