using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buildings;

namespace Structures
{
    public struct FurnitureStats
    {
        public List<(Furniture furniture, (int min, int max) count)> requiredFurniture;
        public List<List<(Furniture furniture, (int min, int max) count)>> groups;

        public FurnitureStats(List<(Furniture, (int, int))> requiredFurniture, List<List<(Furniture, (int, int))>> groups)
        {
            this.requiredFurniture = requiredFurniture;
            this.groups = groups;
        }
    }
    public class Room
    {
        public string Name;
        public FurnitureStats Furniture;
        public List<Vector2Int> possibleSizes = new List<Vector2Int>();
        public List<Floor> possibleFloors;

        public Room(string name, FurnitureStats furniture, List<Vector2Int> possibleSizes, List<Floor> possibleFloors)
        {
            Name = name;
            Furniture = furniture;
            this.possibleSizes = possibleSizes;
            this.possibleFloors = possibleFloors;
        }

        public static List<Room> List = new List<Room>();

        public static Room Create(string name, FurnitureStats furniture, List<Vector2Int> possibleSizes, List<Floor> possibleFloors)
        {
            if (!List.Exists(x => x.Name == name))
            {
                Room c = new Room(name, furniture, possibleSizes, possibleFloors);
                List.Add(c);
                return c;
            }
            else
            {
                //Debug.Log("Tried to create multiple of: "+name);
                return null;
            }
        }
        public static Room Get(string name)
        {
            if(List.Exists(x => x.Name == name))
            {
                return List.Find(x => x.Name == name);
            }
            else
            {
                //Create(name);
                if(name != "false")
                    DB.Attention($"Couldn't find Room of name \"{name}\"");
                return null;
                //return CountryList.Find(x => x.Name == name);
            }
        }
    }
}