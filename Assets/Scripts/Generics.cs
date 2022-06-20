using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generics
{
    public struct Generic<T>
    {
        public string name;
        public Type type;
        public List<T> elements;

        public Generic(string name, List<T> elements, Type type)
        {
            this.name = name;
            this.elements = elements;
            this.type = type;
        }
    }
    public static class GenericManager
    {
        public static List<object> generics = new List<object>();

        public static void CreateGenericList<T>(string name, List<T> contents, Type type)
        {
            generics.Add(new Generic<T>(name, contents, type));
        }
    }
}