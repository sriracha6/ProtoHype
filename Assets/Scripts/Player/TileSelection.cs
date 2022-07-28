using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PawnFunctions;
using UnityEngine.Tilemaps;

public class TileSelection : MonoBehaviour
{
    public LineRenderer lineRenderer;
    private Vector2 initialMousePos;
    private Vector2 currentMousePos;
    private BoxCollider2D bcollider;
    public Camera maincam;
    public Tilemap tmap;
    private BoundsInt area;
    private List<Vector2Int> selectedPoints = new List<Vector2Int> ();

    public static bool started;
    public static bool isBulkMode;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 4;
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(Keybinds.RightMouse) && !Input.GetKey(Keybinds.bulkTileSelect) && !started && !Pawn.mouseOverPawn && !UIManager.mouseOverUI && !BoxSelection.started && Time.timeScale > 0)
        {
            lineRenderer.positionCount = 0;
            lineRenderer.loop = false;
            selectedPoints.Clear();
            started = true;
            isBulkMode = false;
        }
        if(Input.GetMouseButton(Keybinds.RightMouse) && !Input.GetKey(Keybinds.bulkTileSelect) && started && !isBulkMode)
        {
            var c = maincam.ScreenToWorldPoint(Input.mousePosition);
            currentMousePos = new Vector2(Mathf.Ceil(c.x), Mathf.Ceil(c.y));

            if (!selectedPoints.Contains(new Vector2Int((int)currentMousePos.x, (int)currentMousePos.y)))
            {
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, currentMousePos);
                selectedPoints.Add(new Vector2Int((int)currentMousePos.x, (int)currentMousePos.y));
            }
        }
        if(Input.GetMouseButtonUp(Keybinds.RightMouse) && !Input.GetKey(Keybinds.bulkTileSelect) && started && !isBulkMode)
        {
            started = false;
            lineRenderer.positionCount = 0;

            if (!Input.GetKey(Keybinds.SelectAdd))
                Player.selectedTilePoses.Clear();
            
            foreach (Vector2Int p in selectedPoints)
                Player.selectedTilePoses.Add(new Vector3(p.x, p.y));
        }

        // --- BULK --- //

        if (Input.GetMouseButtonDown(Keybinds.RightMouse) && Input.GetKey(Keybinds.bulkTileSelect) && !started && !Pawn.mouseOverPawn && !UIManager.mouseOverUI && !BoxSelection.started && Time.timeScale > 0)
        {
            lineRenderer.loop = true;
            isBulkMode = true;
            started = true;
            lineRenderer.positionCount = 4;
            initialMousePos = maincam.ScreenToWorldPoint(Input.mousePosition);
            lineRenderer.SetPosition(0, new Vector3(initialMousePos.x, initialMousePos.y, -1)); // this can be better, right?
            lineRenderer.SetPosition(1, new Vector3(initialMousePos.x, initialMousePos.y, -1));
            lineRenderer.SetPosition(2, new Vector3(initialMousePos.x, initialMousePos.y, -1));
            lineRenderer.SetPosition(3, new Vector3(initialMousePos.x, initialMousePos.y, -1));

            bcollider = gameObject.AddComponent<BoxCollider2D>();
            bcollider.isTrigger = true;
            bcollider.offset = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        }

        if (Input.GetMouseButton(Keybinds.RightMouse) && Input.GetKey(Keybinds.bulkTileSelect) && started && isBulkMode)
        {
            currentMousePos = maincam.ScreenToWorldPoint(Input.mousePosition);

            lineRenderer.SetPosition(0, new Vector3(Mathf.Round(initialMousePos.x), Mathf.Round(initialMousePos.y),-5));
            lineRenderer.SetPosition(1, new Vector3(Mathf.Round(initialMousePos.x), Mathf.Round(currentMousePos.y),-5));
            lineRenderer.SetPosition(2, new Vector3(Mathf.Round(currentMousePos.x), Mathf.Round(currentMousePos.y),-5));
            lineRenderer.SetPosition(3, new Vector3(Mathf.Round(currentMousePos.x), Mathf.Round(initialMousePos.y),-5));

            transform.position = (currentMousePos + initialMousePos)/2;
            transform.position = new Vector3(transform.position.x,transform.position.y,-5);
            bcollider.size = new Vector2(
                Mathf.Abs(initialMousePos.x - currentMousePos.x),
                Mathf.Abs(initialMousePos.y - currentMousePos.y));

            area = new BoundsInt(Vector3Int.FloorToInt(transform.position), Vector3Int.FloorToInt(lineRenderer.bounds.size));
        }

        if (Input.GetMouseButtonUp(Keybinds.RightMouse) && Input.GetKey(Keybinds.bulkTileSelect) && started && isBulkMode)
        {
            started = false;
            transform.position = new Vector3(0, 0, -1);
            Player.selectedTileBounds.Add(area);
            GetTilesInArea();
            Destroy(bcollider);
            MoveControls.toggleMoveButton(Player.selectedTiles.Count > 1);
            lineRenderer.positionCount = 0;
        }

        void GetTilesInArea()
        {
            print("Selected Tiles: " + area);
            int it = 0;
            if (!Input.GetKey(Keybinds.SelectAdd))
                Player.selectedTilePoses.Clear();
            for (int y = tmap.cellBounds.min.y; y < tmap.cellBounds.max.y; y++)
            {
                for (int x = tmap.cellBounds.min.x; x < tmap.cellBounds.max.x; x++)
                {
                    Vector3Int p = new Vector3Int(x, y, 0);
                    if (bcollider.bounds.Contains(new Vector3(x, y, -1)))
                    {
                        it++;
                        Player.selectedTilePoses.Add(p);
                    }
                }
            }
            Debug.Log($"IT:<color=magenta>{it}</color>");
        }
    }
}

