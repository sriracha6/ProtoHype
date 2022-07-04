using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ContextMenuShower : MonoBehaviour
{
    [SerializeField] UIDocument root;
    public VisualElement menu;

    public static ContextMenuShower I;

    bool canClose;

    public bool watchOut = false;

    protected void Awake() {
        if (I == null)
            I = this;
        else
        {
            var oldRoot = root;
            VisualElement mm = root.rootVisualElement.Q<VisualElement>("contextmenu");
            root = UIManager.ui;
            root.rootVisualElement.Add(mm);
            oldRoot.rootVisualElement.Remove(mm);
        }
        DontDestroyOnLoad(gameObject);
    }

    protected void Start()
    {
        //
        menu = root.rootVisualElement.Q<VisualElement>("contextmenu");

        menu.RegisterCallback<MouseEnterEvent>(x => canClose = false);
        menu.RegisterCallback<MouseLeaveEvent>(x => canClose = true);
        menu.RegisterCallback<MouseDownEvent>(x=>Debug.Log($"Monkey testicles"));

        menu.style.display = DisplayStyle.None;
        //menu.parent.style.display = DisplayStyle.None;
    }

    public void Show(MouseUpEvent e, bool UI)
    {
        bool shouldShow = UI || !UIManager.mouseOverUI;
        if(Input.GetMouseButtonUp(Keybinds.RightMouse) && shouldShow)
        {
            Vector2 pos;
            //pos.y = Screen.height - e.originalMousePosition.y - menu.style.height.value.value;
            pos.y = e.originalMousePosition.y;
            pos.x = e.originalMousePosition.x;
            //pos.x = e.originalMousePosition.x - menu.style.width.value.value;

            menu.style.left = pos.x;
            menu.style.top = pos.y;
            menu.style.display = DisplayStyle.Flex;
            menu.style.visibility = Visibility.Visible;
            menu.BringToFront();
        }
    }

    protected void Update()
    {
        if(watchOut)
        {
            if (Input.GetMouseButtonUp(Keybinds.RightMouse))
            {
                root.rootVisualElement.Q<VisualElement>("Membrane").pickingMode = PickingMode.Ignore;
                root.rootVisualElement.Q<VisualElement>("Membrane").style.visibility = Visibility.Hidden;
                root.rootVisualElement.Q<VisualElement>("Membrane").style.display = DisplayStyle.None;
                Show(new MouseUpEvent(), false);
            }
        }
    }

    public void ClearItems()
    {
        menu.Clear();
    }

    protected void LateUpdate()
    {
        HideIfClickedOutside(menu);
    }

    public void AddButton(ContextMenuItem item)
    {
        var button = new Button(item.onClick);
        button.AddToClassList("contextmenubutton");
        button.text = item.ItemName;
        button.clicked += delegate
        {
            item.onClick();
            menu.style.display = DisplayStyle.None;
            root.rootVisualElement.Q<VisualElement>("Membrane").pickingMode = PickingMode.Position;
            root.rootVisualElement.Q<VisualElement>("Membrane").style.visibility = Visibility.Visible;
            root.rootVisualElement.Q<VisualElement>("Membrane").style.display = DisplayStyle.Flex;
        };
        menu.Add(button);
    }

    public void RemoveButton(ContextMenuItem item)
    {
        Button b = null;
        foreach (Button v in menu.Children())
            if (v.text == item.ItemName)
                b = v;

        menu.Remove(b);
    }

    void HideIfClickedOutside(VisualElement menu)
    {
        if (Input.GetMouseButtonDown(Keybinds.LeftMouse) && menu.style.display == DisplayStyle.Flex && canClose)
        {
            menu.style.display = DisplayStyle.None;
            root.rootVisualElement.Q<VisualElement>("Membrane").pickingMode = PickingMode.Position;
            root.rootVisualElement.Q<VisualElement>("Membrane").style.visibility = Visibility.Visible;
            root.rootVisualElement.Q<VisualElement>("Membrane").style.display = DisplayStyle.Flex;
        }
    }
}