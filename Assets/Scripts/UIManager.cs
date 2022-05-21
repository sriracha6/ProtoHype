using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Buddah bless this class
/// </summary>
public class UIManager : MonoBehaviour
{
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
        ui = __ui;
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
        if (Input.GetMouseButton(0) && draggable.Contains(v))
        {
            //mouseOverUI = true;
            v.style.top = x.mousePosition.y - v.layout.height / 2;
            v.style.left = x.mousePosition.x - v.layout.width / 2;
        }
    }
}

