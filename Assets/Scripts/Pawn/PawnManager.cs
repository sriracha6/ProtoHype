using Countries;
using PawnFunctions;
using Projectiles;
using Regiments;
using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using TroopTypes;
using Armors;
using UnityEngine;
using XMLLoader;

using Random = UnityEngine.Random;
using System.Linq;
using static MapGenerator;

public class PawnManager : MonoBehaviour
{
    public static PawnManager I = null;
    public GameObject pawnPrefab; // lol this is the most important line of code in the game
    public GameObject horsePrefab;
    public static readonly List<Pawn> allPawns = new List<Pawn>();

    static readonly List<(Regiment r, Vector2Int pos)> regimentPoses = new List<(Regiment r, Vector2Int pos)>();

    public static bool doneLoading = false;
    List<Vector2Int> usedPoints = new List<Vector2Int>();

    public float minDistance;
    public const int AROUND_BASE_DISTANCE = 30;
    public const int OUTSIDE_BASE_DISTANCE = 50;
    public const int MIN_ENEMY_DISTANCE = 50;

    protected void Awake()
    {
        if (I == null)
        {
            I = this;
            DontDestroyOnLoad(gameObject);
        }
        else if(Menus.I.inBattle)
            I.CreatePawns(QuickBattle.I.regimentSize, QuickBattle.I.friends, QuickBattle.I.enemies);
    }


    public void CreatePawns(int regimentSize, List<CountryInfo> friendlies, List<CountryInfo> enemies)
    { // this is somehow the cause for that dumb bug with animals, array size must be .... and onchange weapon
        Loading.I.Status = "Birthing people...";
        List<CountryInfo> cs = new List<CountryInfo>();
        cs.AddRange(friendlies);
        cs.AddRange(enemies);
        var map = MapGenerator.I;

        //Vector2Int friendsPos = PositionRegiment(new Vector2Int(Random.Range(map.mapWidth - AROUND_BASE_DISTANCE, map.mapWidth), Random.Range(map.mapHeight - AROUND_BASE_DISTANCE, map.mapHeight)));
        //Vector2Int enemiesPos = PositionRegiment(new Vector2Int(Random.Range(map.mapWidth - AROUND_BASE_DISTANCE, map.mapWidth), Random.Range(map.mapHeight - AROUND_BASE_DISTANCE, map.mapHeight)));
        NSide friendsSide = ParseFuncs.RandomEnum<NSide>(new System.Random(Random.state.GetHashCode()));
        int friendsSize = (int)Random.Range(0.1f * MapGenerator.I.mapWidth, 0.4f * MapGenerator.I.mapWidth);
        
        NSide enemiesSide = NSide.Right;
        int enemiesSize = (int)Random.Range(0.1f * MapGenerator.I.mapWidth, 0.4f * MapGenerator.I.mapWidth);

        if(friendsSide == NSide.Left) enemiesSide = NSide.Right;
        if(friendsSide == NSide.Right) enemiesSide = NSide.Left;
        if(friendsSide == NSide.Bottom) enemiesSide = NSide.Top;
        if(friendsSide == NSide.Top) enemiesSide = NSide.Bottom;

        foreach (CountryInfo country in cs)
        {
            for (int currentRegiment = 0; currentRegiment < country.regimentsCount; currentRegiment++)
            {
                var troopType = RandomTroopType(country.country);
                Regiment.Create(troopType, country.country);

                Vector2Int regimentPos;
                if (map.structure != null)
                    if (troopType.preferSpawn == PreferSpawn.AroundBase) // todo: this is only on right side
                        regimentPos = new Vector2Int(Random.Range(map.mapWidth - AROUND_BASE_DISTANCE, map.mapWidth), Random.Range(map.mapHeight - AROUND_BASE_DISTANCE, map.mapHeight));
                    else if (troopType.preferSpawn == PreferSpawn.OutsideBase)
                        regimentPos = new Vector2Int(Random.Range(map.structurePos.x + map.structureSize.x, map.structurePos.x + map.structureSize.x + OUTSIDE_BASE_DISTANCE), Random.Range(map.structurePos.y + map.structureSize.y, map.structurePos.y + map.structureSize.y + OUTSIDE_BASE_DISTANCE));
                    else
                        regimentPos = new Vector2Int(Random.Range(map.structurePos.x, map.structurePos.x + map.structureSize.x), Random.Range(map.structurePos.y, map.structurePos.y + map.structureSize.y));
                else
                {
                    if (QuickBattle.I.friends.Contains(country))
                    {
                        if (friendsSide == NSide.Top) regimentPos = new Vector2Int(Random.Range(0, MapGenerator.I.mapWidth), Random.Range(MapGenerator.I.mapHeight - friendsSize, MapGenerator.I.mapHeight));
                        else if (friendsSide == NSide.Bottom) regimentPos = new Vector2Int(Random.Range(0, MapGenerator.I.mapWidth), Random.Range(0, friendsSize));
                        else if (friendsSide == NSide.Right) regimentPos = new Vector2Int(Random.Range(MapGenerator.I.mapWidth - friendsSize, MapGenerator.I.mapWidth), Random.Range(0, MapGenerator.I.mapHeight));
                        else if (friendsSide == NSide.Left) regimentPos = new Vector2Int(Random.Range(0, friendsSize), Random.Range(0, MapGenerator.I.mapHeight));
                        else regimentPos = Vector2Int.zero; // why tf do i need this?
                    }
                    else
                    {
                        if (enemiesSide == NSide.Top) regimentPos = new Vector2Int(Random.Range(0, MapGenerator.I.mapWidth), Random.Range(MapGenerator.I.mapHeight - enemiesSize, MapGenerator.I.mapHeight));
                        else if (enemiesSide == NSide.Bottom) regimentPos = new Vector2Int(Random.Range(0, MapGenerator.I.mapWidth), Random.Range(0, enemiesSize));
                        else if (enemiesSide == NSide.Right) regimentPos = new Vector2Int(Random.Range(MapGenerator.I.mapWidth - enemiesSize, MapGenerator.I.mapWidth), Random.Range(0, MapGenerator.I.mapHeight));
                        else if (enemiesSide == NSide.Left) regimentPos = new Vector2Int(Random.Range(0, enemiesSize), Random.Range(0, MapGenerator.I.mapHeight));
                        else regimentPos = Vector2Int.zero;
                    }
                }

                regimentPos = PositionRegiment(regimentPos);

                regimentPoses.Add((Regiment.Get(currentRegiment), regimentPos));

                float regimentMemberCount = Mathf.Max(Random.Range(0.75f, 1.25f) * regimentSize, 2);
                for (int j = 0; j < regimentMemberCount; j++)
                {
                    Vector2Int pos = PositionPawn(regimentPos, I.usedPoints);

                    Pawn p = I.CreatePawn(country.country, CachedItems.RandomName, troopType,
                        Regiment.Get(currentRegiment), pos); // make sure this id is right!

                    if (troopType.ridingAnimal)
                    {
                        GameObject go = Instantiate(I.horsePrefab, p.transform); // v im so sorry
                        go.transform.localPosition = new Vector3(-2, -3.26f, -1);
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
        I.usedPoints = new List<Vector2Int>();
        foreach(Country c in Player.enemies)
            foreach(Pawn p in c.members)
                if(p.hasPrimary && p.heldPrimary.Type != Weapons.WeaponType.Ranged)
                p.actionTypes.Add (new ActionType("SearchAndDestroy", true));

        PopulateRegiments.updateAllRegimentsSelectNumber(Player.regimentSelectNumber);
    }

    private static Vector2Int PositionPawn(Vector2Int pos, List<Vector2Int> usedPoints, int maxRecursion=100)
    {
        if(maxRecursion <= 0)
            return pos;

        var newPos = (Vector2Int)PathfindExtra.FindNearest(pos, usedPoints);
        newPos = new Vector2Int(newPos.x + Random.Range(-7, 8), newPos.y + Random.Range(-7, 8)).clampVector();

        if (TilemapPlace.tilemap[newPos.x, newPos.y].type == SpecialType.None)
        {
            I.usedPoints.Add(newPos);
            PathfindExtra.SetUsed(pos.x, pos.y);
            return newPos;
        }
        else
            return PositionPawn(newPos, usedPoints, maxRecursion--);
    }

    private static Vector2Int PositionRegiment(Vector2Int vPos, int maxRecursion=100)
    {
        if (maxRecursion < 0)
            return vPos;

        Vector2Int bestGuess = vPos;
        int s = 0;
        foreach ((Regiment r, Vector2Int pos) in regimentPoses)
        {
            if (Vector2.Distance(pos, vPos) < MIN_ENEMY_DISTANCE && !r.isFriendly)
            {
                bestGuess = new Vector2Int(Mathf.Clamp(vPos.x + Random.Range(-30, 31), 0, MapGenerator.I.mapWidth), Mathf.Clamp(vPos.y + Random.Range(-30, 31), 0, MapGenerator.I.mapHeight));
                s++;
            }
        }
        if (s == 0)
            return bestGuess;
        else
            return PositionRegiment(vPos, maxRecursion--);
    }

    public static TroopType RandomTroopType(Country c)
    {
        //if (c != Player.playerCountry)
        //    return TroopType.List.Find(x=>x.country==c&&x.Name == "Archer");
        List<TroopType> t = TroopType.List.FindAll(x => x.country == c);
        return t[Random.Range(0, t.Count)];
    }

    public Pawn CreatePawn(Country c, string n, TroopType tt, Regiment r, Vector2 pos, Projectile projectile=null)
    {
        // USE POOLING!!!!!!!!!!!!!!!!!!!! TODO
        GameObject newPawnObj = Instantiate(I.pawnPrefab);//gameObject.AddComponent<Pawn>();
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

        if (Player.friends.Contains(newPawn.country))
            newPawn.enemyCountries.AddRange(Player.enemies);
        else
            newPawn.enemyCountries.AddRange(Player.friends);

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
        newPawn.skinColor = GenerateSkinColor();

        newPawn.pawnPathfind.isRunAndGun = Settings.RunAndGunDefaultState;

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
}
