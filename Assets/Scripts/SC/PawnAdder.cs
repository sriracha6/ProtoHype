using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TroopTypes;
using Regiments;
using UnityEngine.UIElements;

public enum PawnAddMode { Single, Regiment }
public class PawnAdder : MonoBehaviour
{
    enum PlaceMode { Add, Delete }
    bool deleteMode;
    internal static bool MoveMode;
    public static PawnAddMode Mode;
    public TroopType CurrentTT;
    public Regiment CurrentRegiment;
    [SerializeField] UIDocument uidoc;
    VisualElement root;
    bool isVisible;

    List<string> choices = new List<string>();
    List<TroopType> tts = new List<TroopType>();

    protected void Awake() => root = uidoc.rootVisualElement;

    protected void Start()
    {
        UIManager.MakeDraggable(root.Q<VisualElement>("AddPawn"), root.Q<VisualElement>("AddPawn").Q<VisualElement>("ATitle"));

        foreach(TroopType t in TroopType.List.FindAll(x=>Player.friends.Contains(x.country) || Player.enemies.Contains(x.country)).OrderBy(x=>x.Name))
        {
            choices.Add($"{t.country.memberName} {t.Name}");
            tts.Add(t);
        }
        root.Q<Button>("MovePawns").clicked += delegate { deleteMode = false; MoveMode = true; };
        root.Q<Button>("DeletePawns").clicked += delegate { deleteMode = true; MoveMode = false; };

        root.Q<Button>("Add1").clicked += delegate { Mode = PawnAddMode.Single; deleteMode = false; MoveMode = false; Show(); };
        root.Q<Button>("AddRegiment").clicked += delegate { Mode = PawnAddMode.Regiment; deleteMode = false; MoveMode = false; Show(); };
        root.Q<VisualElement>("ATitle").Q<Button>("CloseButton").clicked += Hide;
        root.Q<DropdownField>("TroopType").choices = choices;
        root.Q<DropdownField>("TroopType").RegisterValueChangedCallback(delegate { CurrentTT = tts[choices.FindIndex(x=>x== root.Q<DropdownField>("TroopType").value)]; });
        root.Q<TextField>("Regiment").RegisterValueChangedCallback(delegate
        {
            if (!int.TryParse(root.Q<TextField>("Regiment").text, out int result))
                Messages.AddMessage("Non number input.");
            if (Regiment.List.Exists(x => x.id == result))
                CurrentRegiment = Regiment.List.Find(x => x.id == result);
            else
                CurrentRegiment = Regiment.Create(CurrentTT, CurrentTT.country);
        });
    }

    void Show()
    {
        root.Q<VisualElement>("AddPawn").style.visibility = Visibility.Visible;
        root.Q<VisualElement>("AddPawn").style.display = DisplayStyle.Flex;
        root.Q<Label>("Mode").text = $"Mode: {Mode}";
        isVisible = true;
        if (Mode == PawnAddMode.Regiment)
            root.Q<SliderInt>("RegimentSize").style.display = DisplayStyle.Flex;
        else
            root.Q<SliderInt>("RegimentSize").style.display = DisplayStyle.None;
        Placer.canPlace = false; Placer.currentItem = null; Placer.PlacedItem = null;
    }

    void Hide()
    {
        root.Q<VisualElement>("AddPawn").style.visibility = Visibility.Hidden;
        root.Q<VisualElement>("AddPawn").style.display = DisplayStyle.None;
        Placer.canPlace = true;
        isVisible = false;
    }

    protected void Update()
    {
        if (!deleteMode)
        {
            if (isVisible && CurrentTT != null && !UIManager.mouseOverUI && !Placer.canPlace && CurrentRegiment != null && Input.GetMouseButtonDown(Keybinds.LeftMouse))
            {
                if (Mode == PawnAddMode.Single)
                    PawnManager.I.CreatePawn(false, CurrentTT.country, CachedItems.RandomName, CurrentTT, CurrentRegiment, WCMngr.I.mainCam.ScreenToWorldPoint(Input.mousePosition));
                else
                    PawnManager.I.CreateRegiment(false, CurrentTT, CurrentRegiment, root.Q<SliderInt>("RegimentSize").value, WCMngr.I.mainCam.ScreenToWorldPoint(Input.mousePosition));
            }
        }
        else if (deleteMode && Input.GetMouseButton(Keybinds.LeftMouse) && PawnFunctions.Pawn.mouseOverPawn && !UIManager.mouseOverUI)
            PawnManager.I.RemovePawn(PawnManager.allPawns.Find(p => p.thisPawnMouseOver));
    }
}
