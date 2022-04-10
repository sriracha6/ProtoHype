using PawnFunctions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

/// <summary>
/// drag on pawns to move them : <b>if you can't take that it doesn't draw a line for each person, suffer. maybe i'll make a box for where they end up?</b>
/// </summary>
public class QuickMove : MonoBehaviour
{
    [SerializeField] Camera mainCam;
    LineRenderer lineRenderer;

    Vector2Int lastPosition;

    List<Vector2Int> targetedPositions = new List<Vector2Int>();
    //List<List<Vector2Int>> pawnMovePositions = new List<List<Vector2Int>>();

    bool shouldGo;

    float startheldTime;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        // todo: keybinds
        // && Time.time-startheldTime >= 100) // ms??
        if (Input.GetMouseButtonDown(0)) // first check: mouse down, not over UI, selected pawns, and over a pawn 
        {
            Vector2 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Linecast(mousePos, Vector3.one);
            if (hit.transform.gameObject.TryGetComponent(out Pawn p)
                && Player.ourSelectedPawns.Count > 0
                /*&& !UIManager.mouseOverUI*/)
            {
                startheldTime = Time.time;
                shouldGo = true;
                lineRenderer.loop = false;
            }
        }

        if(Input.GetMouseButton(1))
        {
            targetedPositions.Clear();

            shouldGo = false;
            startheldTime = 0f;
        }

        if (Input.GetMouseButton(0) && shouldGo)
        {
            updateLine(new Vector2Int((int)Input.mousePosition.x, (int)Input.mousePosition.y));
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (targetedPositions.Count > 0)
                addActionToPawns();

            targetedPositions.Clear();

            shouldGo = false;
            startheldTime = 0f;
        }
    }

    private void updateLine(Vector2Int position)
    {
        Vector2Int tilePos = Vector2Int.FloorToInt(mainCam.ScreenToWorldPoint(new Vector2(position.x, position.y)));
        if (!targetedPositions.Contains(tilePos)) // this fucking works
        {
            targetedPositions.Add(tilePos);

            lineRenderer.positionCount++;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, new Vector3(tilePos.x + 0.5f, tilePos.y + 0.5f, -5));
        }
        // VVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV ALERT!!! THIS IS A DEMONIC PILE OF SHIT!! IT DOESNT WORK!!! SUFFER IF OYU WANT TO BE ABLE TO BACKTRACK! YOU YOU! IT WONT WORK ITS NOT MY FAULT!
        /*else if (targetedPositions.Contains(tilePos))
        {
            Vector3[] tiles = new Vector3[targetedPositions.Count-1];

            for(int i = 0; i < tiles.Length; i++)
            {
                tiles[i] = new Vector3(targetedPositions[i].x, targetedPositions[i].y);
            }
            targetedPositions.Remove(tilePos);
            
            lineRenderer.SetPositions(shift(tiles, new Vector3(tilePos.x, tilePos.y, 0)));
        }*/
    }

    private Vector3[] shift(Vector3[] points,Vector3 tile)
    {
        Vector3[] temp = new Vector3[Math.Max(points.Length-1,0)];
        /*for(int i = 0; i<temp.Length;i++)
        {
            if (points[i] != new Vector3(tile.x, tile.y, 0))
                temp[i] = points[i];
            else
                Debug.Log("found");
        }*/
        temp = points.Where(x=>x!=tile).ToArray();
        //temp = points.ToList();
        //temp.Remove(tile);
        Debug.Log($"{points.Contains(tile)} | {temp.Contains(tile)}");
        return temp;
    }

    private void addActionToPawns()
    {
        foreach(Vector2Int v in targetedPositions)
        {
            var cc = PathfindExtra.FindNearest(Vector2Int.FloorToInt(v));
            ActionType action = new ActionType("Move", true,new Vector2Int(cc.x, cc.y), false);  
            foreach(Pawn p in Player.ourSelectedPawns)
            {
                p.actionTypes.Add(
                    new ActionType("Move",true,
                    PathfindExtra.FindNearest(action.positionTarget),
                    false));
            }
        }
    }
}
