using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Beware line 86.
/// </summary>
public class ContextMenuShower : MonoBehaviour
{
    public VisualElement root;
    public VisualElement menu;

    public Button moveHereButton;
    public Button showSelectionButton;
    public Button clearSelectionButton;

    public LineRenderer lineRenderer;

    bool canClose;

    private void Start()
    {
        //
        root = GetComponent<UIDocument>().rootVisualElement;//
        menu = root.Q<VisualElement>("contextmenu");

        menu.RegisterCallback<MouseEnterEvent>(x => canClose = false);
        menu.RegisterCallback<MouseLeaveEvent>(x => canClose = true);

        moveHereButton = menu.Q<Button>("MoveHereButton");
        showSelectionButton = menu.Q<Button>("ShowSelectionButton");
        clearSelectionButton = menu.Q<Button>("ClearSelectionButton");

        moveHereButton.clicked += delegate { MenuItemPressed("Move"); };
        showSelectionButton.clicked += delegate { MenuItemPressed("ShowSelection"); };
        clearSelectionButton.clicked += delegate { MenuItemPressed("ClearSelection"); };

        menu.style.display = DisplayStyle.None;
        //menu.parent.style.display = DisplayStyle.None;
    }
    private void Update()
    {
        if (Input.GetMouseButtonUp(1)) // TODO: keybinds
        {
            menu.style.top = Screen.height - Input.mousePosition.y; // why for y but not x? wtf?
            menu.style.marginLeft = Input.mousePosition.x;

            menu.style.display = DisplayStyle.Flex; 
        }
    }
    private void LateUpdate()
    {
        HideIfClickedOutside(menu);
    }

    void MenuItemPressed(string name)
    {
        if (name == "ClearSelection")
        {
            Player.ourSelectedPawns.Clear();
            Player.selectedPawns.Clear();
            Player.selectedTileBounds.Clear();
            Player.selectedTilePoses.Clear();
        }
        else if (name == "ShowSelection")
        {
            int currentLoop = 1;
            lineRenderer.positionCount = 0;
            lineRenderer.positionCount = Player.selectedTileBounds.Count * 5;
            for (int i = 0; i < Player.selectedTileBounds.Count; i++)
            {
                lineRenderer.transform.position = new Vector2(Player.selectedTileBounds[i].xMax, Player.selectedTileBounds[i].yMax);//(new Vector2(Player.selectedTileBounds[i].xMin, Player.selectedTileBounds[i].yMin) + new Vector2(Player.selectedTileBounds[i].xMax, Player.selectedTileBounds[i].yMax)) / 2;
                lineRenderer.SetPosition(i * currentLoop, new Vector3(Player.selectedTileBounds[i].xMin, Player.selectedTileBounds[i].yMax, 0));
                lineRenderer.SetPosition(i * currentLoop + 1, new Vector3(Player.selectedTileBounds[i].xMax, Player.selectedTileBounds[i].yMax, 0));
                lineRenderer.SetPosition(i * currentLoop + 2, new Vector3(Player.selectedTileBounds[i].xMax, Player.selectedTileBounds[i].yMin, 0));
                lineRenderer.SetPosition(i * currentLoop + 3, new Vector3(Player.selectedTileBounds[i].xMin, Player.selectedTileBounds[i].yMin, 0));
                lineRenderer.SetPosition(i * currentLoop + 4, new Vector3(Player.selectedTileBounds[i].xMin, Player.selectedTileBounds[i].yMax, 0));
                currentLoop++;
            }
        }
        else if (name == "MoveHere")
        {
            //Player.selectedTiles.Clear();
            Player.selectedTileBounds.Clear();
            Player.selectedTilePoses.Clear();

            Debug.Log(GameManager2D.Instance.groundTilemap.WorldToCell(GameManager2D.Instance.mainCam.ScreenToWorldPoint(Input.mousePosition)));
            // wtf
            Player.selectedTilePoses.Add(GameManager2D.Instance.groundTilemap.WorldToCell(GameManager2D.Instance.mainCam.ScreenToWorldPoint(Input.mousePosition)));

            ActionType item = new ActionType("Move", true);
            foreach (PawnFunctions.Pawn p in Player.ourSelectedPawns) 
            {
                p.actionTypes.Add(item);
            }
            print("Doing ActionType: " + "MoveHere");
        }
        menu.style.display = DisplayStyle.None;
        //menu.parent.style.display = DisplayStyle.None;
    }

    void HideIfClickedOutside(VisualElement menu)
    {
        if (Input.GetMouseButtonDown(0) && menu.style.display==DisplayStyle.Flex && canClose)
        {
            menu.style.display=DisplayStyle.None;
        }
    }
}
