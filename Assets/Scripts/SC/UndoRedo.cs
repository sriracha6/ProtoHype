using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndoRedo : MonoBehaviour
{
    public static int currentIndex;
    public static List<IUndoable> changedValues = new List<IUndoable>();

    protected void Update()
    {
        if(Input.GetKey(Keybinds.SubtractSelection) && Input.GetKeyDown(KeyCode.O))
        {
            //if (currentIndex - 1 < 0) return;
            if(currentIndex > 0) currentIndex--;
            Debug.Log($"UNDOING!");
            changedValues[currentIndex].Undo();
        }
        if (Input.GetKey(Keybinds.SubtractSelection) && Input.GetKeyDown(KeyCode.P))
        {
            //if (currentIndex + 1 >= changedValues.Count) return;
            if(currentIndex < changedValues.Count) currentIndex++;
            Debug.Log($"REDOING!");
            changedValues[currentIndex].Redo();
        }
    }
}
