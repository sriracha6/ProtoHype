using PawnFunctions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Weapons;

public class WCMngr : MonoBehaviour
{
    public static WCMngr I { get; private set; }

    public Camera mainCam;
    public Weapon flagWeapon { get; private set; }
    [SerializeField] Sprite flagSprite;
    public Difficulty difficulty;

    [Header("Groups")]
    public PawnInfo pawnInfo;
    public GameObject bloodParent;
    public GameObject projectileParent;

    [Header("Tilemaps")]
    public Tilemap groundTilemap;
    public Tilemap solidTilemap;

    [Header("Texture")]
    public Texture2D defaultPawnTexture;

    public GameObject flagPrefab;
    public GameObject firePrefab;
    public RuleTile mountainTile;
    public Tile testTile;
    public GameObject testMat;

    public List<string> bluntWoundNames;
    public List<string> seriousBluntWoundNames;

    protected void Awake()
    {
        mainCam = Camera.main;
        flagWeapon = new Weapon("", "Flag", WeaponType.Melee, "A flag carried by heroic flagbearers. Boosts troops when the bearer is still alive. Not an effective weapon.", "Flag", new MeleeRange(MeleeRange.Medium), false, 0, 0, 1.25f, null);

        CachedItems.renderedWeapons.Add(new CachedItems.RenderedWeapon(flagSprite, flagWeapon.ID));
        if (I == null)
            I = this;
        else
        {
            I.groundTilemap = groundTilemap;
            I.solidTilemap = solidTilemap;
            I.projectileParent = projectileParent;
            I.bloodParent = bloodParent;
            I.pawnInfo = pawnInfo;
            I.mainCam = Camera.main;
        }

        bluntWoundNames.AddRange(new string[] { "Crack", "Fracture", "Fissure" });
        seriousBluntWoundNames.AddRange(new string[] { "Dislocation", "Break" });
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
        PathfindExtra _ = new PathfindExtra(); // singleton :)
    }
}
