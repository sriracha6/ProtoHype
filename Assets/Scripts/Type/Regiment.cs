using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PawnFunctions;
using TroopTypes;
using Countries;

namespace Regiments
{
    public static class RegimentManager
    {
        public static List<Regiment> RegimentList = new List<Regiment>();

        public static void Create(TroopType trooptype, Country origin, int id) // creates if it DOESNT exist
        {
            Regiment c = new Regiment(trooptype, origin, RegimentList.Count);
            RegimentList.Add(c);
        }
        public static Regiment Get(int id)
        {
            try
            {
                return RegimentList.Find(x => x.id == id);
            }
            catch (NullReferenceException)
            {
                //Create(name);
                return null; // lol
            }
        }
    }
    public class Regiment
    {
        public TroopType type; // like archer, swordsman, etc.
        public int id;
        public bool isFriendly; // probably should convert this to countries
        public List<Pawn> members = new List<Pawn>();
        public Country countryOrigin;
        public List<Transform> memberTransforms = new List<Transform>();

        public Regiment(TroopType t, Country origin, int i)
        {
            type = t;
            id = i;
            countryOrigin = origin;
            //isFriendly = Player.playerCountry.Equals(origin) ? true:false;
        }
        public void Add(Pawn p)
        {
            members.Add(p);
            memberTransforms.Add(p.gameObject.transform);
        }
    }
}