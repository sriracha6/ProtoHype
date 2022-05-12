using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PawnFunctions;
using System;
using System.Linq;

/// <summary>
/// This code is unbelievably poorly made. This will break something in the future. This will probably make the game slower.
/// </summary>
public enum SelectionMode { Default, Subtract, ClearAll }
public class BoxSelection : MonoBehaviour
{
    public LineRenderer lineRenderer;
    private Vector2 initialMousePos;
    private Vector2 currentMousePos;
    private BoxCollider2D bcollider;
    [SerializeField] private Camera maincam;
    [SerializeField] private SpriteRenderer boxFill;
    
    public static List<Pawn> newSelectedPawns = new List<Pawn>();

    public static bool letGo;
    public static SelectionMode mode;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl))
            mode = SelectionMode.Subtract; 
        else if (Input.GetMouseButtonDown(0) && Pawn.mouseOverPawn)
            mode = SelectionMode.ClearAll;
        else
            mode = SelectionMode.Default; // normal

        if (Input.GetMouseButtonDown(0) && !Pawn.mouseOverPawn)
        {
            Player.selectedTiles.Clear();
            // WHY THE FUCK WERE THESE EVEN HERE???S???? FUCK YOU!!
            //Player.selectedPawns.Clear();
            //Player.ourSelectedPawns.Clear();

            boxFill.forceRenderingOff = false;

            lineRenderer.loop = true;
            lineRenderer.positionCount = 4;
            initialMousePos = maincam.ScreenToWorldPoint(Input.mousePosition);
            lineRenderer.SetPosition(0, new Vector3(initialMousePos.x, initialMousePos.y, -5));
            lineRenderer.SetPosition(1, new Vector3(initialMousePos.x, initialMousePos.y, -5));
            lineRenderer.SetPosition(2, new Vector3(initialMousePos.x, initialMousePos.y, -5));
            lineRenderer.SetPosition(3, new Vector3(initialMousePos.x, initialMousePos.y, -5));

            bcollider = gameObject.AddComponent<BoxCollider2D>();
            bcollider.isTrigger = true;
            letGo = false;
            bcollider.offset = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            // todo:keybinds
        }

        if (Input.GetMouseButton(0) && !Pawn.mouseOverPawn && !Input.GetMouseButton(1))
        {
            currentMousePos = maincam.ScreenToWorldPoint(Input.mousePosition);

            lineRenderer.SetPosition(0, new Vector3(initialMousePos.x, initialMousePos.y, -5));
            lineRenderer.SetPosition(1, new Vector3(initialMousePos.x, currentMousePos.y, -5));
            lineRenderer.SetPosition(2, new Vector3(currentMousePos.x, currentMousePos.y, -5));
            lineRenderer.SetPosition(3, new Vector3(currentMousePos.x, initialMousePos.y, -5));

            //                              V   why does this work? wtf?
            transform.localScale = new Vector2(initialMousePos.x - currentMousePos.x, initialMousePos.y - currentMousePos.y);
            letGo = false;

            transform.position = (currentMousePos + initialMousePos) /2;
            transform.position = new Vector3(transform.position.x, transform.position.y, -5);

            bcollider.size = new Vector2(
                Mathf.Abs(initialMousePos.x - currentMousePos.x),
                Mathf.Abs(initialMousePos.y - currentMousePos.y));
        }

        if (Input.GetMouseButtonUp(0) && !UIManager.mouseOverUI)
        {
            lineRenderer.positionCount = 0;
            Destroy(bcollider);
            boxFill.forceRenderingOff = true;

            transform.position = new Vector3(0, 0, -5);
            if (!Pawn.mouseOverPawn)
                letGo = true;

            if (newSelectedPawns.Count > 0)
            {
                if (Input.GetKey(KeyCode.LeftShift)) // todo keybinds                  // over write
                {
                    Player.selectedPawns.AddRange(newSelectedPawns);
                    Player.ourSelectedPawns.AddRange(newSelectedPawns.Where(x => x.country == Player.playerCountry).ToList());
                }
                else                                                          // over write
                {
                    Player.selectedPawns = newSelectedPawns;
                    Player.ourSelectedPawns = newSelectedPawns.Where(x => x.country == Player.playerCountry).ToList();
                }
                MoveControls.showPanel();
            }
            newSelectedPawns.Clear();
            
            print("Selected Pawns: " + Player.selectedPawns.Count +
    "\nOUR Seleted Pawns (haha funny USSR): " + Player.ourSelectedPawns.Count);
            //Debug.Log($"<b>Mouse Up | Clear selected pawns</b> : {Player.ourSelectedPawns.Count}");
        }
    }
}

