using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ContextMenuShower : MonoBehaviour
{
    [SerializeField] UIDocument root;
    public VisualTreeAsset MENUASSET;
    public VisualElement menu;

    public static ContextMenuShower I;

    bool canClose;
    bool open = false;
    public bool watchOut = false;
    public List<ContextMenuItem> watchOutItems = new List<ContextMenuItem>();

    protected void Start() 
    {
        if (I == null)
        {
            I = this;
            UIManager.OnUiChange += I.OnUiChange;
        }
    }

    public void OnUiChange() 
    {
        //var oldRoot = root;
        VisualElement mm = I.MENUASSET.CloneTree().contentContainer.Q<VisualElement>("contextmenu");
        UIManager.TransferToNewUI(mm, "contextmenu");
        I.root = UIManager.ui;
        /*var MM = I.MENUASSET.CloneTree().contentContainer.Q<VisualElement>("contextmenu");
        if (!I.root.rootVisualElement.Contains(MM))
            I.root.rootVisualElement.Add(MM);*/

        I.menu = I.root.rootVisualElement.Q<VisualElement>("contextmenu");
        I.menu.BringToFront();
        I.menu.RegisterCallback<MouseEnterEvent>(x => I.canClose = false);
        I.menu.RegisterCallback<MouseLeaveEvent>(x => I.canClose = true);

        I.menu.style.display = DisplayStyle.None;
        //oldRoot.rootVisualElement.Remove(mm);
    }

    public void Show(MouseUpEvent e, bool UI, bool IsGay=false)
    {
        bool shouldShow = UI || !UIManager.mouseOverUI;

        if (Input.GetMouseButtonUp(Keybinds.RightMouse) && shouldShow && !TileSelection.started)
        {
            I.open = true;
            Vector2 pos;
            if (!IsGay)
            {
                pos.y = e.originalMousePosition.y;
                pos.x = e.originalMousePosition.x;
            }
            else
            {
                float xxx = (Input.mousePosition.x / Screen.width) * I.menu.layout.width;
                float yyy = (Input.mousePosition.y / Screen.height) * I.menu.layout.height;
                
                pos.y = Screen.height - Input.mousePosition.y + yyy;
                pos.x = Input.mousePosition.x + xxx;
            }
            I.menu.style.left = pos.x;
            I.menu.style.top = pos.y;
            I.menu.style.display = DisplayStyle.Flex;
            I.menu.style.visibility = Visibility.Visible;
            I.menu.BringToFront();
        }
    }

    protected void Update()
    {
        if(I.watchOut)
        {
            if (Input.GetMouseButtonUp(Keybinds.RightMouse))
            {
                I.ClearItems();
                /*UIManager.ui.rootVisualElement.Q<VisualElement>("Membrane").pickingMode = PickingMode.Ignore;
                UIManager.ui.rootVisualElement.Q<VisualElement>("Membrane").style.visibility = Visibility.Hidden;
                UIManager.ui.rootVisualElement.Q<VisualElement>("Membrane").style.display = DisplayStyle.None;*/
                foreach (ContextMenuItem item in I.watchOutItems)
                    I.AddButton(item);
                I.Show(new MouseUpEvent(), false, true);
            }
        }
    }

    public void ClearItems()
    {
        I.menu.Clear();
    }

    protected void LateUpdate()
    {
        I.HideIfClickedOutside(I.menu);
    }

    public void AddButton(ContextMenuItem item)
    {
        var button = new Button(item.onClick);
        button.AddToClassList("contextmenubutton");
        button.text = item.ItemName;
        button.clicked += delegate
        {
            item.onClick();
            I.menu.style.display = DisplayStyle.None;
            I.open = false;
            /*UIManager.ui.rootVisualElement.Q<VisualElement>("Membrane").pickingMode = PickingMode.Position;
            UIManager.ui.rootVisualElement.Q<VisualElement>("Membrane").style.visibility = Visibility.Visible;
            UIManager.ui.rootVisualElement.Q<VisualElement>("Membrane").style.display = DisplayStyle.Flex;*/
        };
        I.menu.Add(button);
    }

    public void RemoveButton(ContextMenuItem item)
    {
        Button b = null;
        foreach (Button v in I.menu.Children())
            if (v.text == item.ItemName)
                b = v;

        I.menu.Remove(b);
    }

    void HideIfClickedOutside(VisualElement menu)
    {
        if (I.open && 
            (Input.GetMouseButtonDown(Keybinds.LeftMouse) || Input.GetMouseButton(Keybinds.LeftMouse) || Input.GetMouseButtonUp(Keybinds.LeftMouse)) 
            && I.menu.style.display == DisplayStyle.Flex && I.canClose)
        {
            I.menu.style.display = DisplayStyle.None;
            /*UIManager.ui.rootVisualElement.Q<VisualElement>("Membrane").pickingMode = PickingMode.Position;
            UIManager.ui.rootVisualElement.Q<VisualElement>("Membrane").style.visibility = Visibility.Visible;
            UIManager.ui.rootVisualElement.Q<VisualElement>("Membrane").style.display = DisplayStyle.Flex;*/

            I.open = false;
        }
    }
}