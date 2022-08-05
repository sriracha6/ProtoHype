using PawnFunctions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Weapons;

/// <summary>
/// Since you're already here, here's some cool ideas I rejected:
/// <b>Take Cover</b> - too lazy to implement. Also, i'd never be satisfied with it's results and it would be very dumb.,
/// <b>Follow General</b> - pretty obvious, yeah, i was too lazy, but there was no way i'd implement it after i thought about it for 10 seconds. it overcomplicates the game, and there's not point to it anyway when you control everything.
/// </summary>
public class MoveControls : MonoBehaviour
{
    public Toggle runGunToggle;
    public Toggle secondaryToggle;
    public static VisualElement panel;

    public Button searchDestroyButton;
    public Button moveButton;
    public Button attackButton;
    public Button followCursorButton;
    public Button takeCoverButton;
    public Button standGroundButton;
    public Button rallyBehindButton;
    public Button cancelButton;
    public Button setFireButton;

    private static MoveControls instance;

    private bool pressShift = false;
    private bool selectTileFlag = false;
    //--------
    //public static ActionType actionType;
    //public static List<MoveQueue> actionTypes = new List<MoveQueue>();

    /*public static MoveQueue findQueue(Pawn search)
    {
        foreach(MoveQueue m in actionTypes)
        {
            if (!m.contains(search))
                continue;
            else
                return m;
        }
        return new MoveQueue();
    }
    int findQueueIndex(Pawn search)
    {
        int i = 0;
        foreach (MoveQueue m in actionTypes)
        {
            if (!m.ourPawns.Contains(search))
                continue;
            else
                return i;
            i++;
        }
        return -1;
    }*/

    protected void Update()
    {
        pressShift = Input.GetKey(Keybinds.SelectAdd);

        if (Input.GetKeyDown(Keybinds.mc_secondary))
            SecondaryToggle();
        if (Input.GetKeyDown(Keybinds.mc_runandgun))
            RunAndGunToggle();
        if (Input.GetKeyDown(Keybinds.mc_cancel))
            Option("Cancel", false);
        if (Input.GetKeyDown(Keybinds.mc_standground))
            Option("StandGround", false);
        if (Input.GetKeyDown(Keybinds.mc_followcursor))
            Option("FollowCursor", true);
        if(Input.GetKeyDown(Keybinds.mc_move))
            Option("Move", true);
        if(Input.GetKeyDown(Keybinds.mc_searchdestroy))
            Option("SearchAndDestroy", true);

        if(selectTileFlag && Input.GetMouseButtonUp(Keybinds.RightMouse) && !UIManager.mouseOverUI)
        {
            Vector2Int pos = Vector2Int.FloorToInt(Input.mousePosition);
            foreach(Pawn p in Player.ourSelectedPawns)
            {
                var ppos = Vector2Int.FloorToInt(p.transform.position);
                if(Vector2.Distance(ppos, pos) <= 1)
                {
                    StartCoroutine(startFire(pos));
                }
            }
            selectTileFlag = false;
        }
    }

    IEnumerator startFire(Vector2Int pos)
    {
        yield return new WaitForSeconds(2);
        var go = Instantiate(WCMngr.I.firePrefab);
        go.transform.position = new Vector3(pos.x, pos.y, -2);
    }

    protected void Awake()
    {
        // THIS SHOULDNT WORk
        //List<ActionType> action = new List<ActionType>();
        //action.Add(new ActionType("StandGround", Pawn.GetAll(), Pawn.GetAllFriendlies(), false));
        //actionTypes.Add(new MoveQueue(Pawn.GetAllFriendlies(),action));
        // finally it doesnt work :^)

        instance = this;
        var root = GetComponent<UIDocument>().rootVisualElement;//

        runGunToggle = root.Q<Toggle>("RunGunToggle");
        secondaryToggle = root.Q<Toggle>("SecondaryToggle");
        panel = root.Q<VisualElement>("MoveControls");

        searchDestroyButton = root.Q<Button>("SearchDestroyButton");
        moveButton = root.Q<Button>("MoveButton");
        attackButton = root.Q<Button>("AttackButton");
        followCursorButton = root.Q<Button>("FollowCursorButton");
        standGroundButton = root.Q<Button>("StandGroundButton");
        cancelButton = root.Q<Button>("CancelButton");
        setFireButton = root.Q<Button>("SetFireButton");

        /*        List<Button> blist = panel.Children<Button>();

                foreach (Button b in blist)
                {
                    b.clicked += delegate { Option(GetUntilOrEmpty(b.name, "Button")); };
                }*/
        searchDestroyButton.clicked += delegate { Option("SearchAndDestroy", true); }; // kill me
        moveButton.clicked += delegate { Option("Move", true); };
        attackButton.clicked += delegate { Option("Attack", true); };
        followCursorButton.clicked += delegate { Option("FollowCursor", true); };
        standGroundButton.clicked += delegate { Option("StandGround", false); };
        cancelButton.clicked += delegate { Option("Cancel", false); };
        setFireButton.clicked += delegate { SetFire(); };

        runGunToggle.RegisterValueChangedCallback(evt => RunAndGunToggle());
        secondaryToggle.RegisterValueChangedCallback(evt => SecondaryToggle());

        panel.style.display = DisplayStyle.None;
    }

    public static void showPanel() =>
        panel.style.display = DisplayStyle.Flex;

    public static void hidePanel() =>
        panel.style.display = DisplayStyle.None;

    public static void toggleMoveButton(bool off)
    {
        if (off)
        {
            instance.moveButton.style.backgroundColor = Color.gray;
            instance.moveButton.focusable = false;
        }
        else
        {
            instance.moveButton.focusable = true;
            instance.moveButton.style.backgroundColor = new Button().style.backgroundColor;
        }
    }

    public void Option(string option, bool shouldMove)
    {
        if (pressShift)
        {
            Debug.Log("SHIFT");
            ActionType item = new ActionType(option, shouldMove);
            foreach (Pawn p in Player.ourSelectedPawns)
            {
                p.actionTypes.Add(item);
            }
        }
        else
        {
            ActionType item;
            if (option != "Move")
                item = new ActionType(option, shouldMove);
            else if(option == "Cancel")
            {
                item = null;
                foreach(Pawn p in Player.ourSelectedPawns)
                {
                    p.actionTypes.Clear();
                }
            }
            else
            {
                Vector2Int center = Vector2Int.FloorToInt(Player.selectedTilePoses[Player.selectedTilePoses.Count/2]);
                Pos point = PathfindExtra.FindNearest(center);

                item = new ActionType(option, shouldMove, new Vector2Int(point.x, point.y), false);
            }
            //Debug.Log($"SEL:{Player.ourSelectedPawns.Count}");
            foreach (Pawn p in Player.ourSelectedPawns) 
            {
                Debug.Log($"adding {option} to {p.pname}");
                p.actionTypes.Clear();
                if(item!=null)
                    p.actionTypes.Add(item);
            }
        }
        print("Doing ActionType: " + option);
        if (option == "FollowCursor")
        {
            print("{ADD (uh oh dont do this pls)}Beginning following cursor");
            Player.isFollowingCursor = true;
        }

        Player.selectedPawns.Clear(); // ez way to make the game feel better
    }

    public void RunAndGunToggle()
    {
        ActionType item = new ActionType("RunAndGun", false ,runGunToggle.value);
        foreach (Pawn p in Player.ourSelectedPawns)
        {
            p.actionTypes.Add(item);
        }

        Debug.Log($"Toggled run&gun for {Player.ourSelectedPawns.Count} pawns.");
    }

    public void SecondaryToggle()
    {
        foreach (Pawn p in Player.ourSelectedPawns)
        {
            Weapon w = secondaryToggle.value ? p.heldSidearm : p.heldPrimary;
            p.activeWeapon = w; 
        }

        Debug.Log($"Toggled secondary for {Player.ourSelectedPawns.Count} pawns.");
    }

    public void SetFire()
    {
        Messages.AddMessage("Select a block to set fire to. The selected pawn \n within one tile will do it.");
        selectTileFlag = true;
    }

    public static string GetUntilOrEmpty(string text, string stopAt)
    {
        if (!String.IsNullOrWhiteSpace(text))
        {
            int charLocation = text.IndexOf(stopAt, StringComparison.Ordinal);

            if (charLocation > 0)
            {
                return text.Substring(0, charLocation);
            }
        }

        return String.Empty;
    }
}
