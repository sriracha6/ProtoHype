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
using Body;
using Buildings;
using Generics;
using UnityEngine.Tilemaps;
using Nature;
using static TilemapPlace;
using static WeatherManager;

/// <summary>
/// No matter what is done, this namespace will never look good or make sense to anyone trying to debug it. (literally me. just me. only me, im the only debugger.) Better collapse the { } in your IDE.
/// 
/// FOR GENERIC:
///     make folders with the different types. iterate them. set isGeneric to true
/// </summary>
namespace XMLLoader
{
    public class Loaders
    {
        public static XmlElement LoadFile(string filepath)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(filepath);
            return xmlDocument.DocumentElement;
        }
        public static byte[] LoadImage(string filepath)
        {
            return File.ReadAllBytes(filepath);
        }

        public static void LoadMeleeWeapon(string filepath, bool isGeneric = false)
        {
            XmlElement xmls = LoadFile(filepath);

            if (isGeneric)
            {
                List<Weapon> list = new List<Weapon>();
                List<Item> items = new List<Item>();
                List<Weapon> meleeWeapons = new List<Weapon>();
                foreach(Item item in items)
                {
                    meleeWeapons.Add(WeaponManager.Get(item.ID));
                }

                list.AddRange(meleeWeapons);

                GenericManager.CreateGenericList(xmls.SelectSingleNode("List").Attributes.GetNamedItem("name").InnerText, list,
                    typeof(Weapon));

                return;
            }

            if (xmls.SelectSingleNode("Type").InnerText.Equals("Melee"))
            {
                List<Attack> attacks = new List<Attack>();

                foreach(XmlNode x in xmls.SelectSingleNode("Attacks").ChildNodes)
                {
                    if (x.InnerXml.Equals("Attack"))
                    {
                        attacks.Add(new Attack(x.Attributes.Item(0).InnerText,
                            (DamageType)Enum.Parse(typeof(DamageType), x.SelectSingleNode("DamageType").InnerText),
                            x.SelectSingleNode("Rare").InnerText.strToBool(),
                            int.Parse(x.SelectSingleNode("Damage").InnerText)));
                    }
                }

                //Debug.Log((string)typeof(MeleeRange).GetField(xmls.SelectSingleNode("MeleeRange").InnerText).GetValue(typeof(MeleeRange)));//.GetValue(typeof(MeleeRange))));
                WeaponManager.CreateMelee(xmls.SelectSingleNode("Name").InnerText, WeaponType.Melee,
                    xmls.SelectSingleNode("WeaponClass").InnerText, xmls.SelectSingleNode("Description").InnerText,
                    MeleeRange.getByName(xmls.SelectSingleNode("MeleeRange").InnerText),
                    xmls.SelectSingleNode("Warmup").InnerText.strToBool(),
                    int.Parse(xmls.SelectSingleNode("ArmorPenSharp").InnerText),
                    int.Parse(xmls.SelectSingleNode("ArmorPenBlunt").InnerText),
                    float.Parse(xmls.SelectSingleNode("Size").InnerText),
                    attacks);
            }
            else
            {
                Debug.Log("Not melee weapon.");
                return;
            }

        }
        public static void LoadRangedWeapon(string filepath, bool isGeneric = false)
        {
            XmlElement xmls = LoadFile(filepath);

            if (isGeneric)
            {
                List<Weapon> list = new List<Weapon>();
                List<Weapon> items = ParseFuncs.parseWeapons(xmls.SelectSingleNode("List"));
                List<Weapon> rangedWeapons = new List<Weapon>();
                foreach(Item item in items)
                {
                    rangedWeapons.Add(WeaponManager.Get(item.ID)); // so glad i made this
                }
                list.AddRange(rangedWeapons);

                GenericManager.CreateGenericList(xmls.SelectSingleNode("List").Attributes.GetNamedItem("name").InnerText, list,
                    typeof(Weapon));

                return;
            }

            if (xmls.SelectSingleNode("Type").InnerText.Equals("Ranged"))
            {
                WeaponManager.CreateRanged(xmls.SelectSingleNode("Name").InnerText,
                    xmls.SelectSingleNode("Description").InnerText, WeaponType.Ranged, 
                    xmls.SelectSingleNode("WeaponClass").InnerText,
                    int.Parse(xmls.SelectSingleNode("Range").InnerText),
                    float.Parse(xmls.SelectSingleNode("ArmorPen").InnerText),
                    (RangeType)Enum.Parse(typeof(RangeType), xmls.SelectSingleNode("RangeType").InnerText),
                    float.Parse(xmls.SelectSingleNode("MeleeDamage").InnerText),
                    float.Parse(xmls.SelectSingleNode("Warmup").InnerText),
                    xmls.SelectSingleNode("MeleeDamageType").InnerText,
                    int.Parse(xmls.SelectSingleNode("Damage").InnerText),
                    float.Parse(xmls.SelectSingleNode("Size").InnerText),
                    float.Parse(xmls.SelectSingleNode("Accuracy").SelectNodes("Long")[0].InnerText),  // long
                    float.Parse(xmls.SelectSingleNode("Accuracy").SelectNodes("Med")[0].InnerText),  // med
                    float.Parse(xmls.SelectSingleNode("Accuracy").SelectNodes("Short")[0].InnerText)   // short
                    );
            }
            else
            {
                Debug.Log("Not range weapon.");
                return;
            }

        }
        public static void LoadProjectile(string filepath, bool isGeneric = false)
        {
            XmlElement xmls = LoadFile(filepath);

            if (isGeneric)
            {
                List<Projectile> list = new List<Projectile>();
                list.AddRange(ParseFuncs.parseProjectiles(xmls.SelectSingleNode("List")));

                GenericManager.CreateGenericList(xmls.SelectSingleNode("List").Attributes.GetNamedItem("name").InnerText, list,
                    typeof(Projectile));

                return;
            }

            if (xmls.SelectSingleNode("Type").InnerText.Equals("Projectile"))
            {
                ProjectileManager.Create(xmls.SelectSingleNode("Name").InnerText,
                    xmls.SelectSingleNode("Description").InnerText,
                    xmls.SelectSingleNode("ForWeapon").InnerText,
                    float.Parse(xmls.SelectSingleNode("Damage").InnerText),
                    (DamageType)Enum.Parse(typeof(DamageType),ParseFuncs.toTitleCase(xmls.SelectSingleNode("DamageType").InnerText)),
                    float.Parse(xmls.SelectSingleNode("AccuracyEffect").InnerText),
                    xmls.SelectSingleNode("Fire").InnerText.strToBool());
            }
            else
            {
                Debug.Log("Not a projectile.");
                return;
            }
        }

        public static void LoadShield(string filepath, bool isGeneric = false)
        {
            XmlElement xmls = LoadFile(filepath);

            if (isGeneric)
            {
                List<Shield> list = new List<Shield>();
                list.AddRange(ParseFuncs.parseShields(xmls.SelectSingleNode("List")));

                GenericManager.CreateGenericList(xmls.SelectSingleNode("List").Attributes.GetNamedItem("name").InnerText, list,
                    typeof(Shield));

                return;
            }

            if (xmls.SelectSingleNode("Type").InnerText.Equals("Shield"))
            {
                ShieldManager.Create(xmls.SelectSingleNode("Name").InnerText,
                    xmls.SelectSingleNode("Description").InnerText,
                    float.Parse(xmls.SelectSingleNode("Protection").SelectNodes("Sharp")[0].InnerText),
                    float.Parse(xmls.SelectSingleNode("Protection").SelectNodes("Blunt")[0].InnerText),
                    float.Parse(xmls.SelectSingleNode("MovementSpeedAffect").InnerText),
                    float.Parse(xmls.SelectSingleNode("BaseBlockChance").InnerText),
                    float.Parse(xmls.SelectSingleNode("Size").InnerText));
            }
            else
            {
                Debug.Log("Not a shield.");
                return;
            }
        }
        public static void LoadArmor(string filepath, bool isGeneric = false)
        {
            XmlElement xmls = LoadFile(filepath);

            if (isGeneric)
            {
                List<Armor> list = new List<Armor>();
                list.AddRange(ParseFuncs.parseArmor(xmls.SelectSingleNode("List")));

                GenericManager.CreateGenericList(xmls.SelectSingleNode("List").Attributes.GetNamedItem("name").InnerText, list,
                    typeof(Armor));

                return;
            }

            if (xmls.SelectSingleNode("Type").InnerText.Equals("Armor"))
            {
                ArmorManager.Create(xmls.SelectSingleNode("Name").InnerText,
                    xmls.SelectSingleNode("Description").InnerText,
                    int.Parse(xmls.SelectSingleNode("Hitpoints").InnerText),
                    float.Parse(xmls.SelectSingleNode("Protection").SelectNodes("Sharp")[0].InnerText),
                    float.Parse(xmls.SelectSingleNode("Protection").SelectNodes("Blunt")[0].InnerText),
                    float.Parse(xmls.SelectSingleNode("MovementSpeedAffect").InnerText),
                    (Layer)Enum.Parse(typeof(Layer), xmls.SelectSingleNode("Layer").InnerText),
                    xmls.SelectSingleNode("Utility").InnerText.strToBool(),
                    ParseFuncs.parseAllowedBodyparts(xmls.SelectSingleNode("CoversList")));
            }
            else
            {
                Debug.Log("Not armor.");
                return;
            }
        }

        public static void LoadCountryOutfit(string filepath)
        {
            XmlElement xmls = LoadFile(filepath);

            int currentLoop = 0;
            foreach (XmlElement x in xmls.SelectNodes("Soldier"))
            {
                TroopTypeManager.Create(xmls.ParentNode.Attributes.GetNamedItem("name").InnerText,
                    CountryManager.Get(xmls.SelectSingleNode("CountryOrigin").InnerText),
                    ParseFuncs.parseWeapons(xmls.SelectNodes("Soldier")[currentLoop].SelectSingleNode("Weapons")),
                    ParseFuncs.parseWeapons(xmls.SelectNodes("Soldier")[currentLoop].SelectSingleNode("Sidearms")),
                    ParseFuncs.parseArmor(xmls.SelectNodes("Soldier")[currentLoop].SelectSingleNode("Armor")),
                    ParseFuncs.parseShields(xmls.SelectNodes("Soldier")[currentLoop].SelectSingleNode("Shields")),
                    ParseFuncs.parseSkill(0, xmls.SelectNodes("Soldier")[currentLoop].SelectSingleNode("SkillsRange").ChildNodes.Item(0).InnerText),
                    ParseFuncs.parseSkill(1, xmls.SelectNodes("Soldier")[currentLoop].SelectSingleNode("SkillsRange").ChildNodes.Item(0).InnerText),
                    ParseFuncs.parseSkill(0, xmls.SelectNodes("Soldier")[currentLoop].SelectSingleNode("SkillsRange").ChildNodes.Item(1).InnerText),
                    ParseFuncs.parseSkill(1, xmls.SelectNodes("Soldier")[currentLoop].SelectSingleNode("SkillsRange").ChildNodes.Item(1).InnerText));
                currentLoop++;
            }
        }

        public static void LoadCountries(string filepath)
        {
            XmlElement xmls = LoadFile(filepath);
            if (xmls.SelectSingleNode("Countries")!=null)
            {
                foreach (XmlElement x in xmls.SelectSingleNode("Countries").ChildNodes)
                {
                    CountryManager.Create(x.Attributes.GetNamedItem("name").Value,
                        x.SelectSingleNode("MemberName").InnerText);
                }
            }
            else
            {
                Debug.Log("Not a countries file.");
                return;
            }
        }
        
        public static void LoadBodyparts(string filepath)
        {
            XmlElement xmls = LoadFile(filepath);
            if (xmls.SelectSingleNode("//BodyParts")!=null)
            {
                foreach (XmlNode x in xmls.SelectSingleNode("//BodyParts").SelectNodes("BodyPart"))
                {
                    BodypartManager.Create(x.Attributes.GetNamedItem("name").InnerText,
                        int.Parse(x.SelectSingleNode("HP").InnerText),
                        (PartType)Enum.Parse(typeof(PartType), x.SelectSingleNode("Type").InnerText),
                        BodypartManager.Get(x.SelectSingleNode("PartOf").InnerText),
                        float.Parse(x.SelectSingleNode("PainFactor").InnerText),
                        float.Parse(x.SelectSingleNode("BleedingFactor").InnerText),
                        float.Parse(x.SelectSingleNode("DamageMultiplier").InnerText),
                        int.Parse(x.SelectSingleNode("Count").InnerText),
                        (VitalSystem)Enum.Parse(typeof(VitalSystem), x.SelectSingleNode("Effects").InnerText),
                        (EffectAmount)Enum.Parse(typeof(EffectAmount), x.SelectSingleNode("EffectAmount").InnerText),
                        (HitChance)Enum.Parse(typeof(HitChance), x.SelectSingleNode("HitChance").InnerText),
                        (CountType)Enum.Parse(typeof(CountType), x.SelectSingleNode("CountType").InnerText));
                }
                foreach (Bodypart b in new List<Bodypart>(BodypartManager.BodypartList)) // fuck you i have to make a new temp list
                {
                    if (b.count > 1)
                    {
                        for (int i = 0; i < b.count; i++)
                        {
                            if (b.countType == CountType.Sides)
                            {
                                string newname = i == 0 ? "Left " + b.Name : "Right " + b.Name;
                                BodypartManager.Create(newname, b.TotalHP, b.type, b.partOf, b.painFactor, b.bleedingFactor, b.damageMultiplier, b.count, b.effects, b.effectAmount, b.hitChance, b.countType);
                            }
                            else if (b.countType == CountType.Numbered)
                            {
                                string newname = ParseFuncs.AddOrdinal(i) + " "+b.Name;
                                BodypartManager.Create(newname, b.TotalHP, b.type, b.partOf, b.painFactor, b.bleedingFactor, b.damageMultiplier, b.count, b.effects, b.effectAmount, b.hitChance, b.countType);
                            }
                        }
                    }
                }
            }
            else
            {
                Debug.Log("Not a bodyparts list.");
                return;
            }
        }

        public static void LoadBuilding(string filepath)
        {
            XmlElement xmls = LoadFile(filepath);
            if (xmls.SelectSingleNode("Type").InnerText.Equals("Building"))
            {
                BuildingManager.Create(xmls.SelectSingleNode("Name").InnerText,
                    int.Parse(xmls.SelectSingleNode("Hitpoints").InnerText),
                    int.Parse(xmls.SelectSingleNode("Flammability").InnerText),
                    int.Parse(xmls.SelectSingleNode("CoverQuality").InnerText),
                    xmls.SelectSingleNode("Lean").InnerText.strToBool(),
                    xmls.SelectSingleNode("SpecialPlace").InnerText.strToBool(),
                    xmls.SelectSingleNode("Rubble").InnerText.strToBool(),
                    (RubbleType)Enum.Parse(typeof(RubbleType), xmls.SelectSingleNode("RubbleType").InnerText));
            }
            else
            {
                Debug.Log("Not a building.");
                return;
            }
        }
        public static void LoadFurniture(string filepath)
        {
            XmlElement xmls = LoadFile(filepath);
            if (xmls.SelectSingleNode("Type").InnerText.Equals("Prop"))
            {
                FurnitureManager.Create(xmls.SelectSingleNode("Name").InnerText,
                    xmls.SelectSingleNode("Tilable").InnerText.strToBool(),
                    int.Parse(xmls.SelectSingleNode("Hitpoints").InnerText),
                    xmls.SelectSingleNode("Rubble").InnerText.strToBool(),
                    (RubbleType)Enum.Parse(typeof(RubbleType), xmls.SelectSingleNode("RubbleType").InnerText),
                    int.Parse(xmls.SelectSingleNode("Flammability").InnerText),
                    int.Parse(xmls.SelectSingleNode("CoverQuality").InnerText),
                    xmls.SelectSingleNode("Lean").InnerText.strToBool());
            }
            else
            {
                Debug.Log("Not a furniture.");
                return;
            }
        }
        public static void LoadFloor(string filepath)
        {
            XmlElement xmls = LoadFile(filepath);
            if (xmls.SelectSingleNode("Type").InnerText.Equals("Floor"))
            {
                FloorManager.Create(xmls.SelectSingleNode("Name").InnerText,
                    int.Parse(xmls.SelectSingleNode("Hitpoints").InnerText),
                    int.Parse(xmls.SelectSingleNode("Flammability").InnerText));
            }
            else
            {
                Debug.Log("Not a floor.");
                return;
            }
        }
        public static void LoadRoof(string filepath)
        {
            XmlElement xmls = LoadFile(filepath);
            if (xmls.SelectSingleNode("Type").InnerText.Equals("Roof"))
            {
                RoofManager.Create(xmls.SelectSingleNode("Name").InnerText,
                    int.Parse(xmls.SelectSingleNode("Hitpoints").InnerText),
                    int.Parse(xmls.SelectSingleNode("Flammability").InnerText),
                    new RoofStats(int.Parse(xmls.SelectSingleNode("RoofStats").SelectSingleNode("SmallProjectileBlock").InnerText),
                    int.Parse(xmls.SelectSingleNode("RoofStats").SelectSingleNode("LargeProjectileBlock").InnerText)));
            }
            else
            {
                Debug.Log("Not a roof.");
                return;
            }
        }
        public static void LoadNature(string filepath)
        {
            XmlElement xmls = LoadFile(filepath);
            if (xmls.SelectSingleNode("Type").InnerText.Equals("Nature"))
            {
                NatureManager.Create(xmls.SelectSingleNode("Name").InnerText,
                    int.Parse(xmls.SelectSingleNode("Hitpoints").InnerText),
                    int.Parse(xmls.SelectSingleNode("Flammability").InnerText),
                    int.Parse(xmls.SelectSingleNode("CoverQuality").InnerText),
                    xmls.SelectSingleNode("Lean").InnerText.strToBool(),
                    ParseFuncs.parseSpriteSheetFromName(xmls.SelectSingleNode("Name").InnerText, "Nature"));
            }
            else
            {
                Debug.Log("Not a nature. lol wtf");
                Debug.Log(":: " + xmls.SelectSingleNode("Type").InnerText);
                return;
            }
        }
        public static void LoadTrap(string filepath)
        {
            XmlElement xmls = LoadFile(filepath);
            if (xmls.SelectSingleNode("Type").InnerText.Equals("Trap"))
            {
                TrapManager.Create(xmls.SelectSingleNode("Name").InnerText,
                    int.Parse(xmls.SelectSingleNode("Hitpoints").InnerText),
                    int.Parse(xmls.SelectSingleNode("Flammability").InnerText),
                    int.Parse(xmls.SelectSingleNode("Damage").InnerText),
                    xmls.SelectSingleNode("SpecialPlace").InnerText.strToBool(),
                    xmls.SelectSingleNode("Rubble").InnerText.strToBool(),
                    (RubbleType)Enum.Parse(typeof(RubbleType), xmls.SelectSingleNode("RubbleType").InnerText),
                    int.Parse(xmls.SelectSingleNode("CoverQuality").InnerText),
                    xmls.SelectSingleNode("Lean").InnerText.strToBool());
            }
            else
            {
                Debug.Log("Not a trap. lol trap??????? like the gay people?");
                return;
            }
        }

        public static void LoadTerrainType(string filepath)
        {
            XmlElement xmls = LoadFile(filepath);
            if (xmls.SelectSingleNode("Type").InnerText == "TerrainType")
            {
                TerrainType temp = 
                TerrainTypeManager.Create(xmls.SelectSingleNode("Name").InnerText,
                    float.Parse(xmls.SelectSingleNode("Height").InnerText),
                    ParseFuncs.parseColor(xmls.SelectSingleNode("Color").InnerText),
                    null,
                    (SpecialType)Enum.Parse(typeof(SpecialType), xmls.SelectSingleNode("SpecialType").InnerText),
                    ParseFuncs.strToBool(xmls.SelectSingleNode("SupportsNature").InnerText));
                    SpriteSheetCreator.Instance.createTerrainTileFromSheet(LoadImage(ParseFuncs.nameToImagePath(temp.name, "TerrainType")), ref temp);
            }
            else
            {
                Debug.Log("Not a terraintype.");
                return;
            }
        }
        public static void LoadBiome(string filepath)
        {
            XmlElement xmls = LoadFile(filepath);

            if (xmls.SelectSingleNode("Type").InnerText=="Biome")
            {
                BiomeManager.Create(xmls.SelectSingleNode("Name").InnerText,
                    xmls.SelectSingleNode("Description").InnerText,
                    new LocationData(float.Parse(xmls.SelectSingleNode("Location").SelectSingleNode("Temperature").InnerText),
                    (FlatnessPreference)Enum.Parse(typeof(FlatnessPreference), xmls.SelectSingleNode("Location").SelectSingleNode("Prefers").InnerText)),
                    ParseFuncs.parseWeatherFrequencies(xmls.SelectSingleNode("WeatherFrequencies")),
                    ParseFuncs.parseTerrainFrequencies(xmls.SelectSingleNode("Terrains")),
                    ParseFuncs.parseFlora(xmls.SelectSingleNode("Plants").InnerText),
                    ParseFuncs.parseColor(xmls.SelectSingleNode("Color").InnerText),
                    float.Parse(xmls.SelectSingleNode("PlantDensity").InnerText));
            }
            else
            {
                Debug.Log("Not a biome.");
                return;
            }
        }

        // TODO:::::: ACTUAL PATH!!!!!!!
        public static void loadBlood()
        {
            string[] files = Directory.GetFiles("C:\\Users\\frenz\\Music\\assets\\textures\\splatters");
            foreach (string file in files)
            {
                var rawData = System.IO.File.ReadAllBytes(file);
                Texture2D tex = new Texture2D(512, 512);
                tex.LoadImage(rawData);
                tex.Apply();

                Sprite spr = Sprite.Create(tex, new Rect(Vector2.zero
                , new Vector2(tex.width, tex.height)), Vector2.zero, 256);
                CachedItems.bloodSplatters.Add(spr);
            }
        }
        public static void loadNames()
        {
            CachedItems.firstNames = File.ReadAllText("C:\\Users\\frenz\\Music\\assets\\names.txt").Split('\n').ToList();
            CachedItems.surnames = File.ReadAllText("C:\\Users\\frenz\\Music\\assets\\surnames.txt").Split('\n').ToList();
        }
    }

    public static class ParseFuncs
    {
        public static List<Weapon> parseWeapons(XmlNode xmls) // instead of getting by name, i could save the hashcode of the item
        {
            List<Weapon> weapons = new List<Weapon>();

            foreach(XmlElement x in xmls.SelectNodes("GenericSpecial"))
            {
                if (x.InnerText == "Any")
                {
                    weapons.Add(WeaponManager.WeaponList[Random.Range(0, WeaponManager.WeaponList.Count)]);
                }
                else if (x.InnerText == "None")
                    weapons.Add(null); //todo: this does nothing yet
            }

            foreach (string s in xmls.LastChild.InnerText.Split(',')) // only this tag's text. not children
            {
                weapons.Add(WeaponManager.Get(s));
            }

            return weapons;
        }
        public static List<Projectile> parseProjectiles(XmlNode xmls)
        {
            List<Projectile> weapons = new List<Projectile>();

            foreach (XmlElement x in xmls.SelectNodes("GenericSpecial"))
            {
                if (x.InnerText == "Any")
                {
                    weapons.Add(ProjectileManager.ProjectileList[Random.Range(0, ProjectileManager.ProjectileList.Count)]);
                }
                else if (x.InnerText == "None")
                {
                    weapons.Add(null); //todo: this does nothing yet
                }
            }

            foreach (string s in xmls.LastChild.InnerText.Split(',')) // only this tag's text. not children
            {
                weapons.Add(ProjectileManager.Get(s));
            }

            return weapons;
        }

        public static List<Armor> parseArmor(XmlNode xmls)
        {
            List<Armor> armor = new List<Armor>();

            foreach (XmlElement x in xmls.SelectNodes("PickFrom"))
            {
                List<Armor> tempChoice = new List<Armor>();
                foreach (string s in x.InnerText.Split(','))
                {
                    tempChoice.Add(ArmorManager.Get(s));
                }
                int index = tempChoice.Count>1 ? Random.Range(0, tempChoice.Count) : 0;
                armor.Add(tempChoice[index]);
            }
            foreach (string s in xmls.LastChild.InnerText.Split(',')) // only this tag's text. not children
            {
                armor.Add(ArmorManager.Get(s));
            }
            return armor;
        }
        public static List<Shield> parseShields(XmlNode xmls)
        {
            List<Shield> shields = new List<Shield>();

            foreach (XmlElement x in xmls.SelectNodes("PickFrom"))
            {
                List<Shield> tempChoice = new List<Shield>();
                foreach (string s in x.InnerText.Split(','))
                {
                    tempChoice.Add(ShieldManager.Get(s));
                }
                int index = tempChoice.Count > 1 ? Random.Range(0, tempChoice.Count) : 0;
                shields.Add(tempChoice[index]);
            }
            foreach (string s in xmls.LastChild.InnerText.Split(',')) // only this tag's text. not children
            {
                shields.Add(ShieldManager.Get(s));
            }
            return shields;
        }
        public static List<Bodypart> parseAllowedBodyparts(XmlNode xmls)
        {
            List<Bodypart> bps = new List<Bodypart>();

            foreach (string s in xmls.LastChild.InnerText.Split(',')) // only this tag's text. not children
            {
                bps.Add(BodypartManager.Get(s.toTitleCase()));
            }
            return bps;
        }
        
        public static List<Buildings.Nature> parseFlora(string input)
        {
            List<Buildings.Nature> flora = new List<Buildings.Nature>();

            foreach (string s in input.Split(',')) // only this tag's text. not children
            {
                flora.Add(NatureManager.Get(s));
            }

            return flora;
        }
        public static List<Weather> parseWeatherFrequencies(XmlNode x)
        {
            List<Weather> weatherFrequencies = new List<Weather>();

            foreach(XmlNode node in x.ChildNodes)
            {
                if(node.NodeType!=XmlNodeType.Comment)
                weatherFrequencies.Add(new Weather((WeatherType)Enum.Parse(typeof(WeatherType),node.InnerText), 
                    float.Parse(node.Attributes.GetNamedItem("frequency").InnerText)));
            }

            return weatherFrequencies;
        }
        public static TerrainFrequencies parseTerrainFrequencies(XmlNode x)
        {
            List<TerrainType> terrain = new List<TerrainType>();
            List<float> frequencies = new List<float>();

            foreach(XmlElement node in x.ChildNodes)
            {
                terrain.Add(TerrainTypeManager.Get(node.InnerText)); // remember this doesnt work yet because i havent created any yet
                frequencies.Add(float.Parse(node.Attributes.GetNamedItem("frequency").InnerText));
            }
            //                                                              V bc call is ambiguous
            terrain.Add(new TerrainType("Water", BiomeArea.waterHeight, Color.blue, Loader.loader.mountainTile, SpecialType.Water, false));
            terrain.Add(new TerrainType("Mountain", 0.9f, new Color(256,100,100), Loader.loader.mountainTile, SpecialType.Mountain, false));

            return new TerrainFrequencies(terrain, frequencies); // todo: frequencies are obsolete if height exists. wtf was i thinking
        }

        public static List<Sprite> parseSpriteSheetFromName(string name, string type)
        {
            string path = ParseFuncs.nameToImagePath(name, type);
            List<Sprite> sprites = new List<Sprite>();
            byte[] file = Loaders.LoadImage(path);
            sprites = SpriteSheetCreator.createSpritesFromSheet(file);
            return sprites;
        }

        public static Color parseColor(string text)
        {
            string[] split = text.Replace(" ","").Split(',');
            return new Color(float.Parse(split[0]),float.Parse(split[1]),float.Parse(split[2]));
        }
        public static int parseSkill(int mode, string input) // 0:min, 1:max
        {
            if (mode == 0)
            {
                return int.Parse(input.Split('-')[0].ToString()); // wtf
            }
            else if (mode == 1)
            {
                return int.Parse(input.Split('-').ToString());
            }
            else
            {
                throw new ArgumentException();
            }
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

            switch (num % 10)
            {
                case 1:
                    return num + "st";
                case 2:
                    return num + "nd";
                case 3:
                    return num + "rd";
                default:
                    return num + "th";
            }
        }
        public static string toTitleCase(this string t)
        {
            return System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.
            ToTitleCase(t.ToLower());
        }

        public static string nameToImagePath(string name, string type)
        {
            return Application.persistentDataPath + "/Textures/" + type +"/"+ name + ".png";
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
            if (input.Equals("true",StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }
            else if (input.Equals("false", StringComparison.CurrentCultureIgnoreCase))
            {
                return false;
            }
            else
            {
                throw new XmlException("Not true or false.");
                // todo
            }
        }
    }
}
