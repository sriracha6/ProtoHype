using PawnFunctions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionType
{
    public string Type;
    public bool shouldMove;
    public bool toggle;
    public Vector2Int positionTarget;
    public bool overrideOverlap;

    public ActionType(string type, bool shouldMove, bool isToggle=false)
    {
        Type = type;
        //allPawnTransforms = transformsAll;
        this.shouldMove = shouldMove;
        toggle = isToggle;
    }
    public ActionType(string type, bool shouldMove, Vector2Int poisition, bool overrideOverlap)
    {
        this.Type = type;
        this.shouldMove= shouldMove;
        this.positionTarget = poisition;
        this.overrideOverlap = overrideOverlap;
    }

    public override string ToString()
    {
        return Type + " | " + positionTarget.ToString();// + $" | ShouldMove:{shouldMove.ToString()}";
    }
}
