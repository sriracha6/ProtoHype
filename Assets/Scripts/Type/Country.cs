using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PawnFunctions;
using System;
using System.Linq;

namespace Countries
{
    public static class CountryManager
    {
        public static List<Country> CountryList = new List<Country>();

        public static Country Create(string name, string memberName)
        {
            if(!CountryList.Any(x => x.Name == name))
            {
                Country c = new Country(name, memberName);
                CountryList.Add(c);
                return c;
            }
            else
            {
                //Debug.Log("Tried to create multiple of: "+name);
                return null;
            }
        }
        public static Country Get(string name)
        {
            try
            {
                return CountryList.Find(x => x.Name == name);
            }
            catch (NullReferenceException)
            {
                //Create(name);
                return null;
                //return CountryList.Find(x => x.Name == name);
            }
        }
    }
    public class Country
    {

        public List<Pawn> members = new List<Pawn>();
        public List<Transform> memberTransforms = new List<Transform>();

        public string Name; // TODO: make this not hardcoded
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
    }
}