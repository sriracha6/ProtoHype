using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XMLLoader;

public class Loader : MonoBehaviour
{
    protected void Awake()
    {
        Loaders.LoadBodyparts("C:\\Users\\frenz\\Music\\bodyparts.xml");

        Loaders.LoadMeleeWeapon("C:\\Users\\frenz\\Music\\ahlspiess.xml");
        Loaders.LoadRangedWeapon("C:\\Users\\frenz\\Music\\arbalest.xml");
        Loaders.LoadShield("C:\\Users\\frenz\\Music\\heatershield.xml");
        Loaders.LoadArmor("C:\\Users\\frenz\\Music\\guantlet.xml");
        Loaders.LoadProjectile("C:\\Users\\frenz\\Music\\bodkinarrow.xml");

        Loaders.LoadNature("C:\\Users\\frenz\\Music\\cactus.xml");
        Loaders.LoadNature("C:\\Users\\frenz\\Music\\shrub.xml");
        Loaders.LoadNature("C:\\Users\\frenz\\Music\\tree.xml");
        Loaders.LoadNature("C:\\Users\\frenz\\Music\\pinetree.xml");
        Loaders.LoadNature("C:\\Users\\frenz\\Music\\grass6969.xml");
        Loaders.LoadNature("C:\\Users\\frenz\\Music\\bush.xml");
        Loaders.LoadTerrainType("C:\\Users\\frenz\\Music\\deepsand.xml");
        Loaders.LoadTerrainType("C:\\Users\\frenz\\Music\\dirt.xml");
        Loaders.LoadTerrainType("C:\\Users\\frenz\\Music\\grass.xml");
        Loaders.LoadTerrainType("C:\\Users\\frenz\\Music\\quicksand.xml");

        //Loaders.LoadTerrainType("C:\\Users\\frenz\\Music\\sand.xml");
        Loaders.LoadBiome("C:\\Users\\frenz\\Music\\plains.xml");
        Loaders.LoadBiome("C:\\Users\\frenz\\Music\\desert.xml");

        Loaders.loadBlood();
        Loaders.loadNames();
        //Loaders.LoadCountryOutfit("C:\\Users\\frenz\\Music\\germany.xml");
    }
}
