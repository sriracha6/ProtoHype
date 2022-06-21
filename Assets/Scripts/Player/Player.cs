using Countries;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Regiments;
using UnityEngine.Tilemaps;
using PawnFunctions;

public enum LoadFrom { Quickbattle }

public class Player : MonoBehaviour
{
    public static Country playerCountry;
    public static bool isFollowingCursor = false;

    public static List<Pawn> selectedPawns = new List<Pawn>();
    public static List<Pawn> ourSelectedPawns = new List<Pawn>();

    public static List<TileBase> selectedTiles = new List<TileBase>();
    public static List<Vector3> selectedTilePoses = new List<Vector3>();
    public static List<BoundsInt> selectedTileBounds = new List<BoundsInt>();

    public static int regimentSelectNumber;
    public static LoadFrom loadedFrom;

    private void Start()
    {
        playerCountry = Country.Get("France"); // AAAA! todo! make dyanimc. this is the source of bugs if i dont remember
    }

    private void Update()
    {   
        if (isFollowingCursor) // i dont like it but it has to be done
        {
            if (Input.GetMouseButtonDown(Keybinds.RightMouse)) // todo: make this clear in some way
            {
                isFollowingCursor = false;
                print("Cancelled follow cursor");
            }
        }
    }
}
