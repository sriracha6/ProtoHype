using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Buddah bless this class
/// </summary>
[RequireComponent(typeof(UIDocument))]
public class UIManager : MonoBehaviour
{
    public static UIManager I = null;
    [SerializeField] UIDocument __ui;
    private static UIDocument ___UI;
    public static int Reloads { get; internal set; }
    public static List<CachedItems.CachedSound> UISounds { get; internal set; } = new List<CachedItems.CachedSound>();

    public static UIDocument ui
    {
        get { return ___UI; }
        set {
            ___UI = value;
            OnUiChange?.Invoke(); // future self: this is why console/viewer not work on main menu
        }
    }
    public static bool UIHidden { get; set; } = false;

    public static bool mouseOverUI { get; private set; }
    private static readonly List<VisualElement> draggable = new List<VisualElement>();

    public delegate void DelegateOnUIChange();
    public static event DelegateOnUIChange OnUiChange;

    private List<VisualElement> hiddenVes = new List<VisualElement> ();

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
        else if (Menus.I.inBattle || Menus.I.inSC)
            ui = GetComponent<UIDocument>();

        OnUiChange += I.ScenarioEditorChange;
    }

    void ScenarioEditorChange()
    {
        if(Menus.I.scenLoad)
            Menus.I.SwitchTo(Menus.I.quickstart, QuickBattle.I);
    }

    void RefreshUI()
    {
        var root = ui.rootVisualElement;
        if (Menus.I.inBattle)
            root = GetComponent<UIDocument>().rootVisualElement;

        if (Menus.I.inBattle)
        {
            List<ContextMenuItem> list = new List<ContextMenuItem>
                {
                    new ContextMenuItem("Move Here", MoveRCMenu),
                    new ContextMenuItem("Clear Selection", ClearRCMenu)
                };
            root.AddManipulator(new ContextMenuManipulator(list, false));
            //root.pickingMode = PickingMode.Position;
        }
        LoopKids(root);
    }

    private void LoopKids(VisualElement root)
    {
        foreach (VisualElement v in root.Children())
        {
            //v.RegisterCallback<MouseMoveEvent>(x => { currentMouse = x; });
            if (v.name == "Membrane")
                continue;

            if (!string.IsNullOrEmpty(v.tooltip))
                v.AddManipulator(new ToolTipManipulator());
            v.style.opacity = Settings.HUDOpacity01;
            v.RegisterCallback<MouseEnterEvent>(x => mouseEnter(v, x)); // worked
            v.RegisterCallback<MouseLeaveEvent>(x => mouseOverUI = false);

            if (v.childCount > 0)
                foreach(VisualElement v2 in root.Children())
                    LoopKids(v2);
        }
    }

    public void PlayUISound()
    {
        SFXManager.I.PlaySound("uiclick"+UnityEngine.Random.Range(1,7)+".mp3", "UI", 1, Vector2.zero, true);
    }

    public static void TransferToNewUI(VisualElement window, string name)
    {
        if (ui.rootVisualElement.Q<VisualElement>(name) == null)
            ui.rootVisualElement.Add(window);
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

    public static void ToggleUI()
    {
        foreach (VisualElement v in ui.rootVisualElement.Children())
        {
            if (v.name == "PauseMenuParent")
                continue;
            if (!UIHidden && v.style.visibility == Visibility.Hidden && v.style.display == DisplayStyle.None)
                I.hiddenVes.Add(v);

            if (!UIHidden)
            {
                v.style.display = DisplayStyle.None;
                v.style.visibility = Visibility.Hidden;
            }
            else
            {
                if (I.hiddenVes.Contains(v))
                    continue;
                v.style.display = DisplayStyle.Flex;
                v.style.visibility = Visibility.Visible;
            }
        }

        UIHidden = !UIHidden;
    }
}