using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FirePlacer : MonoBehaviour
{
    public static bool active = false;

    static List<Vector2Int> poses = new List<Vector2Int>();

    protected void Update()
    {
        if (!active) return;
        Vector2 p = WCMngr.I.mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int pp = new Vector2Int((int)p.x, (int)p.y);

        if (Input.GetMouseButton(Keybinds.LeftMouse) && !poses.Contains(pp))
        {
            var go = Instantiate(WCMngr.I.firePrefab);
            poses.Add(pp);
            go.transform.position = new Vector3(pp.x, pp.y, -2);
        }
        else if (Input.GetMouseButton(Keybinds.LeftMouse) && poses.Contains(pp))
        {
            FireManager.fires.Find(x => new Vector2Int((int)x.transform.position.x, (int)x.transform.position.y) == pp).Size++;
        }
    }
}
