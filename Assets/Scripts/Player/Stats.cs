using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PawnFunctions;
using System.Linq;
using System;
using System.Linq.Expressions;
using System.Reflection;

public static class Stats
{
    public static Pawn PawnWithMostKills 
    {
        get { return PawnManager.allPawns.OrderBy(x=>x.killCount).ToList()[0]; } 
    }

    public static int TotalCasualties
    {
        get { return PawnManager.allPawns.Sum(x=>x.killCount); }
    }
}
