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
using XMLLoader;

using Random = UnityEngine.Random;
using System.Linq;

public class PawnManager : MonoBehaviour
{
    public static PawnManager I = null;
    public GameObject pawnPrefab; // lol this is the most important line of code in the game
    public GameObject horsePrefab;
    private static readonly List<Pawn> allPawns = new List<Pawn>();

    public static bool doneLoading = false;
    List<Vector2Int> usedPoints = new List<Vector2Int>();

    public float minDistance;
    public const int AROUND_BASE_DISTANCE = 30;
    public const int OUTSIDE_BASE_DISTANCE = 50;

    protected void Start()
    {
        if (I == null)
        {
            I = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            if (Player.loadedFrom == LoadFrom.Quickbattle)
                I.CreatePawns(QuickBattle.I.regimentSize, QuickBattle.I.friends, QuickBattle.I.enemies);
    }
    
    public void CreatePawns(int regimentSize, List<CountryInfo> friendlies, List<CountryInfo> enemies)
    { // this is somehow the cause for that dumb bug with animals, array size must be .... and onchange weapon
        Loading.I.Status = "Creating people...";
        List<CountryInfo> cs = new List<CountryInfo>();
        cs.AddRange(friendlies);
        cs.AddRange(enemies);
        var map = MapGenerator.I;

        foreach(CountryInfo country in cs)
        {
            for (int currentRegiment = 0; currentRegiment < country.regimentsCount; currentRegiment++)
            {
                var troopType = RandomTroopType(country.country);
                Regiment.Create(troopType, country.country);

                Vector2Int regimentPos;
                if (map.structure != null)
                    if (troopType.preferSpawn == PreferSpawn.AroundBase)
                        regimentPos = new Vector2Int(Random.Range(map.mapWidth - AROUND_BASE_DISTANCE, map.mapWidth), Random.Range(map.mapHeight - AROUND_BASE_DISTANCE, map.mapHeight));
                    else if (troopType.preferSpawn == PreferSpawn.OutsideBase)
                        regimentPos = new Vector2Int(Random.Range(map.structurePos.x + map.structureSize.x, map.structurePos.x + map.structureSize.x + OUTSIDE_BASE_DISTANCE), Random.Range(map.structurePos.y + map.structureSize.y, map.structurePos.y + map.structureSize.y + OUTSIDE_BASE_DISTANCE));
                    else
                        regimentPos = new Vector2Int(Random.Range(map.structurePos.x, map.structurePos.x+map.structureSize.x), Random.Range(map.structurePos.y, map.structurePos.y+map.structureSize.y));
                else
                    regimentPos = new Vector2Int(Random.Range(0, map.mapWidth), Random.Range(0, map.mapHeight));

                float regimentMemberCount = Mathf.Max(Random.Range(0.75f, 1.25f) * regimentSize, 2);
                for (int j = 0; j < regimentMemberCount; j++)
                {
                    Vector2Int pos = PathfindExtra.FindNearest(regimentPos, I.usedPoints);
                    pos = new Vector2Int(pos.x + Random.Range(-7,8), pos.y + Random.Range(-1, 2)).clampVector();
                    PathfindExtra.SetUsed(pos.x, pos.y);
                    I.usedPoints.Add(pos);

                    Pawn p = I.CreatePawn(country.country, CachedItems.RandomName, troopType,
                        Regiment.Get(currentRegiment), pos); // make sure this id is right!
                    
                    if (troopType.ridingAnimal)
                    {
                        GameObject go = Instantiate(horsePrefab); // im so sorry
                        var supersorryforthisone = go.GetComponent<AnimalBehavior>();
                        supersorryforthisone.rider = p;
                        supersorryforthisone.sourceAnimal = troopType.riddenAnimal;
                    }
                }
            }
        }
        doneLoading = true;
        for(int i = 0; i < I.usedPoints.Count; i++)
            PathfindExtra.SetFree(I.usedPoints[i].x, I.usedPoints[i].y);
        I.usedPoints = null;
        PopulateRegiments.updateAllRegimentsSelectNumber(Player.regimentSelectNumber);
    }

    public static TroopType RandomTroopType(Country c)
    {
        List<TroopType> t = TroopType.List.FindAll(x => x.country == c);
        return t[Random.Range(0, t.Count)];
    }

    public Pawn CreatePawn(Country c, string n, TroopType tt, Regiment r, Vector2 pos, Projectile projectile=null)
    {
        // USE POOLING!!!!!!!!!!!!!!!!!!!! TODO
        GameObject newPawnObj = Instantiate(pawnPrefab);//gameObject.AddComponent<Pawn>();
        Pawn newPawn = newPawnObj.GetComponent<Pawn>();

        newPawnObj.transform.position = new Vector3(pos.x, pos.y, -0.5f);

        tt.weapons = tt.weapons.StripNulls();
        tt.shields = tt.shields.StripNulls();
        tt.sidearms = tt.sidearms.StripNulls();
        tt.armor = tt.armor.StripNulls();

        newPawn.country = c;
        newPawn.pname = n;
        newPawn.troopType = tt;
        newPawn.regiment = r;
        r.Add(newPawn);

        if(newPawn.isFlagBearer)
        {
            newPawn.hasPrimary = true;
            newPawn.heldPrimary = WCMngr.I.flagWeapon;
            newPawn.activeWeapon = WCMngr.I.flagWeapon;
        }

        foreach (Country co in Country.List)
            if (co != newPawn.country)
                newPawn.enemyCountries.Add(co);

        if(tt.weapons.Count > 0)
        {
            newPawn.heldPrimary = tt.weapons[UnityEngine.Random.Range(0, tt.weapons.Count)];

            if (newPawn.heldPrimary.Type == Weapons.WeaponType.Ranged && newPawn.heldPrimary.rangeType == Weapons.RangeType.Shooter)
                if (projectile == null)
                {
                    var forThisPawn = Projectile.List.FindAll(x => x.forWeaponClass == newPawn.heldPrimary.weaponClass);

                    if (forThisPawn.Count > 0)
                        newPawn.inventory = forThisPawn.randomElement();
                    else
                    {
                        Debug.Log($"MISSING WEAPONCLASS : {newPawn.heldPrimary.weaponClass} : {newPawn.heldPrimary.Name}");
                    }
                }
                else
                    newPawn.inventory = projectile;

            newPawn.hasPrimary = true;
        }
        else
            newPawn.hasPrimary = false;

        if (newPawn.hasPrimary == false)
            Debug.Log($"{tt.country.memberName} {tt.Name} has no primaries? is that right? : {tt.weapons.Count}");
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

        newPawn.meleeSkill = Random.Range(tt.meleeSkillMin, tt.meleeSkillMax + 1);
        newPawn.rangeSkill = Random.Range(tt.rangeSkillMin, tt.rangeSkillMax + 1);

        if (tt.sidearms.Count > 0)
        {
            newPawn.heldSidearm = tt.sidearms[UnityEngine.Random.Range(0, tt.sidearms.Count)];
            newPawn.hasSidearm = true;
            if (!newPawn.hasPrimary)
                newPawn.heldPrimary = newPawn.heldSidearm;
        }
        else
            newPawn.hasSidearm = false;

        newPawn.country.Add(newPawn);
        newPawn.sprite.material.color = GenerateSkinColor();
        allPawns.Add(newPawn);
        // todo
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

    public static Color GenerateSkinColor()
    {
        return Color.Lerp(new Color(241 / 255f, 194 / 255f, 125 / 255f), new Color(141 / 255f, 85 / 255f, 36 / 255f), Random.Range(0f, 1f));
    }
    public static Color GenerateSkinColor(int darkness)
    {
        return Color.Lerp(new Color(241 / 255f, 194 / 255f, 125 / 255f), new Color(141 / 255f, 85 / 255f, 36 / 255f), Random.Range(0f, 1f));
    }
}
