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
    /// <summary>
    /// Reserved names: None, false (case-insensitive)
    /// </summary>
    public static class Loaders
    {
        public static string currentFile;
        public static XmlElement LoadWC(string filepath)
        {
            //try
            //{
                currentFile = filepath;
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
            currentFile = filepath;
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(filepath);
            return xmlDocument.DocumentElement;
        }
        public static byte[] LoadImage(string filepath) // ALWAYS WC
        {
            currentFile = filepath;
            if (string.IsNullOrEmpty(filepath))
                return null;
            string line1 = File.ReadAllLines(filepath)[0];
            if (!line1.Contains("WC"))
            {
                DB.Attention("Attempted to load non-WC file image.");
                return null;
            }
            int startRead = int.Parse(line1.Substring(0,line1.IndexOf('W')));
            byte[] fileRaw = File.ReadAllBytes(filepath);
            byte[] img = fileRaw.Skip(startRead).ToArray();
            return img;
        }
        public static byte[] LoadBytes(string filepath)
        {
            return File.ReadAllBytes(filepath);
        }
        public static Texture2D LoadTex(string filepath)
        {
            currentFile = filepath;
            byte[] image = LoadImage(filepath);
            if (image == null) return null;

            Texture2D tex = new Texture2D(0, 0);
            tex.LoadImage(image);
            tex.Apply();
            return tex;
        }
        public static Texture2D LoadTexNonWC(string filepath)
        {
            currentFile = filepath;
            byte[] image = File.ReadAllBytes(filepath);
            if (image == null) return null;

            Texture2D tex = new Texture2D(0, 0);
            tex.LoadImage(image);
            tex.Apply();
            return tex;
        }
        public static Sprite LoadSprite(string filepath, float ppu, bool bottomRightPivot=false)
        {
            currentFile = filepath;
            Texture2D tex = LoadTex(filepath);
            if(tex== null) return null;
            Sprite spr = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), bottomRightPivot ? new Vector2(1, 0) : new Vector2(0, 0), ppu);
            return spr;
        }
        public static Sprite LoadSprite(Texture2D tex, float ppu)
        {
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0), ppu);
        }
        //-----------------------------------------------------------------//

        public static void LoadMeleeWeapon(string filepath)
        {
            XmlElement xmls = LoadWC(filepath);

            if (xmls.Q<string>("Type").Equals("Melee"))
            {
                List<Attack> attacks = new List<Attack>();

                foreach(XmlNode x in xmls.Q<XmlNodeList>("Attacks", false, childNodes:true))
                {
                    attacks.Add(new Attack(x.Attrib(0),
                        x.Enum<DamageType>(x.Q<string>("DamageType")),
                        x.Q<bool>("Rare"),
                        x.Q<int>("Damage")));
                }
                Weapon.CreateMelee(filepath, xmls.Q<string>("Name"), WeaponType.Melee,
                    xmls.Q<string>("WeaponClass"), 
                    xmls.Q<string>("Description"),
                    new MeleeRange().getByName(xmls.Q<string>("MeleeRange")),
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
        public static void LoadRangedWeapon(string filepath)
        {
            XmlElement xmls = LoadWC(filepath);

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
                    xmls.Enum<DamageType>(xmls.Q<string>("MeleeDamageType")),
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
        public static void LoadProjectile(string filepath)
        {
            XmlElement xmls = LoadWC(filepath);

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

        public static void LoadShield(string filepath)
        {
            XmlElement xmls = LoadWC(filepath);
            if (xmls == null) return;

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
        public static void LoadArmor(string filepath)
        {
            XmlElement xmls = LoadWC(filepath);

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
                bool riding = t.Q<string>("RidingAnimal") != "false";

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
                    riding,
                    riding ? Animal.Get(t.Q<string>("RidingAnimal")) : null,
                    riding ? ParseFuncs.parseAnimalArmor(t.Q<string>("RidingAnimal")) : null,
                    t.Q<string>("Icon"),
                    xmls.Enum<PreferSpawn>(t.Q<string>("PreferSpawn")));

                if(usedByAll)
                    foreach(Country c in Country.List)
                        TroopType.List.Add(new TroopType(x.Name, x.Description, x.SourceFile, c, x.weapons, x.sidearms, x.armor, x.shields, x.meleeSkillMin, x.meleeSkillMax, x.rangeSkillMin, x.rangeSkillMax, x.ridingAnimal, x.riddenAnimal, x.animalArmor, x.Icon, x.preferSpawn));

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
                CachedItems.renderedCountries.Add(new RenderedCountry(LoadSprite(filepath, 256, false), x));
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
                        Bodypart.List.Remove(Bodypart.List.Find(x=>x.Name==b.Name));  // so we dont have arm, left arm, right arm
                        //for (int i = 0; i < b.count; i++)
                        //{
                        if (b.countType == CountType.Sides)
                        {
                            Bodypart.Create("Left " + b.Name, b.TotalHP, b.type, b._partOf, b.painFactor, b.bleedingFactor, b.damageMultiplier, b.count, b.effects, b.effectAmount, b.hitChance, b.countType, b.group);
                            Bodypart.Create("Right " + b.Name, b.TotalHP, b.type, b._partOf, b.painFactor, b.bleedingFactor, b.damageMultiplier, b.count, b.effects, b.effectAmount, b.hitChance, b.countType, b.group);
                        }
                        else if (b.countType == CountType.Numbered)
                        {
                            for (int i = 0; i < b.count; i++)
                            {
                                string newname = ParseFuncs.AddOrdinal(i + 1) + " " + b.Name;
                                Bodypart.Create(newname, b.TotalHP, b.type, b._partOf, b.painFactor, b.bleedingFactor, b.damageMultiplier, b.count, b.effects, b.effectAmount, b.hitChance, b.countType, b.group, i);
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
                var q = Building.Create(filepath,
                    xmls.Q<string>("Name"),
                    xmls.Q<string>("Description"),
                    xmls.Q<int>("Hitpoints"),
                    xmls.Q<int>("Flammability"),
                    xmls.Q<int>("CoverQuality"),
                    xmls.Q<bool>("Lean"),
                    xmls.Q<bool>("SpecialPlace"),
                    xmls.Q<bool>("Rubble"),
                    xmls.Enum<RubbleType>(xmls.Q<string>("RubbleType")));

                                                                            // always of size 16
                renderedWalls.Add(new RenderedWall(q.ID, SpriteSheetCreator.createSpritesFromSheet(LoadImage(filepath), 512, 256).ToArray()));
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
                var temp = Furniture.Create(filepath,
                    xmls.Q<string>("Name"),
                    xmls.Q<string>("Description"),
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
                temp.tile = SpriteSheetCreator.createMutliTile(LoadTex(filepath));
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
                var temp = Floor.Create(filepath,
                    xmls.Q<string>("Name"),
                    xmls.Q<string>("Description"),
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
                var s = Roof.Create(filepath,
                    xmls.Q<string>("Name"),
                    xmls.Q<string>("Description"),
                    xmls.Q<int>("Hitpoints"),
                    xmls.Q<int>("Flammability"),
                    new RoofStats(xmls.Q<XmlNode>("RoofStats").Q<int>("SmallProjectileBlock"),
                        xmls.Q<XmlNode>("RoofStats").Q<int>("LargeProjectileBlock")));
                
                var ti = GameObject.Instantiate(ScriptableObject.CreateInstance<Tile>());
                var tex = LoadTex(filepath);
                ti.sprite = LoadSprite(tex, tex.width / 2);
                s.tile = ti;
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
                Buildings.Plant.Create(filepath,
                    xmls.Q<string>("Name"),
                    xmls.Q<string>("Description"),
                    xmls.Q<int>("Hitpoints"),
                    xmls.Q<int>("Flammability"),
                    xmls.Q<int>("CoverQuality"),
                    xmls.Q<bool>("Lean"),
                    ParseFuncs.parseSpriteSheetFromName(filepath, 512, 512));
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
                Trap.Create(filepath,
                    xmls.Q<string>("Name"),
                    xmls.Q<string>("Description"),
                    xmls.Q<int>("Hitpoints"),
                    xmls.Q<int>("Flammability"),
                    xmls.Q<int>("Damage"),
                    xmls.Q<bool>("SpecialPlace"),
                    xmls.Q<bool>("Rubble"),
                    xmls.Enum<RubbleType>(xmls.Q<string>("RubbleType")),
                    xmls.Q<int>("CoverQuality"),
                    xmls.Q<bool>("Lean"),
                    xmls.Q<bool>("IsOneUse"));
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
                var temp = Door.Create(filepath,
                    xmls.Q<string>("Name"),
                    xmls.Q<string>("Description"),
                    xmls.Enum<RubbleType>(xmls.Q<string>("RubbleType")),
                    xmls.Q<int>("Hitpoints"),
                    xmls.Q<int>("Flammability"),
                    xmls.Q<float>("OpeningSpeed"));
                temp.tile = SpriteSheetCreator.createMutliTile(LoadTex(filepath));
                // here: load into TILE for door and rooves
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
                    filepath,
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
                    xmls.Q<float>("PlantDensity"),
                    xmls.Q<float>("WaterComminality"),
                    TerrainType.Get(xmls.Q<string>("WaterClampTT")));
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
        /* public static void LoadStructure(string filepath)
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
        */

        public static void LoadGenericLists(string filepath)
        {
            XmlElement xmls = LoadXML(filepath);
            if (xmls.Q<XmlNode>("List") != null)
            {
                foreach(XmlNode x in xmls.Qs("List"))
                {
                    List<string> items = new List<string>();
                    foreach (string s in x.InnerText.Split(','))
                        items.Add(s.removeWS());
                    GenericManager.CreateGenericList(x.Q<string>("name", true), items, x.Q<string>("type", true));
                }
            }
            else
            {
                Debug.Log("Not a country file.");
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
}
