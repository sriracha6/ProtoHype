using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PawnFunctions;
using Countries;
using TroopTypes;
using UnityEditor;
using Regiments;
using Weapons;
using Attacks;
using Armors;
using Projectiles;
using XMLLoader;
using Shields;
using Body;
using Nature;

public class DevTools : MonoBehaviour
{
    public Camera cam;

    public PawnManager pManager;

    List<Weapon> sampleweapons = new List<Weapon>();
    List<Armor> armors = new List<Armor>();
    List<Projectile> sampleProjectiles = new List<Projectile>();

    private void Awake()
    {
        Loaders.LoadBodyparts("C:\\Users\\frenz\\Music\\bodyparts.xml");

        List<Attack> attks = new List<Attack>();
        attks.Add(new Attack("stab", DamageType.Sharp, false, 5)); 
        attks.Add(new Attack("slash", DamageType.Sharp, false, 10));

        WeaponManager.CreateMelee("Empty", WeaponType.Melee,"Empty","Empty",0f,false,0,0,0,new List<Attack>());

        sampleweapons.Add(WeaponManager.CreateMelee("sexknief", WeaponType.Melee, "Dagger",
                         "its a sex knife", MeleeRange.Far, false, 10, 0, 0.5f, attks));
        sampleweapons.Add(WeaponManager.CreateMelee("hugh mungus", WeaponType.Melee, "Longsword",
                 "its a sex sword", MeleeRange.Short, false, 10, 0, 2f, attks));

        List<Bodypart> head = new List<Bodypart>();
        head.Add(BodypartManager.Get("Head"));
        head.Add(BodypartManager.Get("Torso"));

        armors.Add(ArmorManager.Create("halo","it covers your dome",150,10,5,-2,Layer.outer,false,head));
        armors.Add(ArmorManager.Create("idk","it is idk",150,10,5,-2,Layer.skin,false,head));

        List<Weapon> sampleBOW = new List<Weapon>();
        sampleBOW.Add(WeaponManager.CreateRanged("bow","bow",WeaponType.Ranged, "Bow",
            25,5,RangeType.Shooter,0.5f,0f,"Sharp",5,1f,0.95f,0.8f,0.5f));

        CountryManager.Create("Germany","German");
        CountryManager.Create("France","French");

        Shield shield = ShieldManager.Create("buckler","peeny",5,5,-0.3f,0.25f,0.25f);
        List<Shield> shields = new List<Shield>();
        List<Shield> emptyShields = new List<Shield>();
        shields.Add(shield);

        TroopTypeManager.Create("Archer", CountryManager.Get("France"),sampleBOW, sampleweapons, armors, shields, 0, 2, 3,5);
        TroopTypeManager.Create("Swordsman", CountryManager.Get("Germany"), sampleweapons, new List<Weapon>(), armors, emptyShields, 3,5,0,2);

        RegimentManager.Create(TroopTypeManager.Get("Archer"), CountryManager.Get("France"), 0);
        RegimentManager.Create(TroopTypeManager.Get("Swordsman"), CountryManager.Get("Germany"), 1);
        RegimentManager.Create(TroopTypeManager.Get("Swordsman"), CountryManager.Get("France"), 2);
        RegimentManager.Create(TroopTypeManager.Get("Swordsman"), CountryManager.Get("France"), 3);
        RegimentManager.Create(TroopTypeManager.Get("Swordsman"), CountryManager.Get("France"), 4);
        RegimentManager.Create(TroopTypeManager.Get("Swordsman"), CountryManager.Get("France"), 5);
        RegimentManager.Create(TroopTypeManager.Get("Swordsman"), CountryManager.Get("France"), 6);
        RegimentManager.Create(TroopTypeManager.Get("Swordsman"), CountryManager.Get("France"), 7);
        RegimentManager.Create(TroopTypeManager.Get("Swordsman"), CountryManager.Get("France"), 8);
        RegimentManager.Create(TroopTypeManager.Get("Swordsman"), CountryManager.Get("France"), 9);

        sampleProjectiles.Add(ProjectileManager.Create("arrow","yup","Bow",3f,DamageType.Sharp,0,false));
    }


    [MenuItem("Developer/Kill All Pawns")]
    public static void thing()
    {
        PawnManager.GetAll().ForEach(p => Destroy(p.gameObject));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            pManager.CreatePawn(CountryManager.Get("Germany"), CachedItems.RandomName, TroopTypeManager.Get("Swordsman"),
                         RegimentManager.Get(0),cam.ScreenToWorldPoint(Input.mousePosition)); // make sure this id is right!
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            pManager.CreatePawn(CountryManager.Get("France"), CachedItems.RandomName, TroopTypeManager.Get("Archer"), 
                        RegimentManager.Get(1), cam.ScreenToWorldPoint(Input.mousePosition), sampleProjectiles);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            var go = Instantiate(Loader.loader.firePrefab);
            Vector2Int p = Vector2Int.FloorToInt(cam.ScreenToWorldPoint(Input.mousePosition));
            go.transform.position = new Vector3(p.x, p.y, -2);
        }
    }
}
