using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Linq;

using Random = UnityEngine.Random;

using Countries;
using TroopTypes;
using Regiments;
using Weapons;
using Armors;
using Projectiles;
using Shields;

/// <summary>
/// I tried to make this OOP but I couldn't.
/// and by try i mean 5 minutes
/// </summary>
namespace PawnFunctions
{
    public enum ActiveWeapon
    {
        Primary,
        Secondary
    }
    public class Pawn : MonoBehaviour
    {
        /* TODO: ADD DEFAULT PAWN LEVEL VAR HERE
         * 
         * 
         * 
         * 
         */

        //        [HideInInspector]
        //        public GameObject pawnObject;
        //public SpriteRenderer pawnDarkener; // this is bad right
        [SerializeField]
        public HealthSystem healthSystem;
        public SpriteRenderer sprite;

        public bool pawnSelected = false;
        public static bool mouseOverPawn = false;

        [HideInInspector] public List<ActionType> actionTypes = new List<ActionType>();

        public int meleeSkill;
        public int rangeSkill;

        public bool active { get; set; } // if the pawn is active (not dead lol)

        public Weapon heldPrimary;
        public Weapon heldSidearm;
        public Weapon activeWeapon;
        public ActiveWeapon activeWeaponSlot;

        public bool hasSidearm;
        public bool hasPrimary;

        // c# 7 moment
        //public List<(Projectile projectile ,int count)> inventory = new List<(Projectile,int)>();
        public List<Projectile> inventory = new List<Projectile>();

        public List<Armor> armor = new List<Armor>(); // probably gonna regret doing it like this
        //public float moveSpeed = 1f; // make this update with armor

        public Country country;
        public string pname;
        public TroopType troopType;
        public Regiment regiment;
        public bool dead = false;
        public bool pawnDowned = false;

        public bool hasShield;
        public Shield shield;

        public List<Country> enemyCountries = new List<Country>();

        private void OnTriggerEnter2D(Collider2D collision)     // TODO: FIX???????? 
        { // todo: add returns to save a few cycles?
            if (collision.gameObject.GetComponent<BoxSelection>()) // fairly sure this is better to do by just checking the name
            {
                pawnSelected = true;
                //pawnDarkener.forceRenderingOff = false;
                
                if (!Player.selectedPawns.Contains(this) && BoxSelection.mode == 0) // normal
                {
                    Player.selectedPawns.Add(this);
                    if (country == Player.playerCountry)
                    {
                        Player.ourSelectedPawns.Add(this);
                    }
                }
                if (Player.selectedPawns.Contains(this) && BoxSelection.mode == 2) // subtract
                {
                    Player.selectedPawns.Remove(this);
                    if (country == Player.playerCountry)
                    {
                        Player.ourSelectedPawns.Remove(this);
                    }
                }
                if (BoxSelection.mode == -1) // delete all
                {
                    Player.selectedPawns = null; // this way itll shit out a fucking error when isee my bad programming
                    Player.ourSelectedPawns = null;
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)     // TODO: FIX????????
        {
            if (collision.gameObject.GetComponent<BoxSelection>())
            {
                pawnSelected = false;
                if (BoxSelection.letGo)
                {
                    Player.selectedPawns.Remove(this);
                    if (country == Player.playerCountry)
                    {
                        Player.ourSelectedPawns.Remove(this);
                    }
                }
            }
        }

        private void OnMouseDown()
        {
            mouseOverPawn = true;
        }
        private void OnMouseUp()
        {
            mouseOverPawn = false;
        }
    }
}
