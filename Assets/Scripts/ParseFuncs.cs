using Attacks;
using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using System.Linq;
using System.IO;

using Random = UnityEngine.Random;

using Weapons;
using Shields;
using Countries;
using Armors;
using TroopTypes;
using Projectiles;
using Animals;
using Body;
using Buildings;
using Generics;
using UnityEngine.Tilemaps;
using Nature;
using Structures;
using static TilemapPlace;
using static CachedItems;
using static WeatherManager;
using System.Globalization;
using XMLLoader;

public static class ParseFuncs
{
    public static float parseFloat(this string text)
    {
        return Convert.ToSingle(text);
    }
    public static List<Weapon> parseWeapons(XmlNode xmls)
    {
        List<Weapon> weapons = new List<Weapon>();

        if (xmls.HasNode("GenericSpecial"))
            foreach (XmlElement x in xmls.Qs("GenericSpecial"))
            {
                if (x.InnerText == "Any")
                    weapons.AddRange(Weapon.List);
                else if (x.InnerText == "None")
                    weapons.Clear();
            }
        if (xmls.HasNode("Generic"))
            foreach (XmlNode x in xmls.Qs("Generic"))
                weapons.AddRange(GenericManager.GetGeneric<Weapon>(x.InnerText));

        foreach (string s in xmls.LastChild.InnerText.Split(',')) // only this tag's text. not children
            weapons.Add(Weapon.Get(s.removeWS()));
        return weapons;
    }
    public static List<Projectile> parseProjectiles(XmlNode xmls)
    {
        List<Projectile> weapons = new List<Projectile>();

        foreach (XmlElement x in xmls.Qs("GenericSpecial"))
        {
            if (x.InnerText == "Any")
                weapons.AddRange(Projectile.List);
            else if (x.InnerText == "None")
                weapons.Clear();
        }

        foreach (string s in xmls.LastChild.InnerText.Split(',')) // only this tag's text. not children
        {
            weapons.Add(Projectile.Get(s.removeWS()));
        }

        if (xmls.HasNode("Generic"))
            foreach (XmlNode x in xmls.Qs("Generic"))
                weapons.AddRange(GenericManager.GetGeneric<Projectile>(x.InnerText));

        return weapons;
    }

    public static List<List<Armor>> parseArmor(XmlNode xmls) // this sucks i have to do this for 8 things 8x the code
    {
        if (xmls == null) return null;
        List<List<Armor>> armor = new List<List<Armor>>();
        List<Armor> normalRequired = new List<Armor>();

        if (xmls.Q<XmlNode>("PickFrom") != null)
            foreach (XmlElement x in xmls.Qs("PickFrom"))
            {
                List<Armor> tempChoice = new List<Armor>();
                foreach (string s in x.InnerText.Split(','))
                    tempChoice.Add(Armor.Get(s.removeWS()));
                armor.Add(tempChoice);
            }
        foreach (string s in xmls.LastChild.InnerText.Split(',')) // only this tag's text. not children
        {
            normalRequired.Add(Armor.Get(s.removeWS()));
        }
        if (xmls.HasNode("Generic"))
            foreach (XmlNode x in xmls.Qs("Generic"))
                armor.Add(GenericManager.GetGeneric<Armor>(x.InnerText));
        armor.Add(normalRequired);
        return armor;
    }
    public static List<Shield> parseShields(XmlNode xmls)
    {
        if (xmls == null)
            return null; // no shields
        List<Shield> shields = new List<Shield>();

        if (xmls.Q<XmlNode>("GenericSpecial") != null)
            foreach (XmlElement x in xmls.Qs("GenericSpecial"))
            {
                if (x.InnerText == "Any")
                    shields.AddRange(Shield.List);
                else if (x.InnerText == "None")
                    shields.Clear();
            }

        foreach (string s in xmls.LastChild.InnerText.Split(',')) // only this tag's text. not children
        {
            shields.Add(Shield.Get(s.removeWS()));
        }
        if (xmls.HasNode("Generic"))
            foreach (XmlNode x in xmls.Qs("Generic"))
                shields.AddRange(GenericManager.GetGeneric<Shield>(x.InnerText));
        return shields;
    }
    public static List<Bodypart> parseAllowedBodyparts(XmlNode xmls)
    {
        List<Bodypart> bps = new List<Bodypart>();

        foreach (string s in xmls.LastChild.InnerText.Split(',')) // only this tag's text. not children
        {
            //bps.Add(BodypartManager.Get(s.removeWS().toTitleCase()));
            if (Bodypart.List.Find(x => x.Name == s.removeWS().toTitleCase()) == null)
                continue;
            if (string.IsNullOrEmpty(Bodypart.Get(s.removeWS().toTitleCase()).group))
                bps.Add(Bodypart.Get(s.removeWS().toTitleCase()));
            else
                bps.AddRange(Bodypart.List.FindAll(x => x.group == Bodypart.Get(s.removeWS().toTitleCase()).group));
        }
        return bps;
    }
    public static List<AnimalArmor> parseAnimalArmor(string text)
    {
        List<AnimalArmor> a = new List<AnimalArmor>();
        foreach (string x in text.Split(','))
            a.Add(AnimalArmor.Get(x.removeWS()));
        return a;
    }

    public static List<Plant> parseFlora(string input)
    {
        List<Buildings.Plant> flora = new List<Buildings.Plant>();

        foreach (string s in input.Split(',')) // only this tag's text. not children
        {
            flora.Add(Buildings.Plant.Get(s.removeWS()));
        }

        return flora;
    }
    public static List<Weather> parseWeatherFrequencies(XmlNode x)
    {
        List<Weather> weatherFrequencies = new List<Weather>();

        foreach (XmlNode node in x.ChildNodes)
        {
            if (node.NodeType != XmlNodeType.Comment)
                weatherFrequencies.Add(new Weather(Enum<WeatherType>(null, node.InnerText),
                    node.Q<float>("frequency", attribute: true)));
        }

        return weatherFrequencies;
    }
    public static TerrainFrequencies parseTerrainFrequencies(XmlNode x)
    {
        List<TerrainType> terrain = new List<TerrainType>();
        List<float> frequencies = new List<float>();

        foreach (XmlElement node in x.ChildNodes)
        {
            terrain.Add(TerrainType.Get(node.InnerText)); // remember this doesnt work yet because i havent created any yet
            float amount;

            if (node.Q<XmlNode>("frequency", attribute: true) == null) amount = 1;

            else amount = node.Q<float>("frequency", attribute: true);

            frequencies.Add(amount);
        }
        // todo: mountains have snow? don't just say no im lazy and delete it may make the game beautiful  

        return new TerrainFrequencies(terrain, frequencies); // todo: frequencies are obsolete if height exists. wtf was i thinking
                                                             // REPLY: it's not. what if you want two at the same height? let's make it optional FOR NOW UNTIL I ADD IT
    }

    public static List<Sprite> parseSpriteSheetFromName(string filepath, int size, int ppu)
    {
        List<Sprite> sprites;
        byte[] file = Loaders.LoadImage(filepath);
        sprites = SpriteSheetCreator.createSpritesFromSheet(file, size, ppu);
        return sprites;
    }
    /// <summary>
    /// click if you wanna die
    /// </summary>
    public static FurnitureStats parseFurniture(XmlNode x)
    {
        List<(Furniture f, (int min, int max) c)> required = new List<(Furniture f, (int min, int max) c)>();
        List<List<(Furniture f, (int min, int max))>> groups = new List<List<(Furniture f, (int min, int max))>>();

        if (x.HasNode("F"))
            foreach (XmlNode node in x.Qs("F"))
                required.Add(
                    (Furniture.Get(node.InnerText),
                    parseMinMax(x.Q<string>("count", true))));

        if (x.HasNode("Group"))
            foreach (XmlNode node2 in x.Qs("Group"))
                if (node2.HasNode("F"))
                {
                    var lol = new List<(Furniture f, (int min, int max))>();
                    foreach (XmlNode node in x.Qs("F"))
                        lol.Add((Furniture.Get(node.InnerText), parseMinMax(x.Q<string>("count", true))));
                    groups.Add(lol);
                }

        return new FurnitureStats(required, groups);
    }
    public static List<Floor> parseFloors(string s)
    {
        List<Floor> list = new List<Floor>();
        foreach (string ss in s.Split(','))
            list.Add(Floor.Get(ss.removeWS()));
        return list;
    }
    public static List<Door> parseDoors(string s)
    {
        List<Door> list = new List<Door>();
        foreach (string str in s.Split(','))
            list.Add(Door.Get(str));
        return list;
    }
    /*public static List<RoomInfo> parseRooms(XmlNode x)
    {
        List<RoomInfo> roomInfo = new List<RoomInfo>();
        if (x.HasNode("Required"))
            foreach (string s in x.Q<string>("Required").Split(','))
                roomInfo.Add(new RoomInfo(Room.Get(s.removeWS()), true, false));
        if (x.HasNode("Common"))
            foreach (string s in x.Q<string>("Common").Split(','))
                roomInfo.Add(new RoomInfo(Room.Get(s.removeWS()), false, true));

        return roomInfo;
    }*/
    public static List<Vector2Int> parseRoomSizes(XmlNode x)
    {
        List<Vector2Int> list = new List<Vector2Int>();
        if (x.HasNode("Size"))
            foreach (XmlNode a in x.SelectNodes("Size"))
            {
                var ss = a.InnerText.Split(',');
                try
                {
                    if (int.Parse(ss[0].removeWS()) > 12)
                        DB.Attention($"{Loaders.currentFile} | Potential misinput for room size. All values for room size are x16. This may cause issues. i'll fix it in a later update i swear ON GOD fo sho");
                    list.Add(new Vector2Int(int.Parse(ss[0].removeWS()), int.Parse(ss[1].removeWS())));
                    list.Add(new Vector2Int(int.Parse(ss[1].removeWS()), int.Parse(ss[0].removeWS())));
                }
                catch (Exception)
                {
                    DB.Attention($"Couldn't parse room size. Input: \"{a.InnerText}\" : {Loaders.currentFile}");
                }
            }
        return list;
    }
    public static List<Building> parseBuildings(string text)
    {
        List<Building> b = new List<Building>();
        foreach (string s in text.Split(','))
            b.Add(Building.Get(s.removeWS()));
        return b;
    }

    public static (int min, int max) parseMinMax(string x)
    {
        if (string.IsNullOrEmpty(x))
            return (1, 1);

        (int min, int max) lol;

        try
        {
            lol.min = int.Parse(x.Split('-')[0].ToString());
            if (x.Split('-').Length > 1)
                lol.max = int.Parse(x.Split('-')[1].ToString());
            else lol.max = lol.min;
        }
        catch (FormatException) { DB.Attention($"XMLERROR:Non number count. Input:{x} : {Loaders.currentFile}"); return (0, 0); }
        return lol;
    }

    public static string removeWS(this string s)
    {
        return s.TrimStart('\n', ' ', '\r', '\u0009', '\t').TrimEnd('\n', ' ', '\r', '\u0009', '\t');
    }

    public static Color parseColor(string text)
    {
        string[] split = text.Replace(" ", "").Split(',');
        return new Color32(byte.Parse(split[0]), byte.Parse(split[1]), byte.Parse(split[2]), 255);
    }
    public static int parseSkill(int mode, string input) // 0:min, 1:max
    {
        if (mode == 0)
            return int.Parse(input.Split('-')[0].ToString()); // wtf
        else if (mode == 1)
            return int.Parse(input.Split('-')[1].ToString());
        else
            throw new ArgumentException();
    }

    public static string removeUsername(this string s)
    {
        return System.Text.RegularExpressions.Regex.Replace(s, @"([A-Z]{1}\:*\\Users\\)(\w+\\)(.*)", "$1*\\$3");
    }

    public static string AddOrdinal(int num)
    {
        if (num <= 0) return num.ToString();

        switch (num % 100)
        {
            case 11:
            case 12:
            case 13:
                return num + "th";
        }

        return (num % 10) switch
        {
            1 => num + "st",
            2 => num + "nd",
            3 => num + "rd",
            _ => num + "th",
        };
    }
    public static void WriteEl(this XmlWriter writer, string tag, object content)
    {
        if (content == null) return;
        writer.WriteStartElement(tag);
        writer.WriteValue(content.ToString());
        writer.WriteEndElement();
    }
    public static void WriteAttrib(this XmlWriter writer, string name, object content)
    {
        if (content == null) return;
        writer.WriteStartAttribute(name);
        writer.WriteValue(content.ToString());
        writer.WriteEndAttribute();
    }

    public static string toTitleCase(this string t)
    {
        return System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.
        ToTitleCase(t.ToLower());
    }

    public static List<int> AllIndexesOf(this string str, string value)
    {
        if (String.IsNullOrEmpty(value))
            throw new ArgumentException("the string to find may not be empty", "value");
        List<int> indexes = new List<int>();
        for (int index = 0; ; index += value.Length)
        {
            index = str.IndexOf(value, index);
            if (index == -1)
                return indexes;
            indexes.Add(index);
        }
    }
    public static bool strToBool(this string input)
    {
        if (input.Equals("true", StringComparison.CurrentCultureIgnoreCase)
            || input.Equals("yes", StringComparison.CurrentCultureIgnoreCase))
            return true;
        else if (input.Equals("false", StringComparison.CurrentCultureIgnoreCase)
            || input.Equals("no", StringComparison.CurrentCultureIgnoreCase))
            return false;
        else
            return false;
    }

    public static T StringToEnum<T>(this string s)
    {
        return (T)System.Enum.Parse(typeof(T), s, true);
    }

    public static T RandomEnum<T>(System.Random r)
    {
        var v = System.Enum.GetValues(typeof(T));
        return (T)v.GetValue(r.Next(v.Length));
    }

    public static List<T> StripNulls<T>(this List<T> list)
    {
        List<T> tt = new List<T>();
        foreach (T t in list)
            if (t != null)
                tt.Add(t);
        if (tt.Count != list.Count)
            DB.Attention($"Removed {list.Count - tt.Count} nulls");
        return tt;
    }
    public static Vector2Int clampVector(this Vector2Int @in)
    {
        return new Vector2Int(Mathf.Clamp(@in.x, 0, MapGenerator.I.mapWidth-1), Mathf.Clamp(@in.y, 0, MapGenerator.I.mapHeight-1));
    }
    public static Vector3Int clampVector(this Vector3Int @in, int overrideZ = 0)
    {
        return new Vector3Int(Mathf.Clamp(@in.x, 0, MapGenerator.I.mapWidth - 1), Mathf.Clamp(@in.y, 0, MapGenerator.I.mapHeight - 1), overrideZ);
    }
    public static T randomElement<T>(this List<T> list)
    {
        return list[Random.Range(0, list.Count)];
    }
    public static T randomElement<T>(this T[] list)
    {
        return list[Random.Range(0, list.Length)];
    }
    public static T randomElement<T>(this List<T> list, System.Random rand)
    {
        return list[rand.Next(0, list.Count)];
    }

    public static Side NormalizedV2ToSide(this Vector2Int s)
    {
        if(s == Vector2Int.zero) return Side.Center;
        if(s == Vector2Int.down) return Side.Bottom;
        if(s == Vector2Int.up) return Side.Top;
        if(s == Vector2Int.left) return Side.Left;
        if (s == Vector2Int.right) return Side.Right;
        else return Side.Diagonal;
    }

    public static string stripNewlines(this string x)
    {
        return x.Replace("\n", "").Replace("\r", "");
    }

    public static bool HasNode(this XmlNode x, string text)
    {
        return x.SelectSingleNode(text) != null;
    }
    public static T Q<T>(this XmlNode x, string t, bool attribute = false, bool childNodes = false)
    {
        if (!attribute && x.SelectSingleNode(t) == null)
        {
            if (t != "Description" && t != "WeaponClass" && t != "Group" && t != "Long" && t != "Medium" && t != "GenericSpecial" && t != "Sidearms" && t != "PickFrom" && t != "Shields" && t != "IsCarpet" && t != "PrefersFeature" && t != "PartOf")
                DB.Attention($"XMLERROR: No node : {t} : {Loaders.currentFile}");
            return default;
        }

        if (attribute && x.Attributes.GetNamedItem(t) == null)
        {
            if (t != "count")
                DB.Attention($"XMLERROR: No attribute : {t} : {Loaders.currentFile.removeUsername()}");
            return default;
        }

        string innerText = !attribute ? x.SelectSingleNode(t).InnerText : x.Attributes.GetNamedItem(t).InnerText;
        XmlNode innerNode = x.SelectSingleNode(t);

        if (typeof(T) == typeof(string))
            return (T)(object)innerText;
        if (typeof(T) == typeof(bool))
            return (T)(object)innerText.strToBool();
        if (typeof(T) == typeof(Color))
            return (T)(object)parseColor(innerText);
        if (typeof(T) == typeof(float))
            if (float.TryParse(innerText, out float retf))
                return (T)(object)retf;
            else
            {
                DB.Attention($"Not a float, Input:{innerText} : {Loaders.currentFile}"); return default;
            }
        if (typeof(T) == typeof(int))
            if (int.TryParse(innerText, out int retf))
                return (T)(object)retf;
            else
            {
                DB.Attention($"Not an int. Input:{innerText} : {Loaders.currentFile}"); return default;
            }
        var tt = typeof(T);
        if (tt == typeof(List<AnimalArmor>)) return (T)(object)parseAnimalArmor(innerText);
        if (tt == typeof(TerrainFrequencies)) return (T)(object)parseTerrainFrequencies(innerNode);
        if (tt == typeof(List<Weather>)) return (T)(object)parseWeatherFrequencies(innerNode);
        if (tt == typeof(List<Plant>)) return (T)(object)parseFlora(innerText);
        if (tt == typeof(List<Bodypart>)) return (T)(object)parseAllowedBodyparts(innerNode);
        if (tt == typeof(List<Shield>)) return (T)(object)parseShields(innerNode);
        if (tt == typeof(List<List<Armor>>)) return (T)(object)parseArmor(innerNode);
        if (tt == typeof(List<Projectile>)) return (T)(object)parseProjectiles(innerNode);
        if (tt == typeof(List<Weapon>)) return (T)(object)parseWeapons(innerNode);
        if (tt == typeof(XmlNodeList) && childNodes) return (T)(object)innerNode.ChildNodes;
        if (tt == typeof(XmlNode) && !childNodes) return (T)(object)innerNode;
        if (tt == typeof(FurnitureStats)) return (T)(object)parseFurniture(innerNode);
        if (tt == typeof(List<Floor>)) return (T)(object)parseFloors(innerText);
        if (tt == typeof(List<Door>)) return (T)(object)parseDoors(innerText);
        if (tt == typeof(Vector2)) return (T)(object)new Vector2(float.Parse(innerText.Split(',')[0]), float.Parse(innerText.Split(',')[1]));
        throw new ArgumentException("Invalid T parameter. : " + typeof(T).Name);
    }
    public static XmlNodeList Qs(this XmlNode x, string text)
    {
        if (x.SelectSingleNode(text) == null)
        {
            DB.Attention($"XMLERROR: Invalid group. Input:\"{text}\" | {Loaders.currentFile}");
            return null;
        }
        else
            return x.SelectNodes(text);
    }

    public static string Attrib(this XmlNode x, int id)
    {
        if (x.Attributes[id] == null)
        {
            DB.Attention($"XMLERROR: No attribute ID : {id} : {Loaders.currentFile}");
            return "";
        }
        else
            return x.Attributes[id].InnerText;
    }

    public static T Enum<T>(this XmlNode x, string t) where T : struct, IConvertible
    {
        if (!typeof(T).IsEnum)
            throw new ArgumentException("T must be an enum type");
        if (string.IsNullOrEmpty(t) || t == "false")
            return default;

        if (System.Enum.TryParse(t, out T enu))
            return enu;
        else
        {
            DB.Attention($"Couldn't parse {typeof(T)}. Input: {t} : {Loaders.currentFile}");
            return default;
        }
    }
}