using PawnFunctions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

/// <summary>
/// drag on pawns to move them (with shift support) : <b>if you can't take that it doesn't draw a line for each person, suffer. maybe i'll make a box for where they end up?</b>
/// </summary>
public class QuickMove : MonoBehaviour
{
    [SerializeField] Camera mainCam;
    LineRenderer lineRenderer;
    public float lineSimplification = 0.7f;

    readonly List<Vector2Int> targetedPositions = new List<Vector2Int>();
    //List<List<Vector2Int>> pawnMovePositions = new List<List<Vector2Int>>();
    readonly List<Vector3> allPoints = new List<Vector3>();
    bool shouldGo;

    protected void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    protected void Update()
    {
        // && Time.time-startheldTime >= 100) // ms??
        if (Input.GetMouseButtonDown(Keybinds.LeftMouse)) // first check: mouse down, not over UI, selected pawns, and over a pawn 
        {
            //Vector2 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

            //RaycastHit2D hit = Physics2D.Linecast(mousePos, Vector3.one);
            Pawn p = PawnManager.GetAllFriendlies().Find(x => x.thisPawnMouseOver);
            if (p != null
                && Player.ourSelectedPawns.Count > 0
                && !UIManager.mouseOverUI)
            {
                shouldGo = true;
                lineRenderer.loop = false;
            }
        }

        if(Input.GetMouseButton(Keybinds.RightMouse))
        {
            targetedPositions.Clear();
            allPoints.Clear();
            lineRenderer.positionCount = 0;

            shouldGo = false;
        }

        if (Input.GetMouseButton(Keybinds.LeftMouse) && shouldGo)
            updateLine(Vector2Int.CeilToInt(Input.mousePosition));
        
        if (Input.GetMouseButtonUp(Keybinds.LeftMouse))
        {
            if (targetedPositions.Count > 0)
                addActionToPawns();

            targetedPositions.Clear();
            allPoints.Clear();

            shouldGo = false;
        }
    }

    private void updateLine(Vector2Int position) // todo: check if the mouse is over a building (if it is, dont add it)
    {
        Vector2Int tilePos = Vector2Int.FloorToInt(mainCam.ScreenToWorldPoint(new Vector2(position.x, position.y)));

        if (!targetedPositions.Contains(tilePos)) // this fucking works
        {
            targetedPositions.Add(tilePos);

            lineRenderer.positionCount++;
            Vector3 pos = new Vector3(tilePos.x + 0.5f, tilePos.y + 0.5f, -5);
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, pos);
            
        }
        // VVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV ALERT!!! THIS IS A DEMONIC PILE OF SHIT!! IT DOESNT WORK!!! SUFFER IF OYU WANT TO BE ABLE TO BACKTRACK! YOU YOU! IT WONT WORK ITS NOT MY FAULT!
        /*else if (targetedPositions.Contains(tilePos))
        {
            targetedPositions.Remove(tilePos);
            allPoints.Remove(new Vector3(tilePos.x + 0.5f, tilePos.y + 0.5f, -5));

            Vector3[] poses = allPoints.ToArray();

            lineRenderer.positionCount--;
            lineRenderer.SetPositions(poses);
        }*/
    }

    private void addActionToPawns()
    {
        List<Vector2> simpleLine = new List<Vector2>();
        List<Vector2> linev2 = new List<Vector2>();
        foreach(Vector2Int v in targetedPositions)
        {
            linev2.Add(new Vector2(v.x, v.y));
        }

        LineUtility.Simplify(linev2, lineSimplification, simpleLine);

        if (!Input.GetKey(Keybinds.SelectAdd))
        {
            foreach (Pawn p in Player.ourSelectedPawns)
            {
                p.actionTypes.Clear();
            }
        }
        lineRenderer.positionCount = 0;
        foreach(Vector2 v in simpleLine)
        {
            //var cc = PathfindExtra.FindNearest(Vector2Int.FloorToInt(v));
            // todo ^ do we need this?
            var cc = v;
            ActionType action = new ActionType("Move", true, new Vector2Int((int)cc.x, (int)cc.y), false);  
            foreach(Pawn p in Player.ourSelectedPawns)
            {
                p.actionTypes.Add(action);
            }
        }
        targetedPositions.Clear();
        allPoints.Clear();
    }
}
