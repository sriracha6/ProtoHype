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
    public static Loader I = null;
    public List<Vital> defaultVitals = new List<Vital>();

    protected void Awake()
    {
        if (I == null)
            I = this;
        else
            return;

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
            Codename
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
  TIME: {DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()}
  MEM: {SystemInfo.systemMemorySize}
  VMEM: {SystemInfo.graphicsMemorySize}
  CPU: {SystemInfo.processorType} : {SystemInfo.processorFrequency}MHz : {SystemInfo.processorCount}x
  GPU: {SystemInfo.graphicsDeviceName} : {SystemInfo.graphicsDeviceVersion}
       {"NO MODS DETECTED"}                          
        ");

        Logger.Divide("START XML LOAD");
        Loaders.LoadBodyparts("C:\\Users\\frenz\\Music\\bodyparts.xml");

        Loaders.LoadMeleeWeapon("C:\\Users\\frenz\\Music\\ahlspiess.wc");
        Loaders.LoadRangedWeapon("C:\\Users\\frenz\\Music\\arbalest.wc");
        Loaders.LoadShield("C:\\Users\\frenz\\Music\\heatershield.wc");
        Loaders.LoadArmor("C:\\Users\\frenz\\Music\\guantlet.wc");
        Loaders.LoadProjectile("C:\\Users\\frenz\\Music\\bodkinarrow.wc");

        Loaders.LoadCountry(@"C:\Users\frenz\Music\countries\germany.wc");
        Loaders.LoadCountry(@"C:\Users\frenz\Music\countries\france.wc");

        /*Loaders.LoadNature("C:\\Users\\frenz\\Music\\cactus.xml");
        Loaders.LoadNature("C:\\Users\\frenz\\Music\\shrub.xml");
        Loaders.LoadNature("C:\\Users\\frenz\\Music\\tree.xml");
        Loaders.LoadNature("C:\\Users\\frenz\\Music\\pinetree.xml");
        Loaders.LoadNature("C:\\Users\\frenz\\Music\\grass6969.xml");
        Loaders.LoadNature("C:\\Users\\frenz\\Music\\bush.xml");*/
        Loaders.LoadTerrainType("C:\\Users\\frenz\\Music\\tt\\deepsand.wc");
        Loaders.LoadTerrainType("C:\\Users\\frenz\\Music\\tt\\dirt.wc");
        Loaders.LoadTerrainType("C:\\Users\\frenz\\Music\\tt\\sand.wc");
        Loaders.LoadTerrainType("C:\\Users\\frenz\\Music\\tt\\grass.wc");
        Loaders.LoadTerrainType("C:\\Users\\frenz\\Music\\tt\\quicksand.wc");

        Loaders.LoadBiome("C:\\Users\\frenz\\Music\\plains.xml");
        Loaders.LoadBiome("C:\\Users\\frenz\\Music\\desert.xml");

        Loaders.LoadAnimal(@"C:\Users\frenz\Music\horse.wc");
        
        Loaders.LoadAnimalArmor(@"C:\Users\frenz\Music\aamor\barding.wc");
        Loaders.LoadAnimalArmor(@"C:\Users\frenz\Music\aamor\cataphract.wc");

        TerrainType.Create("Test Tile", 1f, Color.magenta, null, SpecialType.None, false);
        Loaders.loadBlood();
        Loaders.loadNames();

        foreach (string file in Directory.GetFiles(@"D:\medgame\wcfile\WC File Creator\WC File Creator\bin\Release\net6.0\pack\melee"))
            Loaders.LoadMeleeWeapon(file);
        foreach (string file in Directory.GetFiles(@"D:\medgame\wcfile\WC File Creator\WC File Creator\bin\Release\net6.0\pack\ranged"))
            Loaders.LoadRangedWeapon(file);
        foreach (string file in Directory.GetFiles(@"D:\medgame\wcfile\WC File Creator\WC File Creator\bin\Release\net6.0\pack\projectiles"))
            Loaders.LoadProjectile(file);
        foreach (string file in Directory.GetFiles(@"D:\medgame\wcfile\WC File Creator\WC File Creator\bin\Release\net6.0\pack\armor"))
            Loaders.LoadArmor(file);
        foreach (string file in Directory.GetFiles(@"D:\medgame\wcfile\WC File Creator\WC File Creator\bin\Release\net6.0\pack\shields"))
            Loaders.LoadShield(file);

        foreach(string file in Directory.GetFiles(@"C:\Users\frenz\Music\trt\sigh"))
            Loaders.LoadTroopTypeIcon(file);

        Loaders.LoadCountryOutfit(@"C:\Users\frenz\Music\trt\germany.xml");
        Loaders.LoadCountryOutfit(@"C:\Users\frenz\Music\trt\france.xml");

        foreach (string file in Directory.GetFiles(@"C:\Users\frenz\Music\struc\furn"))
            Loaders.LoadFurniture(file);
        foreach (string file in Directory.GetFiles(@"C:\Users\frenz\Music\struc\roof"))
            Loaders.LoadRoof(file);
        foreach (string file in Directory.GetFiles(@"C:\Users\frenz\Music\struc\door"))
            Loaders.LoadDoor(file);
        foreach (string file in Directory.GetFiles(@"C:\Users\frenz\Music\struc\wall"))
            Loaders.LoadBuilding(file);
        foreach (string file in Directory.GetFiles(@"C:\Users\frenz\Music\struc\room"))
            Loaders.LoadRoom(file);
        foreach (string file in Directory.GetFiles(@"C:\Users\frenz\Music\struc\ture"))
            Loaders.LoadStructure(file);

        Logger.EndDivide();
        //Loaders.LoadCountryOutfit("C:\\Users\\frenz\\Music\\germany.xml");

        defaultVitals.Add(new Vital(VitalSystem.Dexterity, 1f));
        defaultVitals.Add(new Vital(VitalSystem.Sight, 1f));
        defaultVitals.Add(new Vital(VitalSystem.Breathing, 1f));
        defaultVitals.Add(new Vital(VitalSystem.Conciousness, 1f));
        defaultVitals.Add(new Vital(VitalSystem.BloodPumping, 1f));
        defaultVitals.Add(new Vital(VitalSystem.Moving, 1f));
    }
}