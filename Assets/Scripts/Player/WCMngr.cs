using PawnFunctions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using Weapons;

public class WCMngr : MonoBehaviour
{
    public static WCMngr I { get; private set; }

    public static readonly string Version = "0.6";

    public Camera mainCam;
    public Weapon flagWeapon { get; private set; }
    [SerializeField] Sprite flagSprite;
    public Difficulty difficulty;

    [Header("Prefabs")]
    public GameObject damagePrefab;
    public GameObject rubblePrefab;
    public GameObject flagPrefab;
    public GameObject firePrefab;
    public GameObject testMat;
    public GameObject heatDistortBlock;

    [Header("Groups")]
    public GameObject bloodParent;
    public GameObject projectileParent;

    //[Header("Tilemaps")]
    public Tilemap groundTilemap { get { return MapGenerator.I.groundTmap; } }
    public Tilemap solidTilemap { get { return MapGenerator.I.solidTmap; } }
    public Tilemap roofTilemap { get { return RoofPlacer.I.roofTmap; } }

    [Header("Texture")]
    public Texture2D defaultPawnTexture;
    public Sprite miscrubbleTex;
    public Sprite stonerubbleTex;
    public Sprite woodrubbleTex;

    [Space]
    public Material iceMat;
    public RuleTile mountainTile;
    public Tile testTile;

    public List<string> bluntWoundNames;
    public List<string> seriousBluntWoundNames;

    protected void Awake()
    {
        mainCam = Camera.main;
        CachedItems.renderedWeapons.Add(new CachedItems.RenderedWeapon(flagSprite, flagWeapon));
        if (I == null)
        {
            I = this;

            flagWeapon = new Weapon("", "Flag", WeaponType.Melee, "A flag carried by heroic flagbearers. Boosts troops when the bearer is still alive. Not an effective weapon.", "Flag", new MeleeRange(MeleeRange.Medium), false, 0, 0, 1.25f, null);
            bluntWoundNames.AddRange(new string[] { "Crack", "Fracture", "Fissure" });
            seriousBluntWoundNames.AddRange(new string[] { "Dislocation", "Break" });
        }
        else if (Menus.I.inBattle || Menus.I.inSC)
        {
            I.projectileParent = projectileParent;
            I.bloodParent = bloodParent;
            I.mainCam = Camera.main;
        }
    }

    /// <summary>
    /// Do not call this to save memory. This is automatically called when the device is low on memory. Clearing cache results in much slower load times.
    /// </summary>
    public static void ClearCache()
    {
        long oldMem = System.GC.GetTotalMemory(true);
        DB.Attention($"Clearing cache due to low memory. Current memory used: {oldMem}");
        CachedItems.renderedAnimals.Clear();
        CachedItems.renderedWeapons.Clear();
        CachedItems.renderedShields.Clear();
        CachedItems.renderedArmors.Clear();
        CachedItems.renderedAnimalArmors.Clear();
        CachedItems.renderedFlora.Clear();
        CachedItems.renderedPawns.Clear();
        CachedItems.renderedProjectiles.Clear();
        CachedItems.renderedAnimalPicks.Clear();
        DB.Attention($"Memory cleared: {oldMem - System.GC.GetTotalMemory(true)}");
    }

    protected void Start()
    {
        if (Menus.I.inBattle)
        {
            new PathfindExtra(); // singleton :)
            TimeController.Speed = 0;
        }
    }
}
