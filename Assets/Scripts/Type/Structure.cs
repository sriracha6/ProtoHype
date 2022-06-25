using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buildings;

public enum WorldFeature
{
    River, Seaside, Mountains, Flat, None=0
}
namespace Structures
{
    public class RoomInfo
    {
        public Room room;
        public bool required;
        public bool common;

        public RoomInfo(Room room, bool required, bool common)
        {
            this.room = room;
            this.required = required;
            this.common = common;
        }
    }

    public class Structure
    {
        public string Name;
        public string Description;
        public int InfluenceRange;
        public (int Min, int Max) RoomCount;
        public float RoomScale;
        public List<RoomInfo> RoomInfo;
        public Building ExteriorWalls;
        public Building InteriorWalls;
        public int EntranceCount;
        public List<Door> EntranceDoorPick;
        public bool HasCourtyard;
        public WorldFeature PrefersFeature;
        public Room CornerRoom;
        public List<Door> Doors;
        public Roof Roof;

        public Structure(string name, string description, int influenceRange, (int Min, int Max) roomCount, float roomScale, List<RoomInfo> roomInfo, Building exteriorWalls, Building interiorWalls, int entranceCount, List<Door> entranceDoor, bool hasCourtyard, WorldFeature prefersFeature, Room cornerRoom, List<Door> doors, Roof roof)
        {
            Name = name;
            Description = description;
            InfluenceRange = influenceRange;
            RoomCount = roomCount;
            RoomScale = roomScale;
            RoomInfo = roomInfo;
            ExteriorWalls = exteriorWalls;
            InteriorWalls = interiorWalls;
            EntranceCount = entranceCount;
            EntranceDoorPick = entranceDoor;
            HasCourtyard = hasCourtyard;
            PrefersFeature = prefersFeature;
            CornerRoom = cornerRoom;
            Doors = doors;
            Roof = roof;
        }

        public static List<Structure> List = new List<Structure>();

        public static Structure Create(string name, string description, int influenceRange, (int Min, int Max) roomCount, float roomScale, List<RoomInfo> roomInfo, Building exteriorWalls, Building interiorWalls, int entranceCount, List<Door> entranceDoor, bool hasCourtyard, WorldFeature prefersFeature, Room cornerRoom, List<Door> doors, Roof roof)
        {
            Structure s = new Structure(name, description, influenceRange, roomCount, roomScale, roomInfo, exteriorWalls, interiorWalls, entranceCount, entranceDoor, hasCourtyard, prefersFeature, cornerRoom, doors, roof);
            List.Add(s);
            return s;
        }

        public static Structure Get(string name)
        {
            if (List.Exists(x => x.Name == name))
                return List.Find(x => x.Name == name);
            else
                DB.Attention("Couldn't find structure of name \""+name+"\"");
            return null;
        }
    }
}
