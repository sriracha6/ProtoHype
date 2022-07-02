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
    private static List<VisualElement> draggable = new List<VisualElement>();

    private void mouseEnter(VisualElement v)
    {
        if (v.style.display == DisplayStyle.Flex && v.name != "Membrane")
            mouseOverUI = true;
    }

    private void Start()
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
                    continue;
                if (!string.IsNullOrEmpty(v.tooltip))
                    v.AddManipulator(new ToolTipManipulator());

                v.RegisterCallback<MouseEnterEvent>(x => mouseEnter(v)); // worked
                v.RegisterCallback<MouseLeaveEvent>(x => mouseOverUI = false);

            }
            root.Q<VisualElement>("Membrane").RegisterCallback<MouseUpEvent>(x => __menu.Position(x));
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
        handle.RegisterCallback<MouseDownEvent>(x => startDrag(dragbody)/*x=>drag(dragbody)*/);
        handle.RegisterCallback<MouseUpEvent>(x => endDrag(dragbody)/*x=>drag(dragbody)*/);
    }

    static void startDrag(VisualElement v)
    {
        draggable.Add(v);
        ui.rootVisualElement.RegisterCallback<MouseMoveEvent>(x => drag(v, x));
    }

    static void endDrag(VisualElement v)
    {
        draggable.Remove(v);
        ui.rootVisualElement.UnregisterCallback<MouseMoveEvent>(x => doNothing());
    }
    static int doNothing() { return 2+2;}

    static void drag(VisualElement v, MouseMoveEvent x)
    {
        if (Input.GetMouseButton(Keybinds.LeftMouse) && draggable.Contains(v))
        {
            //mouseOverUI = true;
            v.style.top = x.mousePosition.y - v.layout.height / 2;
            v.style.left = x.mousePosition.x - v.layout.width / 2;
        }
    }

    public void Flash(VisualElement v, Color currentColor)
    {
        StartCoroutine(__flash(v, currentColor));
        //v.style.backgroundColor = currentColor;
    }

    IEnumerator __flash(VisualElement v, Color oldColor)
    {
        for (int i = 0; i < 3; i++)
        {
            v.style.backgroundColor = Color.yellow;
            yield return new WaitForSecondsRealtime(0.25f);
            v.style.backgroundColor = new StyleColor( oldColor );
            yield return new WaitForSecondsRealtime(0.25f);
        }
        v.style.backgroundColor = oldColor;
        //v.style.backgroundColor = new StyleColor(oldColor);
    }
}

