using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PawnFunctions;
using Countries;
using TroopTypes;
using UnityEditor;
using Regiments;
using Weapons;
using Attacks;
using Animals;
using Armors;
using Projectiles;
using XMLLoader;
using Shields;
using Body;
using Nature;

public class DevTools : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Developer/Kill All Pawns")]
    public static void thing()
    {
        PawnManager.GetAll().ForEach(p => Destroy(p.gameObject));
    }
#endif
    protected void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Q))
        {
            PawnManager.I.CreatePawn(Country.Get("Germany"), CachedItems.RandomName, TroopType.Get("Swordsman"),
                         Regiment.Get(0), WCMngr.I.mainCam.ScreenToWorldPoint(Input.mousePosition)); // make sure this id is right!
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            PawnManager.I.CreatePawn(Country.Get("France"), CachedItems.RandomName, TroopType.Get("Archer"), 
                        Regiment.Get(1), WCMngr.I.mainCam.ScreenToWorldPoint(Input.mousePosition), sampleProjectiles);
        }*/

        if (Input.GetKeyDown(KeyCode.M))
        {
            var go = Instantiate(WCMngr.I.firePrefab);
            Vector2Int p = Vector2Int.FloorToInt(WCMngr.I.mainCam.ScreenToWorldPoint(Input.mousePosition));
            go.transform.position = new Vector3(p.x, p.y, -2);
        }
        if (Input.GetKeyDown(KeyCode.F))
            Messages.I.Add("You pressed F. Respects have been paid.");
    }
}
