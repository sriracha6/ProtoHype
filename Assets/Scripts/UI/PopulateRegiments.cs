using PawnFunctions;
using Regiments;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class PopulateRegiments : MonoBehaviour
{
    public VisualTreeAsset regiment;
    public VisualElement root;
    public VisualElement panel;
    public SliderInt sliderPercent;
    public Button hideButton;
    public Sprite upArrow;
    public Sprite downArrow;

    public static List<int> regimentIDOrder = new List<int>();
    public static bool hideState;

    public static List<(VisualElement panel, int id)> panels = new List<(VisualElement panel, int id)>();

    protected void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        panel = root.Q<VisualElement>("RegimentSelect").Q<VisualElement>("unity-content-container");
        sliderPercent = root.Q<SliderInt>("RegimentControlStuff");
        sliderPercent.RegisterValueChangedCallback(
            v => // THATS HWO YOU DO MULTILINE LAMBDAS AAAAAAH
            {
                Player.regimentSelectNumber = v.newValue;
                updateAllRegimentsSelectNumber(v.newValue);
            }
            );
        hideButton = root.Q<VisualElement>("RegimentShiz").Q<Button>("HideButton");
        hideButton.clicked += changeHideState;
        FillRegiments();
        Player.regimentSelectNumber = 100;
        updateAllRegimentsSelectNumber(100);
    }

    private void changeHideState()
    {
        hideState = !hideState;
        if (hideState)
        {
            root.Q<VisualElement>("RegimentShiz").Q<VisualElement>("RegimentSelect").style.display = DisplayStyle.None;
            //root.Q<VisualElement>("RegimentShiz").style.height = root.Q<VisualElement>("RegimentShiz").Q<Button>("HideButton").layout.height;
            root.Q<SliderInt>("RegimentControlStuff").style.display = DisplayStyle.None;
            hideButton.style.backgroundImage = new StyleBackground(downArrow);
        }
        else
        {
            root.Q<VisualElement>("RegimentShiz").Q<VisualElement>("RegimentSelect").style.display = DisplayStyle.Flex;
            root.Q<SliderInt>("RegimentControlStuff").style.display = DisplayStyle.Flex;
            //root.Q<VisualElement>("RegimentShiz").style.height = root.Q<VisualElement>("RegimentShiz").Q<Button>("HideButton").layout.height + root.Q<VisualElement>("RegimentShiz").Q<VisualElement>("RegimentSelect").layout.height;
            hideButton.style.backgroundImage = new StyleBackground(upArrow);
        }
    }

    public static void updateAllRegimentsSelectNumber(int newValue)
    {
        foreach ((VisualElement p, int id) in panels)
        {
            var x = Regiment.Get(id);
            p.Q<Label>("SelectX").text = $"Select {Mathf.FloorToInt(newValue / 100f * x.members.Count)}";
        }
    }
    
    void FillRegiments()
    {
        List<Regiment> l = Regiment.List; // TODO: this will probably cause issues later. it being here you know
        //for (int i = 0; i < l.Count; i++)
        int currentLoop = 0;
        foreach (Regiment r in l.OrderBy(x=>x.members.Count)) // todo: setting for how this should be ordered
        {
            if (r.countryOrigin.Equals(Player.playerCountry))
            {
                regimentIDOrder.Add(r.id);
                SetupPanel(r.id, regiment.CloneTree());
            }
            currentLoop++;
        }
    }

    void SetupPanel(int id, VisualElement regiment)
    {
        VisualElement regimentnew = regiment;
        var r = Regiment.Get(id);
        regimentnew.Q<VisualElement>("Icon").style.backgroundImage = CachedItems.renderedTroopTypes.Find(x => x.name == r.type.Icon).texture;
        regimentnew.Q<Label>("Regiment-Name-ID").text = $"{r.type.Name} | {r.id+1}";
        regimentnew.Q<Label>("MembersCount").text = $"{r.members.Count} Members";
        bool clicked = false;
        bool done = false;
        Vector2 totalDrag = Vector2.zero;

        regimentnew.RegisterCallback<MouseDownEvent>(x=>clicked=true);
        regimentnew.RegisterCallback<MouseMoveEvent>(x=>
        {
            if (clicked) OnDrag(regimentnew, x, ref done, ref totalDrag); 
        });
        regimentnew.RegisterCallback<MouseUpEvent>(x=> { clicked = false; totalDrag = Vector2.zero; done = false; });
        regimentnew.RegisterCallback<MouseLeaveEvent>(x=> { clicked = false; totalDrag = Vector2.zero; done = false; });
        
        regimentnew.RegisterCallback<MouseDownEvent, int>(selectRegiment,id);
        panel.Add(regimentnew);

        panels.Add((regimentnew, id));
    }

    void OnDrag(VisualElement v, MouseMoveEvent x, ref bool done, ref Vector2 totalDrag)
    {
        totalDrag += x.mouseDelta;
        if (done) return;

        if(totalDrag.x < -40 || totalDrag.x > 40)
        {
            int index = 0;
            foreach(var s in v.parent.Children())
            {
                if (index == panels.FindIndex(x=>x.panel==v))
                {
                    int newPos = totalDrag.x < -40 ? index - 1 : index + 1;
                    v.parent.hierarchy.Insert(Mathf.Clamp(newPos, 0, v.parent.childCount), v);
                    done = true;
                    break;
                }
                index++;
            }
        }
    }

    // changes to one = to both
    public static void selectRegiment(MouseDownEvent evt, int id)
    {
        if (Player.regimentSelectNumber == 0)
            Messages.I.Add("No pawns selected");
        else
        {
            int amount = Mathf.FloorToInt((Player.regimentSelectNumber / 100f) * Regiment.Get(id).members.Count);
            print($"Selecting {amount} pawns via regiment.");
            List<Pawn> temp = GetRandomElements<Pawn>(Regiment.Get(id).members, amount);
            if (Input.GetKey(Keybinds.SelectAdd))
                Player.ourSelectedPawns.AddRange(temp);
            else
                Player.ourSelectedPawns = temp;
        }
        Player.UpdateSelectedPawnsTint();
    }
    // changes to one = to both
    public static void selectRegiment(int visualPosition)
    {
        if (visualPosition > regimentIDOrder.Count || visualPosition < 0)
            return;

        if (Player.regimentSelectNumber == 0)
        {
            print("Selecting pawns via regiment. VPOS:" + visualPosition);
            if (Input.GetKey(Keybinds.SelectAdd))
                Player.ourSelectedPawns.AddRange(Regiment.Get(regimentIDOrder[visualPosition]).members);
            else
                Player.ourSelectedPawns = Regiment.Get(regimentIDOrder[visualPosition]).members;
            if (Player.ourSelectedPawns.Count == 0)
                Messages.I.Add("No pawns selected");
        }
        else
        {
            int amount = Mathf.FloorToInt((Player.regimentSelectNumber / 100f) * Regiment.Get(regimentIDOrder[visualPosition]).members.Count);
            print($"Selecting {amount} pawns via regiment.");
            List<Pawn> temp = GetRandomElements<Pawn>(Regiment.Get(regimentIDOrder[visualPosition]).members, amount);
            if (Input.GetKey(Keybinds.SelectAdd))
                Player.ourSelectedPawns.AddRange(temp);
            else
                Player.ourSelectedPawns = temp;
        }
        Player.UpdateSelectedPawnsTint();
    }

    public static List<T> GetRandomElements<T>(IEnumerable<T> list, int elementsCount)
    {
        return list.OrderBy(arg => Guid.NewGuid()).Take(elementsCount).ToList();
    }
}
