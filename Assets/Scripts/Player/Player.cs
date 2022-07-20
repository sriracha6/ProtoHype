using Countries;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Regiments;
using UnityEngine.Tilemaps;
using PawnFunctions;

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
    public static bool isRoofShow = false;

    public static List<Country> friends = new List<Country>();
    public static List<Country> enemies = new List<Country>();

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
