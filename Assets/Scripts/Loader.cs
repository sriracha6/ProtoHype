using Body;
using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Tilemaps;
using XMLLoader;

public class Loader : MonoBehaviour
{
    public static Loader loader { get; private set; }

    public RuleTile mountainTile;
    public Tile testTile;
    public GameObject testMat;
    public List<Vital> defaultVitals = new List<Vital>();

    protected void Awake()
    {
        loader = this;

        string md5checksum = "";
        /*using (var md5 = MD5.Create())
        {
            using (var stream = System.IO.File.OpenRead(Directory.GetCurrentDirectory()))//+"/Managed/AssemblyCSharp.dll"))
            {
                md5checksum = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
            }
        }*/

        string bit = Environment.Is64BitOperatingSystem ? "64 bit" : "32 bit";
        string bit2 = Environment.Is64BitProcess ? "-64" : "-32";

        Logger.Clear();
        Logger.Log($@"
 _  _  _, __,  _, _,_  _, _, _ ___
 |  | / \ |_) / ` |_| / \ |\ |  | 
 |/\| |~| | \ \ , | | |~| | \|  | 
 ~  ~ ~ ~ ~ ~  ~  ~ ~ ~ ~ ~  ~  ~ 
    Version 0.5 :: LOGFILE
 'This machine. I hate this machine. 
  Because it does exactly what I tell it
  to do and not what I want it to do.'

  CS: {md5checksum}
  CD: {Directory.GetCurrentDirectory()}
  OS: {bit}{bit2} {Environment.OSVersion}
       {"NO MODS DETECTED"}                          
        ");

        Loaders.LoadBodyparts("C:\\Users\\frenz\\Music\\bodyparts.xml");

        Loaders.LoadMeleeWeapon("C:\\Users\\frenz\\Music\\ahlspiess.xml");
        Loaders.LoadRangedWeapon("C:\\Users\\frenz\\Music\\arbalest.xml");
        Loaders.LoadShield("C:\\Users\\frenz\\Music\\heatershield.xml");
        Loaders.LoadArmor("C:\\Users\\frenz\\Music\\guantlet.xml");
        Loaders.LoadProjectile("C:\\Users\\frenz\\Music\\bodkinarrow.xml");

        /*Loaders.LoadNature("C:\\Users\\frenz\\Music\\cactus.xml");
        Loaders.LoadNature("C:\\Users\\frenz\\Music\\shrub.xml");
        Loaders.LoadNature("C:\\Users\\frenz\\Music\\tree.xml");
        Loaders.LoadNature("C:\\Users\\frenz\\Music\\pinetree.xml");
        Loaders.LoadNature("C:\\Users\\frenz\\Music\\grass6969.xml");
        Loaders.LoadNature("C:\\Users\\frenz\\Music\\bush.xml");*/
        Loaders.LoadTerrainType("C:\\Users\\frenz\\Music\\deepsand.xml");
        Loaders.LoadTerrainType("C:\\Users\\frenz\\Music\\dirt.xml");
        Loaders.LoadTerrainType("C:\\Users\\frenz\\Music\\sand.xml");
        Loaders.LoadTerrainType("C:\\Users\\frenz\\Music\\grass.xml");
        Loaders.LoadTerrainType("C:\\Users\\frenz\\Music\\quicksand.xml");
        Loaders.LoadTerrainType("C:\\Users\\frenz\\Music\\testtile.xml");

        //Loaders.LoadTerrainType("C:\\Users\\frenz\\Music\\sand.xml");
        Loaders.LoadBiome("C:\\Users\\frenz\\Music\\plains.xml");
        Loaders.LoadBiome("C:\\Users\\frenz\\Music\\desert.xml");

        Loaders.loadBlood();
        Loaders.loadNames();
        //Loaders.LoadCountryOutfit("C:\\Users\\frenz\\Music\\germany.xml");

        defaultVitals.Add(new Vital(VitalSystem.Dexterity, 1f));
        defaultVitals.Add(new Vital(VitalSystem.Sight, 1f));
        defaultVitals.Add(new Vital(VitalSystem.Breathing, 1f));
        defaultVitals.Add(new Vital(VitalSystem.Conciousness, 1f));
        defaultVitals.Add(new Vital(VitalSystem.BloodPumping, 1f));
        defaultVitals.Add(new Vital(VitalSystem.Moving, 1f));
    }
}