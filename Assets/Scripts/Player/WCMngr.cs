using PawnFunctions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Weapons;

public class WCMngr : MonoBehaviour // thsi class name gets a cool icon
{
    public static WCMngr I { get; private set; }

    [HideInInspector] public Camera mainCam;
    [HideInInspector] public Weapon flagWeapon { get; private set; }
    [SerializeField] Sprite flagSprite;

    [Header("Groups")]
    public PawnInfo pawnInfo;
    public GameObject bloodParent;
    public GameObject projectileParent;

    [Header("Tilemaps")]
    public Tilemap groundTilemap;
    public Tilemap solidTilemap;

    [Header("Texture")]
    public Texture2D defaultPawnTexture;

    protected void Awake()
    {
        mainCam = Camera.main;
        flagWeapon = new Weapon("","Flag", WeaponType.Melee, "A flag carried by heroic flagbearers. Boosts troops when the bearer is still alive. Not an effective weapon.", "Flag", MeleeRange.Medium, false, 0, 0, 1.25f, null);
        
        CachedItems.renderedWeapons.Add(new CachedItems.RenderedWeapon(flagSprite, flagWeapon.ID));
        I = this;
    }

    protected void Start()
    {
        PathfindExtra _ = new PathfindExtra(); // singleton :)
    }
}
