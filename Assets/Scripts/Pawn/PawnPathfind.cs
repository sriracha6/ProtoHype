using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Countries;
using PawnFunctions;
using UnityEngine.Tilemaps;
using System;
using Body;
using Random = UnityEngine.Random;

public enum PawnOrientation
{
    Left,
    Right,
    Up,
    Down
}
public class PawnPathfind : MonoBehaviour
{
    [Header("Info")]
    public float speed;
    public float nextWaypointDistance = 0.1f;
    private string moveOption;
    public bool shouldPath;

    public PawnOrientation orientation = PawnOrientation.Right;

    public float runAndGunEffectToSpeed = 0.75f;
    [HideInInspector] public bool isRunAndGun = true; // todo: start value of this in settings

    private bool justTileFound = false;

    //private ActionType moveAction = MoveControls.actionType;

    public bool isMoving;

    Path path;
    int currentWaypoint = 0;
    bool reached = false;
    [Space]
    
    [Header("Components/Debug")]
    [SerializeField] private Seeker seeker;
    [SerializeField] private Rigidbody2D rb;

    public Transform target;
    public Tilemap tmap; // i dont like that i have to do this but some things just have to happen
    [SerializeField]
    private Camera cam;

    public Animator animator;

    [SerializeField]
    Pawn p;

    void Start()
    {
        cam = WCMngr.I.mainCam;
        if (p.isFlagBearer)
            speed *= 1.2f;
        InvokeRepeating(nameof(UpdatePath), 0f, 1f); // this is a cool fucktion
    }

    private void Update()
    {
        if (p.pawnDowned)
        {
            shouldPath = false;
            return;
        }

        if (FireManager.firePositions.Count > 0)
            if (FireManager.firePositions.AsReadOnly().Contains(new Vector2((int)transform.position.x, (int)transform.position.y)))
            {
                animator.Play("Recoil");
                p.healthSystem.TakeBurn(Random.Range(4, 9)); // i wish i could make it based of size but who gives 2 shits
                p.animal.TakeBurn(Random.Range(4, 9));
            }

        //Debug.Log(path == null);

        //Debug.Log(p.actionTypes.Count, gameObject);
        if (p.actionTypes == null || p.actionTypes.Count<=0)
        {
            shouldPath = false;
            isMoving = false;
            return;
        }
        if (p.actionTypes[0].shouldMove)
        {
            shouldPath = true;
            isMoving = true;
        }
        else
        {
            shouldPath = false;
            //path = null;
            isMoving = false;
        }
        if (p.actionTypes[0].Type.Equals("RunAndGun"))
        {
            shouldPath = true;
            isRunAndGun = p.actionTypes[0].toggle;  
        }
    }

    void UpdatePath()
    {
        if(p.actionTypes.Count<=0)
            return;
        if (p.regiment.flagBearer.dead)
            speed *= 0.8f; // lazy moment!

        if (seeker.IsDone())
        {
            if ((p.actionTypes[0].Type.Equals("SearchAndDestroy") || p.actionTypes[0].Type.Equals("Attack")))
            {
                target = p.enemyCountries.Count > 1
                    ? GetClosestEnemy(p.enemyCountries[Random.Range(0, p.enemyCountries.Count)].memberTransforms)
                    : GetClosestEnemy(p.enemyCountries[0].memberTransforms);
                seeker.StartPath(rb.position, target.position, OnPathComplete); // what the fuck?
            }
            // move normally
            if (p.actionTypes[0].Type.Equals("Move"))
            {
                // THIS IS PROBABLY GOING TO CAUSE ISSUES
                Vector3 t = PosFromMove(p.actionTypes[0].positionTarget);
                if (t != Vector3.negativeInfinity)
                    seeker.StartPath(rb.position, t, OnPathComplete);
            }
            if (Player.isFollowingCursor)
            {
                // this is going to cause issues
                Vector2 m = cam.ScreenToWorldPoint(Input.mousePosition);
                seeker.StartPath(rb.position, m, OnPathComplete);
            }
        }
    }

    void OnPathComplete(Path pa)
    {
        if (!pa.error)
        {
            path = pa;
            currentWaypoint = 0;
        }
        Vector3Int pos = WCMngr.I.groundTilemap.WorldToCell(rb.position);
            PathfindExtra.SetFree(pos.x,pos.y);
            justTileFound = false;
        //}

        //if (p.actionTypes.Count > 0)
        //{
        //    Debug.Log($"{currentWaypoint} >= {path.vectorPath.Count}");
        //    p.actionTypes.RemoveAt(0); // make it always 0
        //}
    }

    void FixedUpdate()
    {
        if (!shouldPath)
            return;

        if (path == null)
            return;

        if (currentWaypoint >= path.vectorPath.Count)
        {
            reached = true;
            if (p.actionTypes.Count > 0)
                p.actionTypes.RemoveAt(0); // make it always 0
            currentWaypoint = 0;
            return;
        }
        else
            reached = false;

        Vector2 dir = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = (p.healthSystem.GetVital(VitalSystem.Moving) * 2) * Time.fixedDeltaTime * dir;

        rb.MovePosition(rb.position + force);
        Vector2 distanceVector = rb.position - (Vector2)path.vectorPath[currentWaypoint];
        float distance = distanceVector.sqrMagnitude;
        Debug.DrawLine(rb.position, path.vectorPath[currentWaypoint], Color.magenta, Time.fixedDeltaTime, false);
        //float distance = (rb.position - (Vector2)path.vectorPath[currentWaypoint]);//.sqrMagnitude;
        // bruh                 V this cause a lot of issues :/
        if (distance < nextWaypointDistance * nextWaypointDistance && currentWaypoint <= path.vectorPath.Count)
            currentWaypoint++; // wtf?
                               // THIS FIXES A FUCKING PATHFINDING SLOWDOWN BUG AND IT OPTIMIZES THE GAME BY
                               // REMOVE A SQRT CALL??? FUCK YEAH THAT WAS WORTH 4 HOURS HHOLY SHIT!!!

        speed *= TilemapPlace.tilemap[(int)transform.position.x, (int)transform.position.y].walkSpeed;

        // crisis averted. instead of using rb velocity, we use the dir variable. duh. my god.
        #region Direction
        if (dir.x > 0)
        {
            if (orientation == PawnOrientation.Up)
                animator.Play("UpToSide", 0);
            transform.localScale = new Vector3(1f, 1f, 1f);
            orientation = PawnOrientation.Right;
        }
        else if (dir.x < 0)
        {
            if (orientation == PawnOrientation.Up)
                animator.Play("UpToSide", 0);
            transform.localScale = new Vector3(-1f, 1f, 1f);
            orientation = PawnOrientation.Left;
        }
        if (dir.y > 0 && dir.x == 0)
        {
            if (orientation != PawnOrientation.Up)
                animator.Play("SideToUp");
            orientation = PawnOrientation.Up;
        }
        else if (dir.y < 0)
        {
            if (orientation != PawnOrientation.Down)
                animator.Play("UpToSide", 0);
            orientation = PawnOrientation.Down;
        }
        #endregion
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    //UpdatePath();
    //    //Vector3 t = GetClosestEnemyV3(Player.selectedTilePoses);
    //    //seeker.StartPath(rb.position, t, OnPathComplete);
    //}

    #region                                  Closest Enemy
    Transform GetClosestEnemy(List<Transform> enemies)
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (Transform potentialTarget in enemies.AsReadOnly())
        {
            Vector3 directionToTarget = potentialTarget.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }

        return bestTarget;
    }
    Vector3 GetClosestEnemyV3(List<Vector3> enemies)
    {
        Vector3 bestTarget = new Vector3(0,0,0);
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (Vector3 potentialTarget in enemies.AsReadOnly())
        {
            Vector3 directionToTarget = potentialTarget - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }

        return bestTarget;
    }
    Vector3 PosFromMove(Vector2Int position)
    {
        //Vector3Int ntile = WCMngr.I.groundTilemap.WorldToCell((Vector3Int)position);
        Vector3Int ntile = new Vector3Int(position.x, position.y, 0);
        //if (!PathfindExtra.PresentAt(ntile.x, ntile.y))
        //{
        //    PathfindExtra.SetUsed(ntile.x, ntile.y);
        //    justTileFound = true;
        //    return ntile;
        //}
        Debug.Log($"stuffs happening here dont delete this line : {position}");
        PathfindExtra.SetUsed(ntile.x, ntile.y);
        justTileFound = true;
        return new Vector3(position.x, position.y); // i dont know wtf else to put here but this'll definitely cause issues
    }
    #endregion
}
