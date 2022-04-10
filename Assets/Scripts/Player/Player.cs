using Countries;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Regiments;
using UnityEngine.Tilemaps;
using PawnFunctions;

public class Player : MonoBehaviour
{
    public static Country playerCountry = CountryManager.Get("France");
    public static bool isFollowingCursor = false;

    public static List<Regiment> playerRegiments = new List<Regiment>();
    public static List<Pawn> selectedPawns = new List<Pawn>();
    public static List<Pawn> ourSelectedPawns = new List<Pawn>();

    public static List<TileBase> selectedTiles = new List<TileBase>();
    public static List<Vector3> selectedTilePoses = new List<Vector3>();
    public static List<BoundsInt> selectedTileBounds = new List<BoundsInt>();

    public static int regimentSelectNumber;

    private void Awake()
    {
        playerCountry = CountryManager.Get("France"); // AAAA! todo! make dyanimc. his is the source of bugs if i dont remember
    }

    private void Update()
    {   
        if (isFollowingCursor) // i dont like it but it has to be done
        {
            if (Input.GetMouseButtonDown(1)) // todo: make this clear in some way. and keybinds
            {
                isFollowingCursor = false;
                print("Cancelled follow cursor");
            }
        }
    }
}
