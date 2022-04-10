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

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1) && !Pawn.mouseOverPawn && !Input.GetMouseButton(0)) // TODO: keybinds
        {
            //Player.selectedPawns = null;
            // TODO: ^ this should be done better or tell the player that it doesnt work.

            lineRenderer.positionCount = 4;
            initialMousePos = maincam.ScreenToWorldPoint(Input.mousePosition);
            lineRenderer.SetPosition(0, new Vector3(initialMousePos.x, initialMousePos.y, -5)); // this can be better, right?
            lineRenderer.SetPosition(1, new Vector3(initialMousePos.x, initialMousePos.y, -5));
            lineRenderer.SetPosition(2, new Vector3(initialMousePos.x, initialMousePos.y, -5));
            lineRenderer.SetPosition(3, new Vector3(initialMousePos.x, initialMousePos.y, -5));

            bcollider = gameObject.AddComponent<BoxCollider2D>();
            bcollider.isTrigger = true;
            bcollider.offset = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        }

        if (Input.GetMouseButton(1) && !Pawn.mouseOverPawn && !Input.GetMouseButton(0)) //todo:keybinds
        {
            currentMousePos = maincam.ScreenToWorldPoint(Input.mousePosition);

            lineRenderer.SetPosition(0, new Vector3(Mathf.Round(initialMousePos.x), Mathf.Round(initialMousePos.y),-5));
            lineRenderer.SetPosition(1, new Vector3(Mathf.Round(initialMousePos.x), Mathf.Round(currentMousePos.y),-5)); // can this be removed???
            lineRenderer.SetPosition(2, new Vector3(Mathf.Round(currentMousePos.x), Mathf.Round(currentMousePos.y),-5)); // TODO: this is bad
            lineRenderer.SetPosition(3, new Vector3(Mathf.Round(currentMousePos.x), Mathf.Round(initialMousePos.y),-5));

            transform.position = (currentMousePos + initialMousePos)/2;
            transform.position = new Vector3(transform.position.x,transform.position.y,-5);
            //transform.position = currentMousePos;
            bcollider.size = new Vector2(
                Mathf.Abs(initialMousePos.x - currentMousePos.x),
                Mathf.Abs(initialMousePos.y - currentMousePos.y));

            area = new BoundsInt(Vector3Int.FloorToInt(transform.position), Vector3Int.FloorToInt(lineRenderer.bounds.size));
        }

        if (Input.GetMouseButtonUp(1))
        {
            Player.selectedTileBounds.Add(area);
            GetTilesInArea();
            lineRenderer.positionCount = 0;
            Destroy(bcollider);
            transform.position = new Vector3(0, 0, 10);
        }

        void GetTilesInArea()
        {
            print("Selected Tiles: " + area);
            //Player.selectedTiles = tmap.GetTilesBlock(area);
            for (int y = tmap.cellBounds.min.y; y < tmap.cellBounds.max.y; y++)
            {
                for (int x = tmap.cellBounds.min.x; x < tmap.cellBounds.max.x; x++)
                {
                    Vector3Int p = new Vector3Int(x, y, 0);
                    if (bcollider.bounds.Contains(p))
                    {
                        //TileBase t = tmap.GetTile(tmap.WorldToCell(p));
                        //Player.selectedTiles.Add(t);
                        Player.selectedTilePoses.Add(p);
                    }
                }
            }
            //print(Player.selectedTiles.Count);
        }
    }
}

