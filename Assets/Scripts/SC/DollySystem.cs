using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DollySystem : MonoBehaviour
{
    // J = PLACE MOVE FAST
    // K = PLACE STOP FOR 3S
    // L = PLACE MOVE SLOW
    // ; = PLAY

    enum MoveType { MoveFast, Stop3S, MoveSlow }
    struct DollyPos
    {
        public Vector2Int position;
        public MoveType moveType;

        public DollyPos(Vector2Int position, MoveType moveType)
        {
            this.position = position;
            this.moveType = moveType;
        }
    }

    static List<DollyPos> Dollies = new List<DollyPos>();

    protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
            Dollies.Add(new DollyPos(Vector2Int.FloorToInt(Input.mousePosition) / 2, MoveType.MoveFast));
        if (Input.GetKeyDown(KeyCode.K))
            Dollies.Add(new DollyPos(Vector2Int.FloorToInt(Input.mousePosition) / 2, MoveType.Stop3S));
        if (Input.GetKeyDown(KeyCode.L))
            Dollies.Add(new DollyPos(Vector2Int.FloorToInt(Input.mousePosition) / 2, MoveType.MoveSlow));
        
        if(Input.GetKeyDown(KeyCode.Semicolon))
        {
            StartCoroutine(Play());
        }
    }

    IEnumerator Play()
    {
        foreach (DollyPos p in Dollies)
        {
            if(p.moveType == MoveType.MoveFast)
            {
                CameraMove.I.GlideTo(p.position);
                yield return new WaitUntil(()=>!CameraMove.I.isGliding);
            }
            else if (p.moveType == MoveType.MoveSlow)
            {
                CameraMove.I.moveSpeed /= 2;
                CameraMove.I.GlideTo(p.position);
                yield return new WaitUntil(() => !CameraMove.I.isGliding);
                CameraMove.I.moveSpeed *= 2;
            }
            else if(p.moveType == MoveType.Stop3S)
            {
                CameraMove.I.canMove = false;
                yield return new WaitForSecondsRealtime(3);
                CameraMove.I.canMove = true;
            }
        }
    }
}
