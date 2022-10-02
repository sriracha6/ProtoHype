using PawnFunctions;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.Jobs;
using UnityEngine;

public struct FindNearestJob : IJob
{
    public Vector2Int position;
    public Vector2Int output;

    public FindNearestJob(Vector2Int position)
    {
        this.position = position;
        this.output = Vector2Int.zero;
    }

    public void Execute()
    {
        output = PathfindExtra.FindNearest(position);   
    }
}
