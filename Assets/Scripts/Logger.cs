using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public static class Logger
{
    public static readonly string fileLocation = Application.persistentDataPath + "\\.log";
    private static string lastDivide;

    private static System.Exception lastError;

    public static void Log(string text)
    {
        string line = $"\n[*]({Time.time}) {text}";
        File.AppendAllText(fileLocation, line);
    }

    public static void Log(System.Exception err)
    {
        if (err == lastError) // prevent spam!
            return;
        string line = $"\n[\\]({Time.time}) Error: \n{err}\n\n\n";
        File.AppendAllText(fileLocation, line);
        lastError = err;
    }

    public static void Divide(string text)
    {
        lastDivide = text;
        string txt = new string('=', 15) + text + new string('=', 15);
        File.AppendAllText(fileLocation, txt);
    }

    public static void EndDivide()
    {
        string txt = new string('=', 30+lastDivide.Length);
        File.AppendAllText(fileLocation, txt);
    }

    public static void Clear()
    {
        File.Delete(fileLocation);
    }
}
