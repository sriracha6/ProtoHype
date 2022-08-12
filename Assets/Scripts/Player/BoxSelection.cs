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
    public static bool started;
    public static SelectionMode mode;

    protected void Start()
    {
        lineRenderer.positionCount = 4;
    }

    protected void Update()
    {
        if (Input.GetKey(Keybinds.SubtractSelection))
            mode = SelectionMode.Subtract; 
        else if (Input.GetMouseButtonDown(Keybinds.LeftMouse) && Pawn.mouseOverPawn)
            mode = SelectionMode.ClearAll;
        else
            mode = SelectionMode.Default; // normal
        
        if (Input.GetMouseButtonDown(Keybinds.LeftMouse) && !Pawn.mouseOverPawn && !UIManager.mouseOverUI && !TileSelection.started)
        {
            Player.selectedTiles.Clear();
            // WHY THE FUCK WERE THESE EVEN HERE???S???? FUCK YOU!!
            //Player.selectedPawns.Clear();
            //Player.ourSelectedPawns.Clear();
            started = true;
            lineRenderer.positionCount = 4;
            boxFill.forceRenderingOff = false;

            lineRenderer.loop = true;
            initialMousePos = maincam.ScreenToWorldPoint(Input.mousePosition);
            lineRenderer.SetPosition(0, new Vector3(initialMousePos.x, initialMousePos.y, -5));
            lineRenderer.SetPosition(1, new Vector3(initialMousePos.x, initialMousePos.y, -5));
            lineRenderer.SetPosition(2, new Vector3(initialMousePos.x, initialMousePos.y, -5));
            lineRenderer.SetPosition(3, new Vector3(initialMousePos.x, initialMousePos.y, -5));

            bcollider = gameObject.AddComponent<BoxCollider2D>();
            bcollider.isTrigger = true;
            letGo = false;
            bcollider.offset = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        }

        if ((Input.GetMouseButton(Keybinds.LeftMouse) || Input.GetMouseButtonDown(Keybinds.RightMouse)) && started )
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

            //bcollider.size = new Vector2(
            //    Mathf.Abs(initialMousePos.x - currentMousePos.x),
            //    Mathf.Abs(initialMousePos.y - currentMousePos.y));
        }

        if ((Input.GetMouseButtonUp(Keybinds.LeftMouse) || Input.GetMouseButtonDown(Keybinds.RightMouse)) && started)
        {
            started = false;
            lineRenderer.positionCount = 0;
            Destroy(bcollider);
            boxFill.forceRenderingOff = true;

            transform.position = new Vector3(0, 0, -1);
            letGo = true;

            if (newSelectedPawns.Count > 0)
            {
                if (Input.GetKey(Keybinds.SelectAdd))
                {
                    Player.selectedPawns.AddRange(newSelectedPawns);
                    Player.ourSelectedPawns.AddRange(newSelectedPawns.FindAll(x => x.country == Player.playerCountry).ToList());
                }
                else                      // over write
                {
                    Player.selectedPawns = new List<Pawn>(newSelectedPawns);
                    Player.ourSelectedPawns = newSelectedPawns.FindAll(x => x.country == Player.playerCountry).ToList();
                }
                MoveControls.showPanel();
            }
            Player.UpdateSelectedPawnsTint();
            newSelectedPawns.Clear();
        }
    }
}

