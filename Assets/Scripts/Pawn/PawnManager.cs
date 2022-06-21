using Countries;
using PawnFunctions;
using Projectiles;
using Regiments;
using System;
using System.Collections;
using System.Collections.Generic;
using TroopTypes;
using Armors;
using UnityEngine;

using Random = UnityEngine.Random;

public class PawnManager : MonoBehaviour
{
    public static PawnManager I = null;
    public GameObject pawnPrefab; // lol this is the most important line of code in the game
    public GameObject horsePrefab;
    private static List<Pawn> allPawns = new List<Pawn>();

    protected void Start()
    {
        if (I == null)
        {
            I = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            if (Player.loadedFrom == LoadFrom.Quickbattle)
                CreatePawns(QuickBattle.I.regimentSize, QuickBattle.I.friends, QuickBattle.I.enemies);
    }

    public void CreatePawns(int regimentSize, List<CountryInfo> friendlies, List<CountryInfo> enemies)
    {
        List<CountryInfo> cs = new List<CountryInfo>();
        cs.AddRange(friendlies);
        cs.AddRange(enemies);

        foreach(CountryInfo country in friendlies)
        {
            for (int i = 0; i < country.regimentsCount; i++)
            {
                var troopType = RandomTroopType(country.country);
                Regiment.Create(troopType, country.country);

                for (int j = 0; j < Mathf.Max(Random.Range(0.75f, 1.25f) * regimentSize, 2); j++)
                {
                    Pawn p = CreatePawn(country.country, CachedItems.RandomName, troopType,
                        Regiment.Get(i), new Vector2(Random.Range(0, MapGenerator.I.mapW), Random.Range(0, MapGenerator.I.mapH)) /* TODO : RANDOM POSITION? WHY?? FIX! */); // make sure this id is right!
                    if(troopType.ridingAnimal)
                    {
                        GameObject go = Instantiate(horsePrefab); // im so sorry
                        var supersorryforthisone = go.GetComponent<AnimalBehavior>();
                        supersorryforthisone.rider = p;
                        supersorryforthisone.sourceAnimal = troopType.riddenAnimal;
                    }
                }
            }
        }
    }

    public static TroopType RandomTroopType(Country c)
    {
        List<TroopType> t = TroopType.List.FindAll(x => x.country == c);
        return t[Random.Range(0, t.Count)];
    }

    public Pawn CreatePawn(Country c, string n, TroopType tt, Regiment r, Vector2 pos, List<Projectile> projectiles = null)
    {
        // USE POOLING!!!!!!!!!!!!!!!!!!!! TODO
        GameObject newPawnObj = Instantiate(pawnPrefab);//gameObject.AddComponent<Pawn>();
        Pawn newPawn = newPawnObj.GetComponent<Pawn>();

        newPawnObj.transform.position = new Vector3(pos.x, pos.y, -0.5f);

        newPawn.country = c;
        newPawn.pname = n;
        newPawn.troopType = tt;
        newPawn.regiment = r;
        r.Add(newPawn);

        foreach (Country co in Country.List)
        {
            if (co != newPawn.country)
            {
                newPawn.enemyCountries.Add(co);
            }
        }

        newPawn.inventory = projectiles;
        if(tt.weapons.Count > 0) {
            newPawn.heldPrimary = tt.weapons[UnityEngine.Random.Range(0, tt.weapons.Count)];
            newPawn.activeWeapon = newPawn.heldPrimary;
            newPawn.hasPrimary = true;
        }
        else
            newPawn.hasPrimary = false;

        newPawn.activeWeapon = newPawn.heldPrimary;

        newPawn.activeWeaponSlot = ActiveWeapon.Primary;

        List<Armor> armor = new List<Armor>();
        for(int i = 0; i < tt.armor.Count - 1; i++)
        {
            int choice = Random.Range(0, tt.armor[i].Count);
            armor.Add(tt.armor[i][choice]);
        }
        if(tt.armor.Count > 0)
        armor.AddRange(tt.armor[tt.armor.Count-1]);

        newPawn.armor.AddRange(armor/*tt.armor*/);
        if (tt.shields.Count > 0)
        {
            newPawn.shield = tt.shields[UnityEngine.Random.Range(0, tt.shields.Count)];
            newPawn.hasShield = true;
        }
        else
            newPawn.hasShield = false;

        newPawn.meleeSkill = UnityEngine.Random.Range(tt.meleeSkillMin, tt.meleeSkillMax + 1);
        newPawn.rangeSkill = UnityEngine.Random.Range(tt.rangeSkillMin, tt.rangeSkillMax + 1);

        if (tt.sidearms.Count > 0)
        {
            newPawn.heldSidearm = tt.sidearms[UnityEngine.Random.Range(0, tt.sidearms.Count)];
            newPawn.hasSidearm = true;
        }
        else
            newPawn.hasSidearm = false;

        newPawn.country.Add(newPawn);
        newPawn.sprite.material.color = generateSkinColor();
        allPawns.Add(newPawn);
        // todo
        PopulateRegiments.updateAllRegimentsSelectNumber(Player.regimentSelectNumber);
        if (newPawn.armor != null)
            foreach (Armor a in newPawn.armor)
                newPawn.pawnPathfind.speed += (a.MovementSpeedAffect / 100); // almost made this a multiplily wonder how many other times ive fucked up a calculation

        return newPawn;
    }

    #region ------- Gets --------
    public static List<Pawn> GetAll() // og code of this used to use gameobject.getobjectswithtag AND getcomponent...... for each pawn
    {
        return allPawns;
    }

    public static List<Pawn> GetAllFriendlies()
    {
        return allPawns.FindAll(x=>x.country==Player.playerCountry);
    }

    public static Pawn GetRandom() // find a way to use this todo
    {
        if (allPawns.Count > 0)
            return allPawns[Random.Range(0, allPawns.Count)];
        else return null;
    }

    public static List<Pawn> GetEnemies(List<Country> enemies) // use this instead of that one todo
    {
        return allPawns.FindAll(x=>enemies.Contains(x.country));
    }
    #endregion

    public static Color generateSkinColor()
    {
        return Color.Lerp(new Color(241 / 255f, 194 / 255f, 125 / 255f), new Color(141 / 255f, 85 / 255f, 36 / 255f), Random.Range(0f, 1f));
    }
    public static Color generateSkinColor(int darkness)
    {
        return Color.Lerp(new Color(241 / 255f, 194 / 255f, 125 / 255f), new Color(141 / 255f, 85 / 255f, 36 / 255f), Random.Range(0f, 1f));
    }
}
