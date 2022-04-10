using PawnFunctions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager2D : MonoBehaviour // thsi class name gets a cool icon
{
    public static GameManager2D Instance { get; private set; }

    [HideInInspector] public Camera mainCam;

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
        Instance = this;
    }

    protected void Start()
    {
        PathfindExtra _ = new PathfindExtra(); // singleton :)
    }
}
