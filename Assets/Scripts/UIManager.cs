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
    [SerializeField] internal UIDocument __ui;
    private static UIDocument ___UI;
    public static UIDocument ui
    {
        get { return ___UI; }
        set {
            ___UI = value;
            if (I.OnUiChange != null)
                I.OnUiChange();
        }
    }

    public static MouseMoveEvent currentMouse;
    public static bool mouseOverUI { get; private set; }
    private static readonly List<VisualElement> draggable = new List<VisualElement>();

    public delegate void DelegateOnUIChange();
    public event DelegateOnUIChange OnUiChange;

    private static void mouseEnter(VisualElement v, MouseEnterEvent e)
    {
        if (v.style.visibility != Visibility.Hidden && v.style.display != DisplayStyle.None && v.name != "Membrane")
            mouseOverUI = true;
    }

    protected void Awake()
    {
        if (I == null)
        {
            I = this;
            OnUiChange += I.RefreshUI;
        }
        else
            ui = __ui;
    }

    void RefreshUI()
    {
        var root = ui.rootVisualElement;
        if(Menus.I.inBattle)
        {
            List<ContextMenuItem> list = new List<ContextMenuItem>
                {
                    new ContextMenuItem("Move Here", MoveRCMenu),
                    new ContextMenuItem("Clear Selection", ClearRCMenu)
                };
            root.AddManipulator(new ContextMenuManipulator(list, false));
            root.pickingMode = PickingMode.Position;
        }
        root.RegisterCallback<MouseMoveEvent>(x => { currentMouse = x; });
        foreach (VisualElement v in root.Children())
        {
            //v.RegisterCallback<MouseMoveEvent>(x => { currentMouse = x; });
            if (v.name == "Membrane")
                continue;

            if (!string.IsNullOrEmpty(v.tooltip))
                v.AddManipulator(new ToolTipManipulator());

            v.RegisterCallback<MouseEnterEvent>(x => mouseEnter(v, x)); // worked
            v.RegisterCallback<MouseLeaveEvent>(x => mouseOverUI = false);

        }
    }

    public static void RegisterMouseOver(VisualElement v)
    {
        v.RegisterCallback<MouseEnterEvent>(x => mouseEnter(v, x)); // worked
        v.RegisterCallback<MouseLeaveEvent>(x => mouseOverUI = false);
    }    

    private void ClearRCMenu()
    {
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

