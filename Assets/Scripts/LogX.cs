using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
/// <summary>
/// TODO: when building, just comment out what the functions do to prevent lag. EZ!
/// </summary>
public static class DB
{
    public static List<string> log = new List<string>();

    public static void Null(object o)
    {
        if(o==null)
            Debug.Log($"<color=red>It's Null</color>");
        else
            Debug.Log($"<color=green>It's Not Null</color>");
    }
    public static void Null<T>(T[] o)
    {
        if (o.Length == 0)
            Debug.Log($"<color=red>It's Empty</color>");
        else
            Debug.Log($"<color=green>It's Not Empty</color>");
    }

    public static void NullCount<T>(IEnumerable<T> o)
    {
        int ch = 0;
        foreach(T t in o)
            if (t == null) ch++;
        if (ch == 0)
            Debug.Log($"<color=green> List of {o.Count()} "+o.GetType().GetGenericArguments()[0] + " has no NULLS. </color>");
        else
            Debug.Log($"<color=red> List of {o.Count()} "+o.GetType().GetGenericArguments()[0] + " has "+ch+" NULLS. </color>");
    }

    public static void NullCount<T>(T[,] o)
    {
        int ch = 0;
        for(int i = 0; i < o.GetLength(0); i++)
        {
            for (int j = 0; j < o.GetLength(1); j++)
            {
                if (o[i,j] == null) ch++;
            }
        }
        if (ch == 0)
            Debug.Log("<color=green> List of " + o.GetType().Name + " has no NULLS. </color>");
        else
            Debug.Log("<color=red> List of " + o.GetType().Name + " has " + ch + " NULLS. </color>");
    }

    public static string ClassString(object o)
    {
        BindingFlags bindingFlags = BindingFlags.Public |
                            BindingFlags.NonPublic |
                            BindingFlags.Instance |
                            BindingFlags.FlattenHierarchy |
                            BindingFlags.Static;

        string x = "";

        foreach (FieldInfo field in o.GetType().GetFields(bindingFlags))
        {
            if (field.GetValue(o) == null)
                x += "!!Null!!\n";
            else
                x += field.Name + " : " + field.GetValue(o).ToString() + "\n";
        }
        return x;
    }

    public static void LogClassString(object o)
    {
        Debug.Log($"{ClassString(o)}");
    }

    public static void NullDrawing<T>(T[,] ooo)
    {
        string x = "";
        for(int i = 0; i < ooo.GetLength(0); i++)
        {
            for(int j = 0; j < ooo.GetLength(1); j++)
            {
                if (ooo[i, j] == null)
                    x += "#";
                else
                    x += " ";
            }
            x+="\n";
        }
        Debug.Log(x);
    }

    public static void ClassString<T>(T[,] ooo)
    {
        ClassString(ooo.Cast<T>());
    }

    public static void ClassString<T>(IEnumerable<T> ooo)
    {
        Debug.Log($"===================== List of {typeof(T).Name}");
        foreach (T o in ooo)
        {
            BindingFlags bindingFlags = BindingFlags.Public |
                                BindingFlags.NonPublic |
                                BindingFlags.Instance |
                                BindingFlags.FlattenHierarchy |
                                BindingFlags.Static;

            string x = "";

            if(o == null)
            {
                Debug.Log($"<color=red>Null</color>");
                continue;
            }

            foreach (FieldInfo field in o.GetType().GetFields(bindingFlags))
            {
                if (field.GetValue(o) == null)
                    x += field.Name + " : " + "<color=red>Null</color>\n";
                else
                    x += field.Name + " : " + field.GetValue(o).ToString() + "\n";
            }
            Debug.Log($"{x}");
        }
        Debug.Log($"=====================");
    }
    public static void LogL<T>(List<T> o)
    {
        string x = "<color=green>List</color>\n";
        foreach (object oo in o)
            if (oo == null)
                x += "<color=red>Null</color>\n";
            else
                x += oo.ToString() + "\n";
        Debug.Log(x);
    }
    public static void Attention(object o)
    {
        Logger.Log(">>>>> "+o+" <<<<<");
        Debug.Log($"<color=yellow>{o}</color>");
    }

    public static void AddToLog(object o)
    {
        log.Add(o.ToString());
    }
    public static void AttentionToLog(object o)
    {
        log.Add($"<color=yellow>{o}</color>");
    }
    public static void WriteLog()
    {
        string x = "";
        foreach(string s in log)
        {
            x += s + "\n";
        }
        Debug.Log($"{x}");
        log.Clear();
    }

    public static void DrawMap(this float[,] a)
    {
        string x = "";

        for(int i = 0; i < a.GetLength(0); i++)
        {
            for(int j = 0; j < a.GetLength(1); j++)
            {
                if (a[i, j] >= 0.9f)
                    x += "\u2588";
                else if (a[i, j] >= 0.70f)
                    x += "\u2593";
                else if (a[i, j] >= 0.5f)
                    x += "\u2592";
                else if (a[i, j] >= 0.3f)
                    x += "\u2591";
                else
                    x += " ";
            }
            x += "\n";
        }
        Debug.Log($"{x}");
    }

}
