using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clipboard : MonoBehaviour
{
    public List<(List<Build> builds, Vector2 position)> clipboard = new List<(List<Build> builds, Vector2 position)>();

    protected void Update()
    {
        if(Input.GetKey(Keybinds.SubtractSelection) && Input.GetKeyDown(KeyCode.C))
        {
            clipboard.Clear();
            foreach(Vector2 v in Player.selectedTilePoses)
                clipboard.Add((TilemapPlace.BuildsAt((int)v.x/2, (int)v.y/2), (v/2) - (Player.tileSelectStartPos / 2)));
        }

        if (Input.GetKey(Keybinds.SubtractSelection) && Input.GetKeyDown(KeyCode.V))
        {
            Vector3 mp = WCMngr.I.mainCam.ScreenToWorldPoint(Input.mousePosition) / 2;
            Vector2Int mousePos = new Vector2Int((int)mp.x, (int)mp.y);
            Debug.Log($"{mousePos}");

            foreach (var s in clipboard)
                TilemapPlace.SetAll(s.builds, mousePos.x + (int)s.position.x, mousePos.y + (int)s.position.y);
        }
    }
}
