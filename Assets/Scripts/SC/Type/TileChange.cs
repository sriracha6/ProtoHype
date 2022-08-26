using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileChange : IUndoable
{
    public List<(Vector2Int position, List<Build> build)> List { get; set; }

    public void Redo()
    {
        foreach (var step in List)
            TilemapPlace.SetAll(step.build, step.position.x, step.position.y);
    }

    public void Undo()
    {
        foreach (var step in List)
            TilemapPlace.RemoveAll(step.position.x, step.position.y);
    }

    public TileChange(List<(Vector2Int position, List<Build> build)> changes)
    {
        this.List = changes;
    }
}
