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
        //        [HideInInspector]
        //        public GameObject pawnObject;
        //public SpriteRenderer pawnDarkener; // this is bad right
        public HealthSystem healthSystem;
        public PawnPathfind pawnPathfind;
        public CombatSystem combatSystem;
        public PawnRenderer pawnRenderer;

        public SpriteRenderer sprite;
        public SpriteRenderer indicator;

        public bool pawnSelected = false;
        public static bool mouseOverPawn = false;
        public bool thisPawnMouseOver = false;

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

        public bool isFlagBearer = false;

        public Projectile inventory;
        public Animator animator;

        public List<Armor> armor = new List<Armor>(); // probably gonna regret doing it like this

        public Country country;
        public string pname;
        public TroopType troopType;
        public Regiment regiment;
        public bool dead = false;
        public bool pawnDowned = false;

        public bool hasShield;
        public Shield shield;
        public AnimalBehavior animal;

        public List<Country> enemyCountries = new List<Country>();
        public Color skinColor;
        public GameObject flagObject;

        public int killCount;

        protected void OnTriggerEnter2D(Collider2D collision)
        {
            if (!BoxSelection.letGo && collision.gameObject.CompareTag("BoxSelect")) // fairly sure this is better to do by just checking the name
            {
                pawnSelected = true;
                //pawnDarkener.forceRenderingOff = false;
                
                if (!BoxSelection.newSelectedPawns.Contains(this) && BoxSelection.mode == SelectionMode.Default) 
                    BoxSelection.newSelectedPawns.Add(this);
                if (BoxSelection.newSelectedPawns.Contains(this) && BoxSelection.mode == SelectionMode.Subtract) 
                    BoxSelection.newSelectedPawns.Remove(this);
                if (BoxSelection.mode == SelectionMode.ClearAll)
                {
                    BoxSelection.newSelectedPawns = null;
                    Player.selectedPawns = null; // this way itll shit out a fucking error when isee my bad programming
                    Player.ourSelectedPawns = null;
                }
            }
        }

        protected void OnTriggerExit2D(Collider2D collision)
        {
            if (BoxSelection.letGo && collision.gameObject.CompareTag("BoxSelect"))
            {
                pawnSelected = false;
                Player.selectedPawns.Remove(this);
                if (country == Player.playerCountry)
                    Player.ourSelectedPawns.Remove(this);
                BoxSelection.newSelectedPawns.Clear();
            }
        }

        protected void OnMouseEnter()
        {
            mouseOverPawn = true;
            thisPawnMouseOver = true;
        }
        protected void OnMouseExit()
        {
            mouseOverPawn = false;
            thisPawnMouseOver = false;
        }
    }
}
