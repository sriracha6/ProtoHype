using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndoRedo : MonoBehaviour
{
    public static int currentIndex;
    public static List<List<(Vector2Int position, List<Build> build)>> changedValues = new List<List<(Vector2Int, List<Build>)>>();

    protected void Update()
    {
        if(Input.GetKey(Keybinds.SubtractSelection) && Input.GetKeyDown(KeyCode.Z))
        {
            if (currentIndex - 1 < 0) return;
            currentIndex--;
            Refresh();
        }
        if(Input.GetKey(Keybinds.SubtractSelection) && Input.GetKeyDown(KeyCode.Y))
        {
            if (currentIndex + 1 >= changedValues.Count) return;
            currentIndex++;
            Refresh();
        }
    }

    void Refresh()
    {
        foreach (var step in changedValues[currentIndex])
            TilemapPlace.SetAll(step.build, step.position.x, step.position.y);
    }
}
