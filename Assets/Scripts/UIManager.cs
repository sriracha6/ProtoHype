using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Buddah bless this class
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager I = null;
    [SerializeField] UIDocument __ui;
    [SerializeField] ContextMenuShower __menu;
    public static UIDocument ui; // this lne sucks
    public static bool mouseOverUI { get; private set; }
    private static readonly List<VisualElement> draggable = new List<VisualElement>();

    private void mouseEnter(VisualElement v)
    {
        if (v.style.display == DisplayStyle.Flex && v.name != "Membrane")
            mouseOverUI = true;
    }

    protected void Start()
    {
        if (I == null)
            I = this;
        else
        {
            ui = __ui;
            I.__menu = __menu;
            var root = ui.rootVisualElement;
            foreach (VisualElement v in root.Children())
            {
                if (v.name == "Membrane")
                {
                    List<ContextMenuItem> list = new List<ContextMenuItem>
                    {
                        new ContextMenuItem("Move Here", MoveRCMenu),
                        new ContextMenuItem("Clear Selection", ClearRCMenu)
                    };
                    v.AddManipulator(new ContextMenuManipulator(list, false));
                    v.pickingMode = PickingMode.Position;
                    continue;
                }
                if (!string.IsNullOrEmpty(v.tooltip))
                    v.AddManipulator(new ToolTipManipulator());

                v.RegisterCallback<MouseEnterEvent>(x => mouseEnter(v)); // worked
                v.RegisterCallback<MouseLeaveEvent>(x => mouseOverUI = false);

            }
        }
    }

    private void ClearRCMenu()
    {
        Debug.Log("ITS THAT Z SHIT ITS THAT Z SHIT");
        Player.ourSelectedPawns.Clear();
        Player.selectedPawns.Clear();
        Player.selectedTileBounds.Clear();
        Player.selectedTilePoses.Clear();
    }

    private void MoveRCMenu()
    {
        Player.selectedTileBounds.Clear();
        Player.selectedTilePoses.Clear();

        Debug.Log(WCMngr.I.groundTilemap.WorldToCell(WCMngr.I.mainCam.ScreenToWorldPoint(Input.mousePosition)));
        // wtf
        Player.selectedTilePoses.Add(WCMngr.I.groundTilemap.WorldToCell(WCMngr.I.mainCam.ScreenToWorldPoint(Input.mousePosition)));

        ActionType item = new ActionType("Move", true);
        foreach (PawnFunctions.Pawn p in Player.ourSelectedPawns)
        {
            p.actionTypes.Add(item);
        }
    }

    public static void SetupTooltips(VisualElement uidoc)
    {
        foreach (VisualElement v in uidoc.Children())
        {
            if (v.name == "Membrane")
                continue;
            if (!string.IsNullOrEmpty(v.tooltip))
                v.AddManipulator(new ToolTipManipulator());
        }
    }

    public static void MakeDraggable(VisualElement dragbody, VisualElement handle)
    {
        handle.RegisterCallback<MouseDownEvent>(x => startDrag(dragbody, handle)/*x=>drag(dragbody)*/);
        handle.RegisterCallback<MouseUpEvent>(x => endDrag(dragbody)/*x=>drag(dragbody)*/);
    }

    static void startDrag(VisualElement v, VisualElement handle)
    {
        draggable.Add(v);
        ui.rootVisualElement.RegisterCallback<MouseMoveEvent>(x => drag(v, handle, x));
    }

    static void endDrag(VisualElement v)
    {
        draggable.Remove(v);
        ui.rootVisualElement.UnregisterCallback<MouseMoveEvent>(x => doNothing());
    }
    static int doNothing() { return 69;}

    static void drag(VisualElement v, VisualElement handle, MouseMoveEvent x)
    {
        if (Input.GetMouseButton(Keybinds.LeftMouse) && draggable.Contains(v))
        {
            //mouseOverUI = true;
            v.style.top = x.mousePosition.y - handle.layout.height / 2;
            v.style.left = x.mousePosition.x - handle.layout.width / 2;
        }
    }

    public void Flash(VisualElement v)
    {
        StartCoroutine(__flash(v));
        //v.style.backgroundColor = currentColor;
    }

    IEnumerator __flash(VisualElement v)
    {
        var cc = v.style.backgroundColor.value;
        var oldC = new StyleColor(new Color(cc.r/255f, cc.g/255f, cc.b/255f));

        for (int i = 0; i < 3; i++)
        {
            v.style.backgroundColor = Color.yellow;
            yield return new WaitForSecondsRealtime(0.25f);
            v.style.backgroundColor = oldC;
            yield return new WaitForSecondsRealtime(0.25f);
        }
        v.style.backgroundColor = oldC;
        //v.style.backgroundColor = new StyleColor(oldColor);
    }
}

