using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public static class Logger
{
    public static readonly string fileLocation = Application.persistentDataPath + "\\.log";
    private static string lastDivide;

    private static string lastMessage;

    public static void Log(string text)
    {
        string line = $"\n[*]({Time.time}) {text}";
        File.AppendAllText(fileLocation, line);
    }

    public static void LogError(string err)
    {
        if (err == lastMessage) // prevent spam!
            return;
        string line = $"\n[\\]({Time.time}) Error: \n{err}\n\n\n";
        File.AppendAllText(fileLocation, line);
        lastMessage = err;
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
        File.AppendAllText(fileLocation, "\n"+txt);
    }

    public static void Clear()
    {
        File.Delete(fileLocation);
    }

    public static void LogUnityMsg(string condition, string stackTrace, LogType type)
    {
        if(type == LogType.Error || type == LogType.Exception)
            Logger.LogError($"{type} | {condition} : \n       {stackTrace}");
    }
}
