using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PawnFunctions;
using System;

/// <summary>
/// This code is unbelievably poorly made. This will break something in the future. This will probably make the game slower.
/// </summary>
public class BoxSelection : MonoBehaviour
{
    public LineRenderer lineRenderer;
    private Vector2 initialMousePos;
    private Vector2 currentMousePos;
    private BoxCollider2D bcollider;
    [SerializeField] private Camera maincam;
    [SerializeField] private SpriteRenderer boxFill;
    
    public static bool letGo;
    public static int mode;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl))
            mode = 2; // subtract
        else if (Input.GetMouseButtonDown(0) && Pawn.mouseOverPawn)
            mode = -1;
        else
            mode = 0; // normal

        if (Input.GetMouseButtonDown(0) && !Pawn.mouseOverPawn)
        {
            Player.selectedTiles.Clear();
            Player.selectedPawns.Clear();
            Player.ourSelectedPawns.Clear();

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

        if (Input.GetMouseButtonUp(0))
        {
            lineRenderer.positionCount = 0;
            Destroy(bcollider);
            boxFill.forceRenderingOff = true;

            transform.position = new Vector3(0, 0, -5);
            if (!Pawn.mouseOverPawn)
                letGo = true;
            try
            {
                if (Player.selectedPawns.Count > 0) // this message is actually unbelievably annoying
                {
                    MoveControls.showPanel();

                    print("Selected Pawns: " + Player.selectedPawns.Count +
                    "\nOUR Seleted Pawns (haha funny USSR): " + Player.ourSelectedPawns.Count);
                }
                else
                    MoveControls.hidePanel();
            }
            catch (NullReferenceException e)
            {
                Debug.Log(e);
            }
        }
    }
}

