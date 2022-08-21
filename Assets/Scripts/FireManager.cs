using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FireManager
{
    public static HashSet<Vector2> firePositions = new HashSet<Vector2>();
    public static List<FireBehaviour> fires = new List<FireBehaviour>();
}