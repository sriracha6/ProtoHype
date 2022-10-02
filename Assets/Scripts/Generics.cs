using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weapons;

namespace Generics
{
    public struct Generic
    {
        public string name;
        public string type;
        public List<string> identifiers;

        public Generic(string name, List<string> elements, string type)
        {
            this.name = name;
            this.identifiers = elements;
            this.type = type;
        }
    }
    public static class GenericManager
    {
        public static List<Generic> generics = new List<Generic>();

        public static List<T> GetGeneric<T>(string name) where T : Item
        {
            if (generics.Exists(x => x.name == name))
                return generics.Find(d => d.name == name).identifiers.ConvertAll<T>(x => (T)Item.GetGenericItem(x));
            else
                DB.Attention($"couldn't find generic of name \"{name}\"");
                return null;
        }

        public static void CreateGenericList(string name, List<string> contents, string type)
        {
            generics.Add(new Generic(name, contents, type));
        }
    }
}