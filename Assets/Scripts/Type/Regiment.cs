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
        public int id { get; }
        public bool isFriendly { get; }
        public List<Pawn> members = new List<Pawn>();
        public Country countryOrigin { get; }
        public List<Transform> memberTransforms { get; } = new List<Transform>();
        public Pawn flagBearer = null;

        public Regiment(TroopType t, Country origin, int i)
        {
            type = t;
            id = i;
            countryOrigin = origin;
            isFriendly = Player.playerCountry == origin;
        }
        /// <summary>
        /// DO NOT USE members.Add!!! USE THIS INSTEAD! Notes: first pawn added is flag beraer
        /// </summary>
        /// <param name="p">Pawn to add.</param>
        public void Add(Pawn p)
        {
            if (members.Count == 0)
            {
                p.isFlagBearer = true;
                this.flagBearer = p;
                var g = UnityEngine.Object.Instantiate(Loader.loader.flagPrefab, p.transform);
                g.GetComponent<FlagBehaviour>().flagTexture = CachedItems.renderedCountries.Find(x=>x.name==countryOrigin.Name).image;
                members.Add(p);
                memberTransforms.Add(p.gameObject.transform);
            }
            else
            {
                members.Add(p);
                memberTransforms.Add(p.gameObject.transform);
            }
        }
    }
}