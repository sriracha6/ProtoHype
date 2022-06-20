using System.Collections;
using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

public static class LoadComponents
{
    /*public static void Load<T>() where T : MonoBehaviour, new()
    {
        T ty = new T();

        foreach (var prop in ty.GetType().GetProperties())
        {
            if(prop.GetType().IsSubclassOf(typeof(Component)))
                ty.GetType()
            Console.WriteLine("{0}={1}", prop.Name, (int)prop.GetValue(ty, null) + ty.GetType().GetProperties().Length);
        }
    }*/
}
