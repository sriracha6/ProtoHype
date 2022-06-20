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
using static TilemapPlace;
using static CachedItems;
using static WeatherManager;
using System.Globalization;

/// <summary>
/// No matter what is done, this namespace will never look good or make sense to anyone trying to debug it. (literally me. just me. only me, im the only debugger.) Better collapse the { } in your IDE.
/// 
/// FOR GENERIC:
///     make folders with the different types. iterate them. set isGeneric to true
/// </summary>
namespace XMLLoader
{
    public static class Loaders
    {
        public static XmlElement LoadWC(string filepath)
        {
            //try
            //{
                string file = File.ReadAllText(filepath);
                XmlDocument xmlDocument = new XmlDocument();
                string num = file.Substring(0, file.Split('\n')[0].IndexOf('W'));
                int readUntil = int.Parse(num);
                int readStart = num.Length + 2; // already added two in WC file maker
            try
            {
                xmlDocument.LoadXml(file.Substring(readStart, readUntil - readStart));
            }
            catch (Exception ex)
            {
                readUntil++; // some reason
                xmlDocument.LoadXml(file.Substring(readStart, readUntil - readStart));
            }
            return xmlDocument.DocumentElement;
            //}
            //catch (Exception ex)
            //{
            //    Debug.LogError($"Error loading WC : {ex}");
            //    return null;
            //}
        }
        public static XmlElement LoadXML(string filepath)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(filepath);
            return xmlDocument.DocumentElement;
        }
        public static byte[] LoadImage(string filepath) // ALWAYS WC
        {
            if (string.IsNullOrEmpty(filepath))
                return null;
            string line1 = File.ReadAllLines(filepath)[0];
            int startRead = int.Parse(line1.Substring(0,line1.IndexOf('W')));
            byte[] fileRaw = File.ReadAllBytes(filepath);
            byte[] img = fileRaw.Skip(startRead).ToArray();
            return img;
        }
        public static Texture2D LoadTex(string filepath)
        {
            byte[] image = LoadImage(filepath);
            if (image == null) return null;

            Texture2D tex = new Texture2D(0, 0);
            tex.LoadImage(image);
            tex.Apply();
            return tex;
        }
        public static Sprite LoadSprite(string filepath)
        {
            Texture2D tex = LoadTex(filepath);
            if(tex== null) return null;
            float ppu = tex.width;
            return Sprite.Create(tex, new Rect(0,0, tex.width, tex.height), new Vector2(0,0), ppu);
        }
        public static Sprite LoadSprite(Texture2D tex, float ppu)
        {
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0), ppu);
        }
        //-----------------------------------------------------------------//

        public static void LoadMeleeWeapon(string filepath, bool isGeneric = false)
        {
            XmlElement xmls = LoadWC(filepath);
            if (xmls == null) return;

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
                WeaponManager.CreateMelee(filepath, xmls.SelectSingleNode("Name").InnerText, WeaponType.Melee,
                    xmls.SelectSingleNode("WeaponClass") == null ? "" : xmls.SelectSingleNode("WeaponClass").InnerText, 
                    xmls.SelectSingleNode("Description") == null ? "" : xmls.SelectSingleNode("Description").InnerText,
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
            XmlElement xmls = LoadWC(filepath);

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
                var aclong = xmls.SelectSingleNode("Accuracy").SelectSingleNode("Long");
                var acmed = xmls.SelectSingleNode("Accuracy").SelectSingleNode("Med");
                var acshort = xmls.SelectSingleNode("Accuracy").SelectSingleNode("Short");

                var ff = xmls.SelectSingleNode("MeleeDamage").InnerText;

                if(!float.TryParse(ff, out float meleeDamage))
                    meleeDamage = 0;
                WeaponManager.CreateRanged(filepath, xmls.SelectSingleNode("Name").InnerText,
                    xmls.SelectSingleNode("Description") == null ? "" : xmls.SelectSingleNode("Description").InnerText, WeaponType.Ranged, 
                    xmls.SelectSingleNode("WeaponClass") == null ? "" : xmls.SelectSingleNode("WeaponClass").InnerText,
                    int.Parse(xmls.SelectSingleNode("Range").InnerText),
                    float.Parse(xmls.SelectSingleNode("ArmorPen").InnerText),
                    (RangeType)Enum.Parse(typeof(RangeType), xmls.SelectSingleNode("RangeType").InnerText),
                    meleeDamage,
                    float.Parse(xmls.SelectSingleNode("Warmup").InnerText),
                    xmls.SelectSingleNode("MeleeDamageType").InnerText,
                    int.Parse(xmls.SelectSingleNode("Damage").InnerText),
                    float.Parse(xmls.SelectSingleNode("Size").InnerText),
                    aclong == null ? 0 : aclong.InnerText.parseFloat(),  // long
                    acmed == null ? 0 : acmed.InnerText.parseFloat(),  // med
                    acshort == null ? 0 : acshort.InnerText.parseFloat()   // short
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
            XmlElement xmls = LoadWC(filepath);

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
                    xmls.SelectSingleNode("Description")==null ? "" : xmls.SelectSingleNode("Description").InnerText, 
                    filepath,
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
            XmlElement xmls = LoadWC(filepath);
            if (xmls == null) return;

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
                ShieldManager.Create(filepath, xmls.SelectSingleNode("Name").InnerText,
                    xmls.SelectSingleNode("Description") == null ? "" : xmls.SelectSingleNode("Description").InnerText,
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
            XmlElement xmls = LoadWC(filepath);

            if (isGeneric)
            {
                List<List<Armor>> list = new List<List<Armor>>();
                list.AddRange(ParseFuncs.parseArmor(xmls.SelectSingleNode("List")));

                GenericManager.CreateGenericList(xmls.SelectSingleNode("List").Attributes.GetNamedItem("name").InnerText, list,
                    typeof(Armor));

                return;
            }

            if (xmls.SelectSingleNode("Type").InnerText == "Armor")
            {
                ArmorManager.Create(filepath, xmls.SelectSingleNode("Name").InnerText,
                    xmls.SelectSingleNode("Description")==null ? "" : xmls.SelectSingleNode("Description").InnerText,
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
            XmlElement xmls = LoadXML(filepath); // todo: this will be WC when we need archer icons and stuff hopefully

            int currentLoop = 0;
            foreach (XmlElement x in xmls.SelectNodes("Soldier"))
            {
                var t = xmls.SelectNodes("Soldier")[currentLoop];
                bool riding = t.SelectSingleNode("RidingAnimal").InnerText == "false";

                TroopTypeManager.Create(t.Attributes.GetNamedItem("name").InnerText,
                    t.SelectSingleNode("Description") == null ? "" : t.SelectSingleNode("Description").InnerText,
                    filepath,
                    CountryManager.Get(t.ParentNode.Attributes.GetNamedItem("country").Value),
                    ParseFuncs.parseWeapons(t.SelectSingleNode("Weapons")),
                    ParseFuncs.parseWeapons(t.SelectSingleNode("Sidearms")),
                    ParseFuncs.parseArmor(t.SelectSingleNode("Armor")),
                    ParseFuncs.parseShields(t.SelectSingleNode("Shields")),
                    ParseFuncs.parseSkill(0, t.SelectSingleNode("SkillsRange").ChildNodes.Item(0).InnerText),
                    ParseFuncs.parseSkill(1, t.SelectSingleNode("SkillsRange").ChildNodes.Item(0).InnerText),
                    ParseFuncs.parseSkill(0, t.SelectSingleNode("SkillsRange").ChildNodes.Item(1).InnerText),
                    ParseFuncs.parseSkill(1, t.SelectSingleNode("SkillsRange").ChildNodes.Item(1).InnerText),
                    !riding,
                    !riding ? AnimalManager.Get(t.SelectSingleNode("RidingAnimal").InnerText) : null,
                    !riding ? ParseFuncs.parseAnimalArmor(t.SelectSingleNode("RidingAnimal").InnerText) : null);
                currentLoop++;
            }
        }

        public static void LoadCountry(string filepath)
        {
            XmlElement xmls = LoadWC(filepath);
            if (xmls.SelectSingleNode("MemberName") != null)
            {
                var x = CountryManager.Create(xmls.SelectSingleNode("MemberName").ParentNode.Attributes.GetNamedItem("name").Value,
                    xmls.SelectSingleNode("MemberName").InnerText);
                CachedItems.renderedCountries.Add(new RenderedCountry(LoadSprite(filepath), x.Name));
            }
            else
            {
                Debug.Log("Not a country file.");
                return;
            }
        }
        
        public static void LoadBodyparts(string filepath)
        {
            XmlElement xmls = LoadXML(filepath);
            if (xmls.SelectSingleNode("//BodyParts")!=null)
            {
                foreach (XmlNode x in xmls.SelectSingleNode("//BodyParts").SelectNodes("BodyPart"))
                {
                    BodypartManager.Create(x.Attributes.GetNamedItem("name").InnerText,
                        int.Parse(x.SelectSingleNode("HP").InnerText),
                        (PartType)Enum.Parse(typeof(PartType), x.SelectSingleNode("Type").InnerText),
                        x.SelectSingleNode("PartOf") == null ? "" : x.SelectSingleNode("PartOf").InnerText/*BodypartManager.Get(x.SelectSingleNode("PartOf").InnerText)*/,
                        float.Parse(x.SelectSingleNode("PainFactor").InnerText),
                        float.Parse(x.SelectSingleNode("BleedingFactor").InnerText),
                        float.Parse(x.SelectSingleNode("DamageMultiplier").InnerText),
                        int.Parse(x.SelectSingleNode("Count").InnerText),
                        (VitalSystem)Enum.Parse(typeof(VitalSystem), x.SelectSingleNode("Effects").InnerText),
                        (EffectAmount)Enum.Parse(typeof(EffectAmount), x.SelectSingleNode("EffectAmount").InnerText),
                        (HitChance)Enum.Parse(typeof(HitChance), x.SelectSingleNode("HitChance").InnerText),
                        (CountType)Enum.Parse(typeof(CountType), x.SelectSingleNode("CountType").InnerText),
                        x.SelectSingleNode("Group") == null ? "" : x.SelectSingleNode("Group").InnerText);
                }
                List<Bodypart> list = new List<Bodypart>();
                foreach(Bodypart B in BodypartManager.BodypartList)
                {
                    list.Add(new Bodypart(B));
                }
                
                foreach (Bodypart b in list)
                {
                    if (b.count > 1)
                    {
                        BodypartManager.BodypartList.Remove(b);  // so we dont have arm, left arm, right arm
                        //for (int i = 0; i < b.count; i++)
                        //{
                        if (b.countType == CountType.Sides)
                        {
                            BodypartManager.Create("Left " + b.Name, b.TotalHP, b.type, b.partOf == null ? "" : b.partOf.Name, b.painFactor, b.bleedingFactor, b.damageMultiplier, b.count, b.effects, b.effectAmount, b.hitChance, b.countType, b.group);
                            BodypartManager.Create("Right " + b.Name, b.TotalHP, b.type, b.partOf == null ? "" : b.partOf.Name, b.painFactor, b.bleedingFactor, b.damageMultiplier, b.count, b.effects, b.effectAmount, b.hitChance, b.countType, b.group);
                        }
                        else if (b.countType == CountType.Numbered)
                        {
                            for (int i = 0; i < b.count; i++)
                            {
                                string newname = ParseFuncs.AddOrdinal(i + 1) + " " + b.Name;
                                BodypartManager.Create(newname, b.TotalHP, b.type, b.partOf == null ? "" : b.partOf.Name, b.painFactor, b.bleedingFactor, b.damageMultiplier, b.count, b.effects, b.effectAmount, b.hitChance, b.countType, b.group);
                            }
                        }
                        //}
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
            XmlElement xmls = LoadWC(filepath);
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
            XmlElement xmls = LoadWC(filepath);
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
            XmlElement xmls = LoadWC(filepath);
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
            XmlElement xmls = LoadWC(filepath);
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
            XmlElement xmls = LoadXML(filepath);
            if (xmls.SelectSingleNode("Type").InnerText.Equals("Nature"))
            {
                NatureManager.Create(xmls.SelectSingleNode("Name").InnerText,
                    int.Parse(xmls.SelectSingleNode("Hitpoints").InnerText),
                    int.Parse(xmls.SelectSingleNode("Flammability").InnerText),
                    int.Parse(xmls.SelectSingleNode("CoverQuality").InnerText),
                    xmls.SelectSingleNode("Lean").InnerText.strToBool(),
                    ParseFuncs.parseSpriteSheetFromName(filepath, 512));
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
            XmlElement xmls = LoadWC(filepath);
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
            XmlNode xmls = LoadWC(filepath);
            if (xmls.SelectSingleNode("Type").InnerText == "TerrainType")
            {
                TerrainType temp = 
                TerrainTypeManager.Create(xmls.SelectSingleNode("Name").InnerText,
                    float.Parse(xmls.SelectSingleNode("Height").InnerText),
                    ParseFuncs.parseColor(xmls.SelectSingleNode("Color").InnerText),
                    null,
                    (SpecialType)Enum.Parse(typeof(SpecialType), xmls.SelectSingleNode("SpecialType").InnerText),
                    ParseFuncs.strToBool(xmls.SelectSingleNode("SupportsNature").InnerText));

                    SpriteSheetCreator.I.createTerrainTileFromSheet(
                        LoadImage(filepath), ref temp);
            }
            else
            {
                Debug.Log("Not a terraintype.");
                return;
            }
        }
        public static void LoadBiome(string filepath)
        {
            XmlElement xmls = LoadXML(filepath);

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

        public static void LoadAnimal(string filepath)
        {
            XmlElement xmls = LoadWC(filepath);
            if (xmls.SelectSingleNode("Ridable") != null)
            {
                var x = AnimalManager.Create(xmls.SelectSingleNode("Name").InnerText,
                    xmls.SelectSingleNode("Description").InnerText,
                    filepath,
                    xmls.SelectSingleNode("Ridable").InnerText.strToBool(),
                    xmls.SelectSingleNode("SpeedEffect").InnerText.parseFloat(),
                    int.Parse(xmls.SelectSingleNode("Hitpoints").InnerText),
                    int.Parse(xmls.SelectSingleNode("HitChance").InnerText));
                DB.ClassString(x);
            }
            else
            {
                Debug.Log("Not an animal file.");
                return;
            }
        }
        public static void LoadAnimalArmor(string filepath)
        {
            XmlElement xmls = LoadWC(filepath);
            if (xmls.SelectSingleNode("ForAnimal") != null)
            {
                AnimalArmorManager.Create(xmls.SelectSingleNode("Name").InnerText,
                    xmls.SelectSingleNode("Description").InnerText,
                    filepath,
                    int.Parse(xmls.SelectSingleNode("Protection").InnerText),
                    AnimalManager.Get(xmls.SelectSingleNode("ForAnimal").InnerText),
                    xmls.SelectSingleNode("MoveSpeedEffect").InnerText.parseFloat());
            }
            else
            {
                Debug.Log("Not an animalarmor file.");
                return;
            }
        }

        public static void loadBlood()
        {
            string[] files = Directory.GetFiles(Application.persistentDataPath+"\\Textures\\Splatters");
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
            CachedItems.firstNames = File.ReadAllText(Application.persistentDataPath+"\\names.txt").Split('\n').ToList();
            CachedItems.surnames = File.ReadAllText(Application.persistentDataPath + "\\surnames.txt").Split('\n').ToList();
        }
    }

    public static class ParseFuncs
    {
        public static float parseFloat(this string text)
        {
            return Convert.ToSingle(text);
        }
        public static List<Weapon> parseWeapons(XmlNode xmls) // instead of getting by name, i could save the hashcode of the item
        {
            if (xmls == null)
                return null; // uh oh : no sidearms

            List<Weapon> weapons = new List<Weapon>();

            if(xmls.SelectSingleNode("GenericSpecial") != null)
                foreach(XmlElement x in xmls.SelectNodes("GenericSpecial"))
                {
                    if (x.InnerText == "Any")
                        weapons.AddRange(WeaponManager.WeaponList);
                    else if (x.InnerText == "None")
                        weapons.Clear();
                }

            foreach (string s in xmls.LastChild.InnerText.Split(',')) // only this tag's text. not children
                weapons.Add(WeaponManager.Get(s.removeWS()));

            return weapons;
        }
        public static List<Projectile> parseProjectiles(XmlNode xmls)
        {
            List<Projectile> weapons = new List<Projectile>();

            foreach (XmlElement x in xmls.SelectNodes("GenericSpecial"))
            {
                if (x.InnerText == "Any")
                    weapons.AddRange(ProjectileManager.ProjectileList);
                else if (x.InnerText == "None")
                    weapons.Clear();
            }

            foreach (string s in xmls.LastChild.InnerText.Split(',')) // only this tag's text. not children
            {
                weapons.Add(ProjectileManager.Get(s.removeWS()));
            }

            return weapons;
        }

        public static List<List<Armor>> parseArmor(XmlNode xmls) // this sucks i have to do this for 8 things 8x the code
        {
            if (xmls == null) return null;
            List<List<Armor>> armor = new List<List<Armor>>();
            List<Armor> normalRequired = new List<Armor>();

            foreach (XmlElement x in xmls.SelectNodes("PickFrom"))
            {
                List<Armor> tempChoice = new List<Armor>();
                foreach (string s in x.InnerText.Split(','))
                    tempChoice.Add(ArmorManager.Get(s.removeWS()));
                armor.Add(tempChoice);
            }
            foreach (string s in xmls.LastChild.InnerText.Split(',')) // only this tag's text. not children
            {
                normalRequired.Add(ArmorManager.Get(s.removeWS()));
            }
            armor.Add(normalRequired);
            return armor;
        }
        public static List<Shield> parseShields(XmlNode xmls)
        {
            if (xmls == null)
                return null; // no shields
            List<Shield> shields = new List<Shield>();

            if (xmls.SelectSingleNode("GenericSpecial") != null)
                foreach (XmlElement x in xmls.SelectNodes("GenericSpecial"))
                {
                    if (x.InnerText == "Any")
                        shields.AddRange(ShieldManager.ShieldList);
                    else if (x.InnerText == "None")
                        shields.Clear();
                }

            foreach (string s in xmls.LastChild.InnerText.Split(',')) // only this tag's text. not children
            {
                shields.Add(ShieldManager.Get(s.removeWS()));
            }
            return shields;
        }
        public static List<Bodypart> parseAllowedBodyparts(XmlNode xmls)
        {
            List<Bodypart> bps = new List<Bodypart>();

            foreach (string s in xmls.LastChild.InnerText.Split(',')) // only this tag's text. not children
            {
                //bps.Add(BodypartManager.Get(s.removeWS().toTitleCase()));
                if (BodypartManager.Get(s.removeWS().toTitleCase()) == null)
                {
                    DB.Attention("ain't no tellin why THIS SHIT DOESNT WORK");
                    continue;
                }
                if (string.IsNullOrEmpty(BodypartManager.Get(s.removeWS().toTitleCase()).group))
                    bps.Add(BodypartManager.Get(s.removeWS().toTitleCase()));
                else
                    bps.AddRange(BodypartManager.BodypartList.FindAll(x=>x.group == BodypartManager.Get(s.removeWS().toTitleCase()).group));
            }
            return bps;
        }
        public static List<AnimalArmor> parseAnimalArmor(string text)
        {
            List<AnimalArmor> a = new List<AnimalArmor>();
            foreach (string x in text.Split(','))
                a.Add(AnimalArmorManager.Get(x.removeWS()));
            return a;
        }

        public static List<Buildings.Nature> parseFlora(string input)
        {
            List<Buildings.Nature> flora = new List<Buildings.Nature>();

            foreach (string s in input.Split(',')) // only this tag's text. not children
            {
                flora.Add(NatureManager.Get(s.removeWS()));
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
                float amount;

                if (node.Attributes.GetNamedItem("frequency") == null) amount = 1;
                else amount = float.Parse(node.Attributes.GetNamedItem("frequency").InnerText);

                frequencies.Add(amount);
            }
            //                                                                            V bc call is ambiguous
            terrain.Add(new TerrainType("Water", BiomeArea.waterHeight, Color.blue, WCMngr.I.mountainTile, SpecialType.Water, false));
            terrain.Add(new TerrainType("Mountain", BiomeArea.mountainHeight, new Color(256,100,100), WCMngr.I.mountainTile, SpecialType.Mountain, false));                              
                                // todo: mountains have snow? don't just say no im lazy and delete it may make the game beautiful  

            return new TerrainFrequencies(terrain, frequencies); // todo: frequencies are obsolete if height exists. wtf was i thinking
                                                                 // REPLY: it's not. what if you want two at the same height? let's make it optional FOR NOW UNTIL I ADD IT
        }

        public static List<Sprite> parseSpriteSheetFromName(string filepath, int size)
        {
            List<Sprite> sprites = new List<Sprite>();
            byte[] file = Loaders.LoadImage(filepath);
            sprites = SpriteSheetCreator.createSpritesFromSheet(file, size);
            return sprites;
        }

        public static string removeWS(this string s)
        {
            return s.TrimStart('\n', ' ', '\r', '\u0009', '\t').TrimEnd('\n', ' ', '\r', '\u0009', '\t');
        }

        public static Color parseColor(string text)
        {
            string[] split = text.Replace(" ","").Split(',');
            Debug.Log($"{new Color(float.Parse(split[0]), float.Parse(split[1]), float.Parse(split[2]))}");
            return new Color(float.Parse(split[0]),float.Parse(split[1]),float.Parse(split[2]));
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
            if (input.Equals("true",StringComparison.CurrentCultureIgnoreCase)
                || input.Equals("yes", StringComparison.CurrentCultureIgnoreCase))
                return true;
            else if (input.Equals("false", StringComparison.CurrentCultureIgnoreCase)
                || input.Equals("no", StringComparison.CurrentCultureIgnoreCase))
                return false;
            else
                throw new XmlException("Not true or false. : "+input);
        }

        public static T randomElement<T>(this List<T> list)
        {
            return list[Random.Range(0, list.Count)];
        }
        public static T randomElement<T>(this T[] list)
        {
            return list[Random.Range(0, list.Length)];
        }
    }
}
