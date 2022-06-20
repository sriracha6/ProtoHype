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
using Animals;
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
        List<Attack> attks = new List<Attack>();
        attks.Add(new Attack("stab", DamageType.Sharp, false, 5)); 
        attks.Add(new Attack("slash", DamageType.Sharp, false, 10));

        //WeaponManager.CreateMelee("","Empty", WeaponType.Melee,"Empty","Empty",0f,false,0,0,0,new List<Attack>());
        armors = new List<Armor>(ArmorManager.ArmorList);
        sampleweapons = new List<Weapon>(WeaponManager.WeaponList);
        //sampleweapons.Add(WeaponManager.CreateMelee("","sexknief", WeaponType.Melee, "Dagger",
        //                 "its a sex knife", MeleeRange.Far, false, 10, 0, 0.5f, attks));
        //sampleweapons.Add(WeaponManager.CreateMelee("","hugh mungus", WeaponType.Melee, "Longsword",
        //         "its a sex sword", MeleeRange.Short, false, 10, 0, 2f, attks));

        List<Bodypart> head = new List<Bodypart>();
        head.Add(BodypartManager.Get("Head"));
        head.Add(BodypartManager.Get("Torso"));

        //armors.Add(ArmorManager.Create("","halo","it covers your dome",150,10,5,-2,Layer.outer,false,head));
        //armors.Add(ArmorManager.Create("","idk","it is idk",150,10,5,-2,Layer.skin,false,head));

        //List<Weapon> sampleBOW = new List<Weapon>();
        //sampleBOW.Add(WeaponManager.CreateRanged("","bow","bow",WeaponType.Ranged, "Bow",
        //    25,5,RangeType.Shooter,0.5f,0f,"Sharp",5,1f,0.95f,0.8f,0.5f));
        //CountryManager.Create("Germany","German");
        //CountryManager.Create("France","French");

        Shield shield = ShieldManager.Create("BUCKLER.    sex sex sex ERROR ALERT!!! SEX!! SEXY SEX!","buckler","peeny",5,5,-0.3f,0.25f,0.25f);
        List<Shield> shields = new List<Shield>();
        List<Shield> emptyShields = new List<Shield>();
        //shields.Add(shield);
        List<Weapon> sampleBow = new List<Weapon>();
        sampleBow.Add(WeaponManager.WeaponList.Find(x => x.Type == WeaponType.Ranged));

        //TroopTypeManager.Create("Archer", "", "sexysexsex", CountryManager.Get("France"), sampleBow, sampleweapons, armors, shields, 0, 2, 3, 5, true, AnimalManager.AnimalList[0], new List<AnimalArmor>() { AnimalArmorManager.AnimalArmorList[0] });
        //TroopTypeManager.Create("Swordsman", "", "sexiersexsex", CountryManager.Get("Germany"), sampleweapons, new List<Weapon>(), armors, emptyShields, 3, 5, 0, 2, false, null, null);
        sampleProjectiles.Add(ProjectileManager.ProjectileList[Random.Range(0, ProjectileManager.ProjectileList.Count)]);
        //sampleProjectiles.Add(ProjectileManager.Create("arrow","yup","This is an arrow thta doesn't exist.","Bow",3f,DamageType.Sharp,0,false));
    }

#if UNITY_EDITOR
    [MenuItem("Developer/Kill All Pawns")]
    public static void thing()
    {
        PawnManager.GetAll().ForEach(p => Destroy(p.gameObject));
    }
#endif
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            pManager.CreatePawn(CountryManager.Get("Germany"), CachedItems.RandomName, TroopTypeManager.Get("Swordsman"),
                         RegimentManager.Get(0), cam.ScreenToWorldPoint(Input.mousePosition)); // make sure this id is right!
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            pManager.CreatePawn(CountryManager.Get("France"), CachedItems.RandomName, TroopTypeManager.Get("Archer"), 
                        RegimentManager.Get(1), cam.ScreenToWorldPoint(Input.mousePosition), sampleProjectiles);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            var go = Instantiate(WCMngr.I.firePrefab);
            Vector2Int p = Vector2Int.FloorToInt(cam.ScreenToWorldPoint(Input.mousePosition));
            go.transform.position = new Vector3(p.x, p.y, -2);
        }
    }
}
