using System.Collections;
using System.Xml;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Nature;
using Countries;
using Regiments;
using Buildings;
using Structures;
using UnityEngine.UIElements;

public static class LoadScenario
{
    /*struct BuiltBuild
    {
        public string Name;
        public float Rotation;

        public BuiltBuild(string Name, float Rotation)
        {
            this.Name = Name; this.Rotation = Rotation;
        }
    }*/
    struct MapSegment
    {
        public int Count;
        public int Id;

        public MapSegment(int Count, int Id)
        {
            this.Count = Count; this.Id = Id;
        }
    }
    public static float currentRotation;
    static (int id, float rotation)[] buildsd;
    internal static int[,] rotations;

    static Build[] builds;
    static Floor[] floors;
    static Roof[] rooves;
    static TerrainType[] terrains;

    public static void LoadScenarioFromPath(string filepath)
    {
        var xml = XMLLoader.Loaders.LoadXML(filepath);
        var defs = xml.Q<XmlNode>("Defs");
        var locs = xml.Q<XmlNode>("Locs");
        var settings = xml.Q<XmlNode>("Settings");
        var scenario = xml.Q<XmlNode>("Scenario");
        var pawns = defs.Q<XmlNode>("Pawns").Qs("P");
        var regs = defs.Q<XmlNode>("Regiments").Qs("R");

        // ------------------- SETTINGS
        MapGenerator.I.currentBiome = Biome.Get(settings.Q<string>("Biome"));
        WeatherManager.currentWeather = settings.Enum<WeatherManager.WeatherType>(settings.Q<string>("Weather"));
        WeatherManager.I.isWeather = settings.Q<bool>("IsWeather");
        PositionSun.I.doDayCycle = settings.Q<bool>("IsSun");
        Player.friends = settings.Q<string>("Friends").Split(',').ToList().ConvertAll(x=>Country.Get(x));
        Player.enemies = settings.Q<string>("Enemies").Split(',').ToList().ConvertAll(x=>Country.Get(x));
        // ------------------- END SETTINGS
        // ----------------- DEFS
        if(regs!=null)
        for(int i = 0; i < regs.Count; i++)
            Regiment.Create(TroopTypes.TroopType.Get(regs[i].Q<string>("TT"), Country.Get(regs[i].Q<string>("Country"))), Country.Get(regs[i].Q<string>("Country")),
                regs[i].Q<int>("ID"));
        if(pawns!=null)
        for(int i = 0; i < pawns.Count; i++)
        {
            var x = pawns[i];
            Regiment r = Regiment.Get(x.Q<int>("R"));
            PawnManager.I.CreatePawn(!Menus.I.inSC, r.countryOrigin, 
                CachedItems.RandomName, r, x.Q<Vector2>("L"), 
                x.HasNode("I") ? Projectiles.Projectile.Get(x.Q<string>("I")) : null,
                x.HasNode("P") ? Weapons.Weapon.Get(x.Q<string>("P")) : null, 
                x.HasNode("S") ? Weapons.Weapon.Get(x.Q<string>("S")) : null, 
                x.HasNode("Sh") ? Shields.Shield.Get(x.Q<string>("Sh")) : null,
                x.HasNode("An") ? Animals.Animal.Get(x.Q<string>("An")) : null,
                x.HasNode("A") ? x.Q<string>("A").Split(',').ToList().ConvertAll(x=>Armors.Armor.Get(x)) : new List<Armors.Armor>());
        }
        var tts = defs.Q<XmlNode>("Terrain").ChildNodes;
        var bbs = defs.Q<XmlNode>("Buildings").ChildNodes;
        var fs = defs.Q<XmlNode>("Floors").ChildNodes;
        var rs = defs.Q<XmlNode>("Rooves").ChildNodes;

        terrains = new TerrainType[tts.Count];
        for (int i = 0; i < tts.Count; i++) terrains[i] = TerrainType.Get(tts[i].InnerText);

        LoadBuildDefs(bbs, fs, rs);
        // ----------------- END DEFS
        // ------------------- LOCS
        int mapSize = int.Parse(locs.Q<XmlNode>("Terrain").Attrib(0));
        rotations = new int[mapSize, mapSize];
        MapGenerator.I.mapWidth = mapSize;
        MapGenerator.I.mapHeight = mapSize;
        var map = GenMap(mapSize, ParseMapSegs(locs.Q<string>("Terrain")), terrains);
        TilemapPlace.UpdateBuildings();
        TilemapPlace.tilemap = map;
        TilemapPlace.UpdateTilemap();
        var buildmap = GenMap(mapSize, ParseMapSegs(locs.Q<string>("Buildings")), builds);
        for (int i = 0; i < map.GetLength(0); i++)
            for (int j = 0; j < map.GetLength(1); j++)
                TilemapPlace.SetAll(new List<Build>() { buildmap[i,j] },i,j);
        var floormap = GenMap(mapSize, ParseMapSegs(locs.Q<string>("Floors")), floors);
        for (int i = 0; i < floormap.GetLength(0); i++) for (int j = 0; j < floormap.GetLength(1); j++) TilemapPlace.SetFloor(floormap[i,j], i, j);

        var roofmap = GenMap(mapSize, ParseMapSegs(locs.Q<string>("Rooves")), rooves);
        for (int i = 0; i < roofmap.GetLength(0); i++) for (int j = 0; j < roofmap.GetLength(1); j++) RoofPlacer.I.PlaceRoof(roofmap[i,j], i, j);

        if(locs.Q<string>("Fires").Length > 0)
        {
            int[] fireNums = locs.Q<string>("Fires").Split(',').ToList().ConvertAll(x=>int.Parse(x)).ToArray();
            for(int i = 0; i < fireNums.Length; i+=3)
            {
                var go = GameObject.Instantiate(WCMngr.I.firePrefab);
                Vector2Int p = new Vector2Int(fireNums[i], fireNums[i+1]);
                go.transform.position = new Vector3(p.x, p.y, -2);
                go.GetComponent<FireBehaviour>().Size = fireNums[i + 2];
            }
        }

        // ------------------- END LOCS
        //MapGenerator.I.GenerateTheStuffWeNeed();
    }

    static void LoadBuildDefs(XmlNodeList bbs, XmlNodeList fs, XmlNodeList rs)
    {
        buildsd = new (int id, float rotation)[bbs.Count];
        builds = new Build[bbs.Count];
        for (int i = 0; i < bbs.Count; i++)
        {
            builds[i] = (Build)Build.GetGenericItem(bbs[i].InnerText);//, bbs[i].Attributes.Count > 0 ? float.Parse(bbs[i].Attrib(0)) : 0);
            buildsd[i] = (i, bbs[i].Attributes.Count > 0 ? float.Parse(bbs[i].Attrib(0)) : 0);
        }
        floors = new Floor[fs.Count];
        for (int i = 0; i < fs.Count; i++) floors[i] = Floor.Get(fs[i].InnerText);
        rooves = new Roof[rs.Count];
        for (int i = 0; i < rs.Count; i++) rooves[i] = Roof.Get(rs[i].InnerText);
    }

    public static void PlaceStructure(string path, System.Random rng, SliderInt mapSizeSlider)
    {
        // set the mapgenerator.structurepos and size
        // messages.add if too big, let the player know the map is being resized
        var x = XMLLoader.Loaders.LoadXML(path);
        LoadBuildDefs(x.Qs("Buildings"),x.Qs("Floors"), x.Qs("Rooves"));
        int mapSize = x.Q<int>("MapSize");
        MapGenerator.I.mapWidth = mapSize;
        MapGenerator.I.mapHeight = mapSize;
        mapSizeSlider.lowValue = mapSize;
        mapSizeSlider.value = mapSize;
        mapSizeSlider.highValue = mapSize;

        var buildmap = GenMap(mapSize, ParseMapSegs(x.Q<string>("LBuildings")), builds);
        for (int i = 0; i < buildmap.GetLength(0); i++)
            for (int j = 0; j < buildmap.GetLength(1); j++)
                TilemapPlace.SetAll(new List<Build>() { buildmap[i, j] }, i, j);
        var floormap = GenMap(mapSize, ParseMapSegs(x.Q<string>("LFloors")), floors);
        for (int i = 0; i < floormap.GetLength(0); i++) for (int j = 0; j < floormap.GetLength(1); j++) TilemapPlace.SetFloor(floormap[i, j], i, j);

        var roofmap = GenMap(mapSize, ParseMapSegs(x.Q<string>("LRooves")), rooves);
        for (int i = 0; i < roofmap.GetLength(0); i++) for (int j = 0; j < roofmap.GetLength(1); j++) RoofPlacer.I.PlaceRoof(roofmap[i, j], i, j);
    }

    static List<MapSegment> ParseMapSegs(string input)
    {
        List<MapSegment> mapsegs = new List<MapSegment>();
        foreach(string s in input.Split(',').ToList().Take(input.Split(',').Length-1))
        {
            string[] split = s.Split('|');
            mapsegs.Add(new MapSegment(int.Parse(split[0]), split[1] != "X" ? int.Parse(split[1]) : -1));
        }
        return mapsegs;
    }

    static T[,] GenMap<T>(int size, List<MapSegment> mapsegs, T[] choices) where T : Item
    {
        T[,] map = new T[size,size];
        int x=0;
        int y=0;
        List<T> choicesList = choices.ToList();
        int t = 0;
        for(int i = 0; i < mapsegs.Count; i++)
        {
            t += mapsegs[i].Count;
        }
        Debug.Log($"{t}");
        for(int i = 0; i < mapsegs.Count; i++)
        {
            Item choice;
            int count = mapsegs[i].Count;
            if (mapsegs[i].Id == -1)
                choice = null;
            else
            {
                choice = (T)Item.GetGenericItem(choicesList[mapsegs[i].Id].ID);
                if(buildsd.Where(d=>mapsegs[i].Id==d.id).Count() > 0)
                {
                    rotations[x, y] = (int)buildsd[mapsegs[i].Id].rotation;
                }
            }
            for(int j = 0; j < count; j++)
            {
                //Debug.Log($"{map.GetLength(1)}. getting at {x},{y}");
                map[x, y] = (T)choice;
                if ((x + 1) == size) { y++; x = 0; }
                else x++;
            }
        }
        return map;
    }
}
