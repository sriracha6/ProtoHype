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
                Debug.LogWarning($"{filepath}\n{ex}");
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
        public static Texture2D LoadTexNonWC(string filepath)
        {
            byte[] image = File.ReadAllBytes(filepath);
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
                    meleeWeapons.Add(Weapon.Get(item.ID));
                }

                list.AddRange(meleeWeapons);

                GenericManager.CreateGenericList(xmls.Q<string>("List", true), list,
                    typeof(Weapon));

                return;
            }

            if (xmls.Q<string>("Type").Equals("Melee"))
            {
                List<Attack> attacks = new List<Attack>();

                foreach(XmlNode x in xmls.Q<XmlNodeList>("Attacks", false, childNodes:true))
                {
                    if (x.InnerXml.Equals("Attack"))
                    {
                        attacks.Add(new Attack(x.Attrib(0),
                            x.Enum<DamageType>(x.Q<string>("DamageType")),
                            x.Q<bool>("Rare"),
                            x.Q<int>("Damage")));
                    }
                }
                Weapon.CreateMelee(filepath, xmls.Q<string>("Name"), WeaponType.Melee,
                    xmls.Q<string>("WeaponClass"), 
                    xmls.Q<string>("Description"),
                    MeleeRange.getByName(xmls.Q<string>("MeleeRange")),
                    xmls.Q<bool>("Warmup"),
                    xmls.Q<int>("ArmorPenSharp"),
                    xmls.Q<int>("ArmorPenBlunt"),
                    xmls.Q<float>("Size"),
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
                List<Weapon> items = xmls.Q<List<Weapon>>("List");
                List<Weapon> rangedWeapons = new List<Weapon>();
                foreach(Item item in items)
                {
                    rangedWeapons.Add(Weapon.Get(item.ID)); // so glad i made this
                }
                list.AddRange(rangedWeapons);

                GenericManager.CreateGenericList(xmls.Q<XmlNode>("List").Q<string>("name", attribute: true), list,
                    typeof(Weapon));

                return;
            }

            if (xmls.Q<string>("Type").Equals("Ranged"))
            {
                var aclong = xmls.Q<XmlNode>("Accuracy").Q<XmlNode>("Long");
                var acmed = xmls.Q<XmlNode>("Accuracy").Q<XmlNode>("Med");
                var acshort = xmls.Q<XmlNode>("Accuracy").Q<XmlNode>("Short");

                var ff = xmls.Q<string>("MeleeDamage");

                if(!float.TryParse(ff, out float meleeDamage))
                    meleeDamage = 0;
                Weapon.CreateRanged(filepath, xmls.Q<string>("Name"),
                    xmls.Q<string>("Description"), 
                    WeaponType.Ranged, 
                    xmls.Q<string>("WeaponClass"),
                    xmls.Q<int>("Range"),
                    xmls.Q<float>("ArmorPen"),
                    xmls.Enum<RangeType>(xmls.Q<string>("RangeType")),
                    meleeDamage,
                    xmls.Q<float>("Warmup"),
                    xmls.Q<string>("MeleeDamageType"),
                    xmls.Q<int>("Damage"),
                    xmls.Q<float>("Size"),
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
                list.AddRange(xmls.Q<List<Projectile>>("List"));

                GenericManager.CreateGenericList(xmls.Q<XmlNode>("List").Q<string>("name", attribute: true), list,
                    typeof(Projectile));

                return;
            }
            if (xmls.Q<string>("Type").Equals("Projectile"))
            {
                Projectile.Create(xmls.Q<string>("Name"),
                    xmls.Q<string>("Description"), 
                    filepath,
                    xmls.Q<string>("ForWeapon"),
                    xmls.Q<float>("Damage"),
                    xmls.Enum<DamageType>(xmls.Q<string>("DamageType").toTitleCase()),
                    xmls.Q<float>("AccuracyEffect"),
                    xmls.Q<bool>("Fire"));
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
                list.AddRange(xmls.Q<List<Shield>>("List"));

                GenericManager.CreateGenericList(xmls.Q<XmlNode>("List").Q<string>("name", attribute: true), list,
                    typeof(Shield));

                return;
            }
            
            if (xmls.SelectSingleNode("Type").InnerText.Equals("Shield"))
            {
                Shield.Create(filepath, xmls.Q<string>("Name"),
                    xmls.Q<string>("Description"),
                    xmls.Q<XmlNode>("Protection").Q<float>("Sharp"),
                    xmls.Q<XmlNode>("Protection").Q<float>("Blunt"),
                    xmls.Q<float>("MovementSpeedAffect"),
                    xmls.Q<float>("BaseBlockChance"),
                    xmls.Q<float>("Size"));
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
                list.AddRange(xmls.Q<List<List<Armor>>>("List"));

                GenericManager.CreateGenericList(xmls.Q<XmlNode>("List").Q<string>("name", attribute: true), list,
                    typeof(Armor));

                return;
            }

            if (xmls.Q<string>("Type") == "Armor")
            {
                Armor.Create(filepath, xmls.Q<string>("Name"),
                    xmls.Q<string>("Description"),
                    xmls.Q<int>("Hitpoints"),
                    xmls.Q<XmlNode>("Protection").Q<float>("Sharp"),
                    xmls.Q<XmlNode>("Protection").Q<float>("Blunt"),
                    xmls.Q<float>("MovementSpeedAffect"),
                    xmls.Enum<Layer>(xmls.Q<string>("Layer")),
                    xmls.Q<bool>("Utility"),
                    xmls.Q<List<Bodypart>>("CoversList"));
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

            bool usedByAll = xmls.Q<XmlNode>("Soldier").ParentNode.Q<string>("country", true) == "all";
            
            int currentLoop = 0;
            foreach (XmlElement LOLWHYISTHISHERE in xmls.Qs("Soldier"))
            {
                var t = xmls.Qs("Soldier")[currentLoop];
                bool riding = t.Q<string>("RidingAnimal") == "false";

                var x = TroopType.Create(t.Q<string>("name", attribute:true),
                    t.Q<string>("Description"),
                    filepath,   
                    usedByAll ? null : Country.Get(t.ParentNode.Q<string>("country", attribute:true)),
                    t.Q<List<Weapon>>("Weapons"),
                    t.Q<List<Weapon>>("Sidearms"),
                    t.Q<List<List<Armor>>>("Armor"),
                    t.Q<List<Shield>>("Shields"),
                    ParseFuncs.parseSkill(0, t.Q<XmlNode>("SkillsRange").ChildNodes[0].InnerText),
                    ParseFuncs.parseSkill(1, t.Q<XmlNode>("SkillsRange").ChildNodes[0].InnerText),
                    ParseFuncs.parseSkill(0, t.Q<XmlNode>("SkillsRange").ChildNodes[1].InnerText),
                    ParseFuncs.parseSkill(1, t.Q<XmlNode>("SkillsRange").ChildNodes[1].InnerText),
                    !riding,
                    !riding ? Animal.Get(t.Q<string>("RidingAnimal")) : null,
                    !riding ? ParseFuncs.parseAnimalArmor(t.Q<string>("RidingAnimal")) : null,
                    t.Q<string>("Icon"));

                if(usedByAll)
                    foreach(Country c in Country.List)
                        TroopType.List.Add(new TroopType(x.Name, x.Description, x.SourceFile, c, x.weapons, x.sidearms, x.armor, x.shields, x.meleeSkillMin, x.meleeSkillMax, x.rangeSkillMin, x.rangeSkillMax, x.ridingAnimal, x.riddenAnimal, x.animalArmor, x.Icon));

                currentLoop++;
            }
        }

        public static void LoadCountry(string filepath)
        {
            XmlElement xmls = LoadWC(filepath);
            if (xmls.Q<XmlNode>("MemberName") != null)
            {
                var x = Country.Create(xmls.Q<XmlNode>("MemberName").ParentNode.Q<string>("name", attribute:true),
                    xmls.Q<string>("MemberName"));
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
            if (xmls.Q<XmlNode>("//BodyParts")!=null)
            {
                foreach (XmlNode x in xmls.Q<XmlNode>("//BodyParts").Qs("BodyPart"))
                {
                    Bodypart.Create(x.Q<string>("name", attribute:true),
                        x.Q<int>("HP"),
                        x.Enum<PartType>(x.Q<string>("Type")),
                        x.Q<string>("PartOf")/*BodypartManager.Get(x.SelectSingleNode("PartOf").InnerText)*/,
                        x.Q<float>("PainFactor"),
                        x.Q<float>("BleedingFactor"),
                        x.Q<float>("DamageMultiplier"),
                        x.Q<int>("Count"),
                        x.Enum<VitalSystem>(x.Q<string>("Effects")),
                        x.Enum<EffectAmount>(x.Q<string>("EffectAmount")),
                        x.Enum<HitChance>(x.Q<string>("HitChance")),
                        x.Enum<CountType>(x.Q<string>("CountType")),
                        x.Q<string>("Group"));
                }
                List<Bodypart> list = new List<Bodypart>();
                foreach(Bodypart B in Bodypart.List)
                {
                    list.Add(new Bodypart(B));
                }
                
                foreach (Bodypart b in list)
                {
                    if (b.count > 1)
                    {
                        Bodypart.List.Remove(b);  // so we dont have arm, left arm, right arm
                        //for (int i = 0; i < b.count; i++)
                        //{
                        if (b.countType == CountType.Sides)
                        {
                            Bodypart.Create("Left " + b.Name, b.TotalHP, b.type, b.partOf == null ? "" : b.partOf.Name, b.painFactor, b.bleedingFactor, b.damageMultiplier, b.count, b.effects, b.effectAmount, b.hitChance, b.countType, b.group);
                            Bodypart.Create("Right " + b.Name, b.TotalHP, b.type, b.partOf == null ? "" : b.partOf.Name, b.painFactor, b.bleedingFactor, b.damageMultiplier, b.count, b.effects, b.effectAmount, b.hitChance, b.countType, b.group);
                        }
                        else if (b.countType == CountType.Numbered)
                        {
                            for (int i = 0; i < b.count; i++)
                            {
                                string newname = ParseFuncs.AddOrdinal(i + 1) + " " + b.Name;
                                Bodypart.Create(newname, b.TotalHP, b.type, b.partOf == null ? "" : b.partOf.Name, b.painFactor, b.bleedingFactor, b.damageMultiplier, b.count, b.effects, b.effectAmount, b.hitChance, b.countType, b.group);
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
            if (xmls.Q<string>("Type").Equals("Building"))
            {
                var q = Building.Create(xmls.Q<string>("Name"),
                    xmls.Q<int>("Hitpoints"),
                    xmls.Q<int>("Flammability"),
                    xmls.Q<int>("CoverQuality"),
                    xmls.Q<bool>("Lean"),
                    xmls.Q<bool>("SpecialPlace"),
                    xmls.Q<bool>("Rubble"),
                    xmls.Enum<RubbleType>(xmls.Q<string>("RubbleType")));

                                                                            // always of size 16
                renderedWalls.Add(new RenderedWall(q.ID, SpriteSheetCreator.createSpritesFromSheet(LoadImage(filepath), 512).ToArray()));
                q.tile = SpriteSheetCreator.I.createRuleTile(q);
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
                Furniture.Create(xmls.Q<string>("Name"),
                    xmls.Q<bool>("Tilable"),
                    xmls.Q<int>("Hitpoints"),
                    xmls.Q<bool>("Rubble"),
                    xmls.Enum<RubbleType>(xmls.Q<string>("RubbleType")),
                    xmls.Q<int>("Flammability"),
                    xmls.Q<int>("CoverQuality"),
                    xmls.Q<bool>("Lean"),
                    xmls.Q<bool>("PrefersTouchingWall"),
                    null,
                    xmls.Q<bool>("IsCarpet"));
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
            if (xmls.Q<string>("Type").Equals("Floor"))
            {
                var temp = Floor.Create(xmls.Q<string>("Name"),
                    xmls.Q<int>("Hitpoints"),
                    xmls.Q<int>("Flammability"));

                temp.tile = SpriteSheetCreator.I.createTerrainTileFromSheet(LoadImage(filepath));
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
                Roof.Create(xmls.Q<string>("Name"),
                    xmls.Q<int>("Hitpoints"),
                    xmls.Q<int>("Flammability"),
                    new RoofStats(xmls.Q<XmlNode>("RoofStats").Q<int>("SmallProjectileBlock"),
                        xmls.Q<XmlNode>("RoofStats").Q<int>("LargeProjectileBlock")));
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
            if (xmls.Q<string>("Type").Equals("Nature"))
            {
                Buildings.Plant.Create(xmls.Q<string>("Name"),
                    xmls.Q<int>("Hitpoints"),
                    xmls.Q<int>("Flammability"),
                    xmls.Q<int>("CoverQuality"),
                    xmls.Q<bool>("Lean"),
                    ParseFuncs.parseSpriteSheetFromName(filepath, 512));
            }
            else
            {
                Debug.Log("Not a nature. lol wtf");
                return;
            }
        }
        public static void LoadTrap(string filepath)
        {
            XmlElement xmls = LoadWC(filepath);
            if (xmls.Q<string>("Type").Equals("Trap"))
            {
                Trap.Create(xmls.Q<string>("Name"),
                    xmls.Q<int>("Hitpoints"),
                    xmls.Q<int>("Flammability"),
                    xmls.Q<int>("Damage"),
                    xmls.Q<bool>("SpecialPlace"),
                    xmls.Q<bool>("Rubble"),
                    xmls.Enum<RubbleType>(xmls.Q<string>("RubbleType")),
                    xmls.Q<int>("CoverQuality"),
                    xmls.Q<bool>("Lean"));
            }
            else
            {
                Debug.Log("Not a trap. lol trap??????? like the gay people?");
                return;
            }
        }
        public static void LoadDoor(string filepath)
        {
            XmlElement xmls = LoadWC(filepath);
            if (xmls.Q<string>("Type").Equals("Door"))
            {
                Door.Create(xmls.Q<string>("Name"),
                    xmls.Enum<RubbleType>(xmls.Q<string>("RubbleType")),
                    xmls.Q<int>("Hitpoints"),
                    xmls.Q<int>("Flammability"),
                    xmls.Q<float>("OpeningSpeed"));
            }
            else
            {
                Debug.Log("Not a door.");
                return;
            }
        }

        public static void LoadTerrainType(string filepath)
        {
            XmlNode xmls = LoadWC(filepath);
            if (xmls.Q<string>("Type") == "TerrainType")
            {
                TerrainType temp = 
                TerrainType.Create(xmls.Q<string>("Name"),
                    xmls.Q<float>("Height"),
                    xmls.Q<Color>("Color"),
                    null,
                    xmls.Enum<SpecialType>(xmls.Q<string>("SpecialType")),
                    xmls.Q<bool>("SupportsNature"));

                    temp.tile = SpriteSheetCreator.I.createTerrainTileFromSheet(LoadImage(filepath));
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

            if (xmls.Q<string>("Type")=="Biome")
            {
                Biome.Create(xmls.Q<string>("Name"),
                    xmls.Q<string>("Description"),
                    new LocationData(xmls.Q<XmlNode>("Location").Q<float>("Temperature"),
                        xmls.Q<XmlNode>("Location").Enum<FlatnessPreference>(xmls.Q<XmlNode>("Location").Q<string>("Prefers"))),
                    xmls.Q<List<Weather>>("WeatherFrequencies"),
                    xmls.Q<TerrainFrequencies>("Terrains"),
                    xmls.Q<List<Plant>>("Plants"),
                    xmls.Q<Color>("Color"),
                    xmls.Q<float>("PlantDensity"));
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
            if (xmls.Q<XmlNode>("Ridable") != null)
            {
                var x = Animal.Create(xmls.Q<string>("Name"),
                    xmls.Q<string>("Description"),
                    filepath,
                    xmls.Q<bool>("Ridable"),
                    xmls.Q<float>("SpeedEffect"),
                    xmls.Q<int>("Hitpoints"),
                    xmls.Q<int>("HitChance"));
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
            if (xmls.Q<XmlNode>("ForAnimal") != null)
            {
                AnimalArmor.Create(xmls.Q<string>("Name"),
                    xmls.Q<string>("Description"),
                    filepath,
                    xmls.Q<int>("Protection"),
                    Animal.Get(xmls.Q<string>("ForAnimal")),
                    xmls.Q<float>("MoveSpeedEffect"));
            }
            else
            {
                Debug.Log("Not an animalarmor file.");
                return;
            }
        }
    
        public static void LoadTroopTypeIcon(string filepath)
        {
            renderedTroopTypes.Add(new RenderedTroopType(LoadTexNonWC(filepath), Path.GetFileNameWithoutExtension(filepath)));
        }

        public static void LoadRoom(string filepath)
        {
            XmlElement xmls = LoadXML(filepath);
            if (xmls.HasNode("SurfaceArea"))
            {
                Room.Create(xmls.Q<string>("Name"),
                    xmls.Q<FurnitureStats>("Furniture"),
                    ParseFuncs.parseRoomSizes(xmls.Q<XmlNode>("SurfaceArea")),
                    xmls.Q<List<Floor>>("Floor"));
            }
            else
            {
                Debug.Log("Not a room file.");
                return;
            }
        }
        public static void LoadStructure(string filepath)
        {
            XmlElement xmls = LoadXML(filepath);
            if (xmls.HasNode("InfluenceRange"))
            {
                Structure.Create(xmls.Q<string>("Name"), 
                    xmls.Q<string>("Description"), 
                    xmls.Q<int>("InfluenceRange"),
                    ParseFuncs.parseMinMax(xmls.Q<string>("RoomCount")), 
                    xmls.Q<float>("RoomScale"),
                    ParseFuncs.parseRooms(xmls.Q<XmlNode>("Rooms")), // .
                    ParseFuncs.parseBuildings(xmls.Q<string>("ExteriorWalls")),
                    Building.Get(xmls.Q<string>("InteriorWalls")),
                    xmls.Q<XmlNode>("Entrance").Q<int>("Count"),
                    xmls.Q<XmlNode>("Entrance").Q<List<Door>>("Door"), 
                    xmls.Q<bool>("HasCourtyard"),   
                    xmls.Enum<WorldFeature>(xmls.Q<string>("PrefersFeature")),
                    Room.Get(xmls.Q<string>("CornerRooms")),
                    xmls.Q<List<Door>>("Doors"), 
                    Roof.Get(xmls.Q<string>("Roof")));
            }
            else
            {
                Debug.Log("Not a structure file.");
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

            if(xmls.Q<XmlNode>("GenericSpecial") != null)
                foreach(XmlElement x in xmls.Qs("GenericSpecial"))
                {
                    if (x.InnerText == "Any")
                        weapons.AddRange(Weapon.List);
                    else if (x.InnerText == "None")
                        weapons.Clear();
                }

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

            return weapons;
        }

        public static List<List<Armor>> parseArmor(XmlNode xmls) // this sucks i have to do this for 8 things 8x the code
        {
            if (xmls == null) return null;
            List<List<Armor>> armor = new List<List<Armor>>();
            List<Armor> normalRequired = new List<Armor>();

            if(xmls.Q<XmlNode>("PickFrom") != null)
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
            return shields;
        }
        public static List<Bodypart> parseAllowedBodyparts(XmlNode xmls)
        {
            List<Bodypart> bps = new List<Bodypart>();

            foreach (string s in xmls.LastChild.InnerText.Split(',')) // only this tag's text. not children
            {
                //bps.Add(BodypartManager.Get(s.removeWS().toTitleCase()));
                if (Bodypart.List.Find(x=>x.Name == s.removeWS().toTitleCase()) == null)
                    continue;
                if (string.IsNullOrEmpty(Bodypart.Get(s.removeWS().toTitleCase()).group))
                    bps.Add(Bodypart.Get(s.removeWS().toTitleCase()));
                else
                    bps.AddRange(Bodypart.List.FindAll(x=>x.group == Bodypart.Get(s.removeWS().toTitleCase()).group));
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

            foreach(XmlNode node in x.ChildNodes)
            {
                if(node.NodeType!=XmlNodeType.Comment)
                weatherFrequencies.Add(new Weather(Enum<WeatherType>(null, node.InnerText), 
                    node.Q<float>("frequency", attribute:true)));
            }

            return weatherFrequencies;
        }
        public static TerrainFrequencies parseTerrainFrequencies(XmlNode x)
        {
            List<TerrainType> terrain = new List<TerrainType>();
            List<float> frequencies = new List<float>();

            foreach(XmlElement node in x.ChildNodes)
            {
                terrain.Add(TerrainType.Get(node.InnerText)); // remember this doesnt work yet because i havent created any yet
                float amount;

                if (node.Q<XmlNode>("frequency", attribute:true) == null) amount = 1;

                else amount = node.Q<float>("frequency", attribute:true);

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
        /// <summary>
        /// click if you wanna die
        /// </summary>
        public static FurnitureStats parseFurniture(XmlNode x)
        {
            List<(Furniture f, (int min, int max) c)> required = new List<(Furniture f, (int min, int max) c)>();
            List<List<(Furniture f, (int min, int max))>> groups = new List<List<(Furniture f, (int min, int max))>>();

            if (x.HasNode("F"))
                foreach(XmlNode node in x.Qs("F"))
                    required.Add(
                        (Furniture.Get(node.InnerText), 
                        parseMinMax(x.Q<string>("count", true)) ));

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
            foreach(string ss in s.Split(','))
                list.Add(Floor.Get(ss));
            return list;
        }
        public static List<Door> parseDoors(string s)
        {
            List<Door> list = new List<Door>();
            foreach (string str in s.Split(','))
                list.Add(Door.Get(str));
            return list;
        }
        public static List<RoomInfo> parseRooms(XmlNode x)
        {
            List<RoomInfo> roomInfo = new List<RoomInfo>();
            if (x.HasNode("Required"))
                foreach (string s in x.Q<string>("Required").Split(','))
                    roomInfo.Add(new RoomInfo(Room.Get(s.removeWS()), true, false));
            if (x.HasNode("Common"))
                foreach (string s in x.Q<string>("Common").Split(','))
                    roomInfo.Add(new RoomInfo(Room.Get(s.removeWS()), false, true));

            return roomInfo;
        }
        public static List<Vector2Int> parseRoomSizes(XmlNode x)
        {
            List<Vector2Int> list = new List<Vector2Int>();
            if (x.HasNode("Size"))
                foreach (XmlNode a in x.SelectNodes("Size"))
                {
                    var ss = a.InnerText.Split(',');
                    try
                    {
                        list.Add(new Vector2Int(int.Parse(ss[0]), int.Parse(ss[1])));
                        list.Add(new Vector2Int(int.Parse(ss[1]), int.Parse(ss[0])));
                    }
                    catch(Exception e)
                    {
                        DB.Attention($"Couldn't parse room size. \"{a.InnerText}\"");
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
                return (1,1);

            (int min, int max) lol;

            try
            {
                lol.min = int.Parse(x.Split('-')[0].ToString());
                if (x.Split('-').Length > 1)
                    lol.max = int.Parse(x.Split('-')[1].ToString());
                else lol.max = lol.min;
            }
            catch (FormatException) { DB.Attention("XMLERROR:Non number count"); return (0,0); }
            return lol;
        }

        public static string removeWS(this string s)
        {
            return s.TrimStart('\n', ' ', '\r', '\u0009', '\t').TrimEnd('\n', ' ', '\r', '\u0009', '\t');
        }

        public static Color parseColor(string text)
        {
            string[] split = text.Replace(" ","").Split(',');
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
        
        public static bool HasNode(this XmlNode x, string text)
        {
            return x.SelectSingleNode(text) != null;
        }
        public static T Q<T>(this XmlNode x, string t, bool attribute=false, bool childNodes=false)
        {
            if(!attribute && x.SelectSingleNode(t) == null)
            {
                if(t != "Description" && t != "WeaponClass" && t != "Group" && t != "Long" && t != "Medium" && t != "Short" && t != "GenericSpecial" && t != "Sidearms" && t != "PickFrom" && t != "Shields" && t != "IsCarpet" && t!="PrefersFeature")
                    DB.Attention($"XMLERROR: No node : {t}");
                return default(T);
            }

            if(attribute && x.Attributes.GetNamedItem(t) == null)
            {
                if (t != "count")
                    DB.Attention($"XMLERROR: No attribute : {t}");
                return default(T);
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
                    DB.Attention("Not a float"); return default(T);
                }
            if (typeof(T) == typeof(int))
                if (int.TryParse(innerText, out int retf))
                    return (T)(object)retf;
                else
                {
                    DB.Attention("Not an int"); return default(T);
                }
            var tt = typeof(T);
            if (tt == typeof(List<AnimalArmor>)) return (T)(object)parseAnimalArmor(innerText);
            if (tt == typeof(TerrainFrequencies))  return (T)(object)parseTerrainFrequencies(innerNode);
            if (tt == typeof(List<Weather>))  return (T)(object)parseWeatherFrequencies(innerNode);
            if (tt == typeof(List<Plant>))  return (T)(object)parseFlora(innerText);
            if (tt == typeof(List<Bodypart>))  return (T)(object)parseAllowedBodyparts(innerNode);
            if (tt == typeof(List<Shield>))  return (T)(object)parseShields(innerNode);
            if (tt == typeof(List<List<Armor>>))  return (T)(object)parseArmor(innerNode);
            if (tt == typeof(List<Projectile>))  return (T)(object)parseProjectiles(innerNode);
            if (tt == typeof(List<Weapon>))  return (T)(object)parseWeapons(innerNode);
            if (tt == typeof(XmlNodeList) && childNodes)  return (T)(object)innerNode.ChildNodes;
            if (tt == typeof(XmlNode) && !childNodes)  return (T)(object)innerNode;
            if (tt == typeof(FurnitureStats))  return (T)(object)parseFurniture(innerNode);
            if (tt == typeof(List<Floor>))  return (T)(object)parseFloors(innerText);
            if (tt == typeof(List<Door>)) return (T)(object)parseDoors(innerText);

            throw new ArgumentException("Invalid T parameter. : "+typeof(T).Name);
        }
        public static XmlNodeList Qs(this XmlNode x, string text)
        {
            if (x.SelectSingleNode(text) == null)
            {
                DB.Attention("XMLERROR: Invalid group");
                return null;
            }
            else
                return x.SelectNodes(text);
        }

        public static string Attrib(this XmlNode x, int id)
        {
            if (x.Attributes[id] == null)
            {
                DB.Attention($"XMLERROR: No attribute ID : {id}");
                return "";
            }
            else
                return x.Attributes[id].InnerText;
        }

        public static T Enum<T>(this XmlNode x, string t) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enum type");
            if (string.IsNullOrEmpty(t) || t=="false")
                return default(T);

            if(System.Enum.TryParse(t, out T enu))
                return enu;
            else
            {
                DB.Attention($"Couldn't parse {typeof(T)}. Input: {t}");
                return default(T);
            }    
        }
    }
}
