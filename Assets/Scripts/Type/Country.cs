using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PawnFunctions;
using System;
using System.Linq;

namespace Countries
{
    public class Country
    {

        public List<Pawn> members = new List<Pawn>();
        public List<Transform> memberTransforms = new List<Transform>();

        public string Name; // make this not harcoded  REPLY: :)
        public string memberName;

        public Country(string name, string membername)
        {
            Name = name;
            memberName = membername;
        }
        public void Add(Pawn p)
        {
            members.Add(p);
            memberTransforms.Add(p.gameObject.transform);
        }

        public static List<Country> List = new List<Country>();

        public static Country Create(string name, string memberName)
        {
            //if(!CountryList.Exists(x => x.Name == name))
            //{
            Country c = new Country(name, memberName);
            List.Add(c);
            return c;
            //}
            //else
            //{
            //    return null;
            //}
        }
        public static Country Get(string name)
        {
            try
            {
                return List.Find(x => x.Name == name);
            }
            catch (NullReferenceException)
            {
                //Create(name);
                DB.Attention($"Couldn't find Country of name {name}");
                return null;
                //return CountryList.Find(x => x.Name == name);
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}