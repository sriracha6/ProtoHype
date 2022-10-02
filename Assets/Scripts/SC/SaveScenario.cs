using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using PawnFunctions;
using UnityEngine;
using Buildings;

using static TilemapPlace;

public static class SaveScenario
{
    static List<(string primary, string secondary, string shield, string animal, string armor, string inventory)> writtenPawns = new List<(string, string, string, string, string, string)>();
    static List<Build> builds;
    static List<(Furniture b, int rot)> furndor;
    static Build[] flows;
    static Build[] rovs;

    public static void SaveScenarioToPath(string filepath)
    {
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Indent = true;
        settings.NewLineOnAttributes = true;

        using XmlWriter w = XmlWriter.Create(filepath, settings);
        w.WriteStartDocument();
        w.WriteComment("Auto-Generated");
        w.WriteStartElement("Save");
        w.WriteStartElement("Scenario");
        w.WriteEl("Name", SCPauseMenu.Name);
        w.WriteEl("Description", SCPauseMenu.Description);
        w.WriteEl("IconPath", SCPauseMenu.LocalPath);   
        w.WriteEndElement();
        w.WriteStartElement("Settings");
        w.WriteEl("Biome", MapGenerator.I.currentBiome.Name);
        w.WriteEl("Weather", WeatherManager.currentWeather);
        w.WriteEl("IsWeather", WeatherManager.I.isWeather);
        w.WriteEl("IsSun", PositionSun.I.doDayCycle);
        w.WriteEl("Friends", string.Join(",", Player.friends));
        w.WriteEl("Enemies", string.Join(",", Player.enemies));
        w.WriteEndElement();
        w.WriteStartElement("Defs");
        w.WriteStartElement("Regiments");
            foreach (Regiments.Regiment r in Regiments.Regiment.List)
            {
                w.WriteStartElement("R");
                w.WriteEl("TT", r.type.Name);
                w.WriteEl("Country", r.countryOrigin);
                w.WriteEl("ID", r.id);
                w.WriteEndElement();
            }
        w.WriteEndElement();
        w.  WriteStartElement("Pawns"); // this is dumb. we have to check each pawn to every other pawn
            foreach(Pawn x in PawnManager.allPawns) // .DISTINCT() doesnt FUKING WORK THIS HAS TAKEN HOURS
                WritePawn(w, x);
        w.  WriteEndElement();
        w.  WriteStartElement("Terrain");
            TerrainType[] tts = GetDistinct(tilemap);
            for (int i = 0; i < tts.Length; i++)
                WriteDef(w, 'T', i, tts[i].Name);
        w.  WriteEndElement();
        WriteBuildingDefs(w);
        w.WriteEndElement();
        w.WriteStartElement("Locs");
        w.  WriteStartElement("Terrain");
        w.  WriteAttrib("size", MapGenerator.I.mapWidth);
            string tval = WriteMap(tilemap, tts);
        w.  WriteValue(tval);
        w.  WriteEndElement();
        w.  WriteEl("Buildings", WriteMap(allbuilds, builds.ToArray()));
        w.  WriteEl("Floors", WriteMap(floors, flows));
        w.  WriteEl("Rooves", WriteMap(rooves, rovs));
            string firePoses = "";
            foreach (Vector2 v in FireManager.firePositions)
                firePoses += $"{(int)v.x},{(int)v.y},{(int)FireManager.fires.Find(x=>(Vector2)x.transform.position == v).Size},";
        w.  WriteEl("Fires", firePoses.Length == 0 ? "" : firePoses.Substring(0, firePoses.Length - 2));
        w.WriteEndElement();
        w.WriteEndDocument();
        w.Close();
        ClearData();
    }

    static void ClearData()
    {
        builds.Clear();
        furndor.Clear();
        flows = null;
        rovs = null;
    }

    public static void SaveStructure(string filepath)
    {
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Indent = true;
        settings.NewLineOnAttributes = true;

        using XmlWriter w = XmlWriter.Create(filepath, settings);
        w.WriteStartElement("Structure");
        w.WriteEl("MapSize", MapGenerator.I.mapWidth);
        WriteBuildingDefs(w);
        w.WriteEl("LBuildings", WriteMap(allbuilds, builds.ToArray()));
        w.WriteEl("LFloors", WriteMap(floors, flows));
        w.WriteEl("LRooves", WriteMap(rooves, rovs));
        w.WriteEndDocument();
        ClearData();
    }

    static void WriteBuildingDefs(XmlWriter w)
    {
        w.WriteStartElement("Buildings");
        builds = new List<Build>();
        builds.AddRange(GetDistinct(traps));
        builds.AddRange(GetDistinct(specials));
        builds.AddRange(GetDistinct(buildings));
        builds.AddRange(GetDistinct(doors));
        for (int i = 0; i < builds.Count; i++) WriteBuild(w, i, 0, builds[i].Name);

        furndor = new List<(Furniture, int)>();
        furndor = furnitureInfo.ConvertAll<(Furniture f, int rotation)>(x => (x.furn, (int)x.rotation)).Distinct().OrderBy(x => x.rotation).ToList();
        for (int i = 0; i < furndor.Count; i++) WriteBuild(w, builds.Count + i, furndor[i].rot, furndor[i].b.Name);
        builds.AddRange(furndor.ConvertAll(x => x.b));
        w.WriteEndElement();
        w.WriteStartElement("Floors");
        flows = GetDistinct(floors);
        for (int i = 0; i < flows.Length; i++)
            WriteDef(w, 'F', i, flows[i].Name);
        w.WriteEndElement();
        w.WriteStartElement("Rooves");
        rovs = GetDistinct(rooves);
        for (int i = 0; i < rovs.Length; i++)
            WriteDef(w, 'R', i, rovs[i].Name);
        w.WriteEndElement();
    }

    static T[] GetDistinct<T>(T[,] source)
    {
        T[] d1 = new T[source.Length];
        int write = 0;
        for (int i = 0; i < source.GetLength(0); i++)
            for (int j = 0; j < source.GetLength(1); j++)
                d1[write++] = source[i, j];
        T[] tts = d1.Distinct().Where(x=>x!=null).ToArray();
        return tts;
    }

    static void WritePawn(XmlWriter w, Pawn p)
    {
        // if (writtenPawns.Exists(x => x == (p.hasPrimary ? p.heldPrimary.Name : null, p.hasSidearm ? p.heldSidearm.Name : null, p.hasShield ? p.shield.Name : null, p.animal == null ? null : p.animal.sourceAnimal.ToString(), p.armor.Count > 0 ? string.Join(",", p.armor) : null, p.inventory == null ? null : p.inventory.Name))) return; // distinct doesnt fucking work.
        w.WriteStartElement("P");
        if(p.hasPrimary) w.  WriteEl("P", p.heldPrimary);
        if(p.hasSidearm) w.  WriteEl("S", p.heldSidearm);
        if(p.hasShield) w.  WriteEl("Sh", p.shield);
        if(p.animal != null) w.  WriteEl("An", p.animal.sourceAnimal);
        if(p.inventory != null) w.  WriteEl("I", p.inventory);
        if(p.armor.Count > 0) w.  WriteEl("A", string.Join(",", p.armor));
        w.  WriteEl("R", p.regiment.id);
        w.  WriteEl("L", $"{p.transform.position.x},{p.transform.position.y}");
        w.WriteEndElement();
        //writtenPawns.Add((p.hasPrimary ? p.heldPrimary.Name : null, p.hasSidearm ? p.heldSidearm.Name : null, p.hasShield ? p.shield.Name : null, p.animal == null ? null : p.animal.sourceAnimal.ToString(), p.armor.Count > 0 ? string.Join(",", p.armor) : null, p.inventory == null ? null : p.inventory.Name));
    }
    
    static void WriteDef(XmlWriter w, char prefix, int id, string contents)
    {
        w.WriteStartElement(prefix.ToString()+id);
        w.  WriteValue(contents);
        w.WriteEndElement();
    }

    static void WriteBuild(XmlWriter w, int id, int rotation, string contents)
    {
        w.WriteStartElement("B"+id);
        if(rotation != 0)
            w.WriteAttrib("rot", rotation);
        w.  WriteValue(contents);
        w.WriteEndElement();
    }

    static string WriteMap<T>(T[,] map, T[] choices) where T : class
    {
        string tval = "";
        int streak = 0;
        int lastK = -1;
        List<T> list = choices.ToList();
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                for (int k = 0; k < choices.Length; k++)
                    if (map[i, j] == choices[k])
                    {
                        if (list.FindAll(x => x == choices[k]).Count > 1) // furniture
                            k = k + ((int)furnitureInfo.Find(x => x.pos == new Vector2Int(i, j)).rotation / 90);
                        if (k == lastK) streak++;
                        else
                        {
                            tval += $"{streak}|{(k == -1 ? "X" : k.ToString())},";
                            streak = 1; //streak = 0;
                        }
                        lastK = k;
                    }
                    else if (map[i, j] == null)
                    {
                        lastK = -1;
                        streak++;
                    }
            }
        }
        if (tval.Length > 0)
            return tval.Substring(0, tval.Length - 1);
        else return "";
    }
}
