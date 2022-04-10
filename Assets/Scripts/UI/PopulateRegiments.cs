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

    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        panel = root.Q<VisualElement>("Regiments").Q<VisualElement>("unity-content-and-vertical-scroll-container").Q<VisualElement>("unity-content-viewport").Q<VisualElement>("unity-content-container");

        FillRegiments();
    }

    void FillRegiments()
    {
        List<Regiment> l = RegimentManager.RegimentList; // TODO: this will probably cause issues later. it being here you know
        print("Found " + l.Count + " regiments and displaying.");
        //for (int i = 0; i < l.Count; i++)
        int currentLoop = 0;
        foreach (Regiment r in l)
        {
            if (l[currentLoop].countryOrigin.Equals(Player.playerCountry))
            {
                SetupPanel(currentLoop, regiment.CloneTree());
            }
            currentLoop++;
        }
    }

    void SetupPanel(int id, VisualElement regiment)
    {
        VisualElement regimentnew = regiment;
        regimentnew.Q<Label>("RegimentTypeID").text = (RegimentManager.Get(id).type.Name+" | "+(id + 1)).ToString();
        regimentnew.Q<Label>("RegimentMemberCount").text = (RegimentManager.Get(id).members.Count).ToString();
        regimentnew.RegisterCallback<MouseDownEvent,int>(selectRegiment,id);
        panel.Add(regimentnew);
    }

    void selectRegiment(MouseDownEvent evt, int id)
    {

        if (Player.regimentSelectNumber == 0)
        {
            print("Selecting pawns via regiment. ID:" + id);
            if (Input.GetKey(KeyCode.LeftShift)) // todo keybind
                Player.ourSelectedPawns.AddRange(RegimentManager.Get(id).members);
            else
                Player.ourSelectedPawns = RegimentManager.Get(id).members;
            if (Player.ourSelectedPawns.Count == 0)
            {
                Messages.AddMessage("No pawns selected");
            }
        }
        else
        {
            print($"Selecting {Player.regimentSelectNumber} pawns via regiment.");
            List<Pawn> temp = GetRandomElements<Pawn>(RegimentManager.Get(id).members, Player.regimentSelectNumber);
            if (Input.GetKey(KeyCode.LeftShift)) // todo keybind
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
