using Countries;
using PawnFunctions;
using Projectiles;
using Regiments;
using System;
using System.Collections;
using System.Collections.Generic;
using TroopTypes;
using UnityEngine;

using Random = UnityEngine.Random;

public class PawnManager : MonoBehaviour
{
    public GameObject pawnPrefab;
    private static List<Pawn> allPawns = new List<Pawn>();

    public Pawn CreatePawn(Country c, string n, TroopType tt, Regiment r, Vector2 pos, List<Projectile> projectiles = null)
    {
        // USE POOLING!!!!!!!!!!!!!!!!!!!! TODO and make this its own class
        GameObject newPawnObj = Instantiate(pawnPrefab);//gameObject.AddComponent<Pawn>();
        Pawn newPawn = newPawnObj.GetComponent<Pawn>();

        newPawnObj.transform.position = new Vector3(pos.x, pos.y, -0.5f);

        newPawn.country = c;
        newPawn.pname = n;
        newPawn.troopType = tt;
        newPawn.regiment = r;
        r.Add(newPawn);

        foreach (Country co in CountryManager.CountryList)
        {
            if (co.Name != newPawn.country.Name)
            {
                newPawn.enemyCountries.Add(co);
            }
        }

        newPawn.inventory = projectiles;
        try
        {
            newPawn.heldPrimary = tt.weapons[UnityEngine.Random.Range(0, tt.weapons.Count)];
            newPawn.hasPrimary = true;
        }
        catch (NullReferenceException e)
        {
            Debug.Log("No weapon? wtf? " + e);
            newPawn.hasPrimary = false;
        }
        newPawn.activeWeapon = newPawn.heldPrimary;

        newPawn.activeWeaponSlot = ActiveWeapon.Primary;

        newPawn.armor.AddRange(tt.armor); // im sure that when i ADD it wont add shit it isnt supposed to.
        if (tt.shields.Count > 0) // TODO: do this for all other of these (armor+weapons)
        {
            newPawn.shield = tt.shields[UnityEngine.Random.Range(0, tt.shields.Count)];
            newPawn.hasShield = true;
        }
        else
            newPawn.hasShield = false;

        newPawn.meleeSkill = UnityEngine.Random.Range(tt.meleeSkillMin, tt.meleeSkillMax + 1);
        newPawn.rangeSkill = UnityEngine.Random.Range(tt.rangeSkillMin, tt.rangeSkillMax + 1);

        if (tt.sidearms.Count > 0)
        {
            newPawn.heldSidearm = tt.sidearms[UnityEngine.Random.Range(0, tt.sidearms.Count)];
            newPawn.hasSidearm = true;
        }
        else
            newPawn.hasSidearm = false;
        // TODO: this line below this is insanely bad. POOLING!
        //newPawn.gameObject = GameObject.Instantiate(pawnPrefab);

        newPawn.country.Add(newPawn);
        newPawn.sprite.material.color = generateSkinColor();
        allPawns.Add(newPawn);
        // todo
        PopulateRegiments.updateAllRegimentsSelectNumber(Player.regimentSelectNumber);
        return newPawn;
    }

    #region ------- Gets --------
    public static List<Pawn> GetAll() // og code of this used to use gameobject.getobjectswithtag AND getcomponent...... for each pawn
    {
        return allPawns;
    }

    public static List<Pawn> GetAllFriendlies() // todo: expand upon
    {
        return allPawns.FindAll(x=>x.country==Player.playerCountry);
    }
    #endregion

    public static Color generateSkinColor()
    {
        return Color.Lerp(new Color(241 / 255f, 194 / 255f, 125 / 255f), new Color(141 / 255f, 85 / 255f, 36 / 255f), Random.Range(0f, 1f));
    }
    public static Color generateSkinColor(int darkness)
    {
        return Color.Lerp(new Color(241 / 255f, 194 / 255f, 125 / 255f), new Color(141 / 255f, 85 / 255f, 36 / 255f), Random.Range(0f, 1f));
    }
}
