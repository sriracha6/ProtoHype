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

    public static List<int> regimentIDOrder = new List<int>();
    public static bool hideState;

    public static List<(VisualElement panel, int id)> panels = new List<(VisualElement panel, int id)>();

    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        panel = root.Q<VisualElement>("RegimentSelect").Q<VisualElement>("unity-content-container");
        sliderPercent = root.Q<VisualElement>("RegimentControlStuff").Q<SliderInt>("Slider");
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
            hideButton.text = "v";
        }
        else
        {
            root.Q<VisualElement>("RegimentShiz").Q<VisualElement>("RegimentSelect").style.display = DisplayStyle.Flex;
            hideButton.text = "^";
        }
    }

    public static void updateAllRegimentsSelectNumber(int newValue)
    {
        foreach ((VisualElement p, int id) v in panels)
        {
            var x = RegimentManager.Get(v.id);
            v.p.Q<Label>("Regiment-Label").text = $"{x.type.Name} ({x.id + 1}) : {x.members.Count} members";
            v.p.Q<Label>("SelectX").text = $"Select {Mathf.FloorToInt(newValue / 100f * x.members.Count)}";
        }
    }
    
    void FillRegiments()
    {
        List<Regiment> l = RegimentManager.RegimentList; // TODO: this will probably cause issues later. it being here you know
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
        var r = RegimentManager.Get(id);
        DB.Null(r.type);
        DB.Null(r.members);
        regimentnew.Q<Label>("Regiment-Label").text = $"{r.type.Name} ({id + 1}) : {r.members.Count} members";
        
        regimentnew.RegisterCallback<MouseDownEvent, int>(selectRegiment,id);
        panel.Add(regimentnew);

        panels.Add((regimentnew, id));
    }

    public static void selectRegiment(MouseDownEvent evt, int id)
    {
        if (Player.regimentSelectNumber == 0)
        {
            print("Selecting pawns via regiment. ID:" + id);
            if (Input.GetKey(Keybinds.SelectAdd))
                Player.ourSelectedPawns.AddRange(RegimentManager.Get(id).members);
            else
                Player.ourSelectedPawns = RegimentManager.Get(id).members;
            if (Player.ourSelectedPawns.Count == 0)
                Messages.AddMessage("No pawns selected");
        }
        else
        {
            int amount = Mathf.FloorToInt((Player.regimentSelectNumber / 100f) * RegimentManager.Get(id).members.Count);
            print($"Selecting {amount} pawns via regiment.");
            List<Pawn> temp = GetRandomElements<Pawn>(RegimentManager.Get(id).members, amount);
            if (Input.GetKey(Keybinds.SelectAdd))
                Player.ourSelectedPawns.AddRange(temp);
            else
                Player.ourSelectedPawns = temp;
        }
    }
    public static void selectRegiment(int visualPosition)
    {
        if (visualPosition > regimentIDOrder.Count)
            return;

        if (Player.regimentSelectNumber == 0)
        {
            print("Selecting pawns via regiment. VPOS:" + visualPosition);
            if (Input.GetKey(Keybinds.SelectAdd))
                Player.ourSelectedPawns.AddRange(RegimentManager.Get(regimentIDOrder[visualPosition]).members);
            else
                Player.ourSelectedPawns = RegimentManager.Get(regimentIDOrder[visualPosition]).members;
            if (Player.ourSelectedPawns.Count == 0)
                Messages.AddMessage("No pawns selected");
        }
        else
        {
            int amount = Mathf.FloorToInt((Player.regimentSelectNumber / 100f) * RegimentManager.Get(regimentIDOrder[visualPosition]).members.Count);
            print($"Selecting {amount} pawns via regiment.");
            List<Pawn> temp = GetRandomElements<Pawn>(RegimentManager.Get(regimentIDOrder[visualPosition]).members, amount);
            if (Input.GetKey(Keybinds.SelectAdd))
                Player.ourSelectedPawns.AddRange(temp);
            else
                Player.ourSelectedPawns = temp;
        }
    }

    public static List<T> GetRandomElements<T>(IEnumerable<T> list, int elementsCount)
    {
        return list.OrderBy(arg => Guid.NewGuid()).Take(elementsCount).ToList();
    }
}
