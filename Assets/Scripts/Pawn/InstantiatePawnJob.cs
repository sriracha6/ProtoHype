using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

public struct InstantiatePawnJob : IJob
{
    public GameObject output;

    public void Execute() =>
        output = GameObject.Instantiate(PawnManager.I.pawnPrefab);
}
