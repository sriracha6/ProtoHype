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
    List<Weapon> sampleweapons = new List<Weapon>();
    List<Armor> armors = new List<Armor>();
    List<Projectile> sampleProjectiles = new List<Projectile>();

    private void Awake()
    {
        List<Attack> attks = new List<Attack>();
        attks.Add(new Attack("stab", DamageType.Sharp, false, 5)); 
        attks.Add(new Attack("slash", DamageType.Sharp, false, 10));

        //WeaponManager.CreateMelee("","Empty", WeaponType.Melee,"Empty","Empty",0f,false,0,0,0,new List<Attack>());
        armors = new List<Armor>(Armor.List);
        sampleweapons = new List<Weapon>(Weapon.List);
        //sampleweapons.Add(WeaponManager.CreateMelee("","sexknief", WeaponType.Melee, "Dagger",
        //                 "its a sex knife", MeleeRange.Far, false, 10, 0, 0.5f, attks));
        //sampleweapons.Add(WeaponManager.CreateMelee("","hugh mungus", WeaponType.Melee, "Longsword",
        //         "its a sex sword", MeleeRange.Short, false, 10, 0, 2f, attks));

        List<Bodypart> head = new List<Bodypart>();
        head.Add(Bodypart.Get("Head"));
        head.Add(Bodypart.Get("Torso"));

        //armors.Add(ArmorManager.Create("","halo","it covers your dome",150,10,5,-2,Layer.outer,false,head));
        //armors.Add(ArmorManager.Create("","idk","it is idk",150,10,5,-2,Layer.skin,false,head));

        //List<Weapon> sampleBOW = new List<Weapon>();
        //sampleBOW.Add(WeaponManager.CreateRanged("","bow","bow",WeaponType.Ranged, "Bow",
        //    25,5,RangeType.Shooter,0.5f,0f,"Sharp",5,1f,0.95f,0.8f,0.5f));
        //CountryManager.Create("Germany","German");
        //CountryManager.Create("France","French");

        Shield shield = Shield.Create("BUCKLER.    sex sex sex ERROR ALERT!!! SEX!! SEXY SEX!","buckler","peeny",5,5,-0.3f,0.25f,0.25f);
        List<Shield> shields = new List<Shield>();
        List<Shield> emptyShields = new List<Shield>();
        //shields.Add(shield);
        List<Weapon> sampleBow = new List<Weapon>();
        sampleBow.Add(Weapon.List.Find(x => x.Type == WeaponType.Ranged));

        //TroopTypeManager.Create("Archer", "", "sexysexsex", CountryManager.Get("France"), sampleBow, sampleweapons, armors, shields, 0, 2, 3, 5, true, AnimalManager.AnimalList[0], new List<AnimalArmor>() { AnimalArmorManager.AnimalArmorList[0] });
        //TroopTypeManager.Create("Swordsman", "", "sexiersexsex", CountryManager.Get("Germany"), sampleweapons, new List<Weapon>(), armors, emptyShields, 3, 5, 0, 2, false, null, null);
        sampleProjectiles.Add(Projectile.List[Random.Range(0, Projectile.List.Count)]);
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
        /*if (Input.GetKeyDown(KeyCode.Q))
        {
            PawnManager.I.CreatePawn(Country.Get("Germany"), CachedItems.RandomName, TroopType.Get("Swordsman"),
                         Regiment.Get(0), WCMngr.I.mainCam.ScreenToWorldPoint(Input.mousePosition)); // make sure this id is right!
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            PawnManager.I.CreatePawn(Country.Get("France"), CachedItems.RandomName, TroopType.Get("Archer"), 
                        Regiment.Get(1), WCMngr.I.mainCam.ScreenToWorldPoint(Input.mousePosition), sampleProjectiles);
        }*/

        if (Input.GetKeyDown(KeyCode.M))
        {
            var go = Instantiate(WCMngr.I.firePrefab);
            Vector2Int p = Vector2Int.FloorToInt(WCMngr.I.mainCam.ScreenToWorldPoint(Input.mousePosition));
            go.transform.position = new Vector3(p.x, p.y, -2);
        }
        if (Input.GetKeyDown(KeyCode.F))
            Messages.I.Add("You pressed F. Respects have been paid.");
    }
}
