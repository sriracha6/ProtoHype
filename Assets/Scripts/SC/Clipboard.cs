using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using PawnFunctions;

public class Clipboard : MonoBehaviour
{
    public List<(List<Build> builds, Vector2 position)> tileClipboard = new List<(List<Build>, Vector2)>();
    public List<(Pawn p, Vector2 position)> pawnClipboard = new List<(Pawn, Vector2)>();

    protected void Update()
    {
        if(Input.GetKey(Keybinds.SubtractSelection) && Input.GetKeyDown(KeyCode.C))
        {
            tileClipboard.Clear();
            pawnClipboard.Clear();
            foreach(Vector2 v in Player.selectedTilePoses)
                tileClipboard.Add((TilemapPlace.BuildsAt((int)v.x/2, (int)v.y/2), (v/2) - (Player.tileSelectStartPos / 2)));

            foreach(Pawn p in Player.selectedPawns)
                pawnClipboard.Add((p, p.transform.position));
        }

        if (Input.GetKey(Keybinds.SubtractSelection) && Input.GetKeyDown(KeyCode.V))
        {
            Vector3 mp = WCMngr.I.mainCam.ScreenToWorldPoint(Input.mousePosition) / 2;
            Vector2Int mousePos = new Vector2Int((int)mp.x, (int)mp.y);

            foreach (var s in tileClipboard)
                TilemapPlace.SetAll(s.builds, mousePos.x + (int)s.position.x, mousePos.y + (int)s.position.y);
            
            foreach((Pawn p, Vector2 position) in pawnClipboard)
                PawnManager.I.CreatePawn(false, p.country, CachedItems.RandomName, p.regiment, p.transform.position, p.inventory, p.heldPrimary, p.heldSidearm, p.shield, p.animal != null ? p.animal.sourceAnimal : null, p.armor);
        }

        if(Input.GetKey(Keybinds.SubtractSelection) && Input.GetKeyDown(KeyCode.S))
        {
            SaveScenario.SaveScenarioToPath(Application.persistentDataPath + "\\scenario\\" + Menus.I.currentScenarioFilename + ".xml");
        }
    }
}
