using Body;
using PawnFunctions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PawnInfo : MonoBehaviour
{
    public VisualElement root;
    public VisualElement panel;
    public VisualElement hpanel;
    public VisualElement bplist;

    public Button closeButton;

    public VisualTreeAsset bodypart;

    private void Start()
    {
        HealthSystem.NotifyPawnBPs b = new HealthSystem.NotifyPawnBPs(UpdateHealth);
        HealthSystem.NotifyPawnInfo i = new HealthSystem.NotifyPawnInfo(UpdateVitals);
        HealthSystem.NotifyPawnShock s = new HealthSystem.NotifyPawnShock(UpdateShock);

        root = GetComponent<UIDocument>().rootVisualElement;
        panel = root.Q<VisualElement>("PawnInfo");
        hpanel = panel.Q<VisualElement>("Health");

        closeButton = panel.parent.Q<Button>("closebutton");
        closeButton.clicked += delegate { close(); };

        bplist = hpanel.Q<VisualElement>("list").Q<VisualElement>("unity-content-and-vertical-scroll-container").Q<VisualElement>("unity-content-viewport").Q<VisualElement>("unity-content-container");
        panel.style.display = DisplayStyle.None;
    }

    public void ShowPawnInfo(Pawn p)
    {
        /*foreach (Transform child in viewport.transform) // rfresh
        {
            GameObject.Destroy(child.gameObject);
        }*/
        panel.style.display = DisplayStyle.Flex;

        panel.Q<Label>("PName").text = p.pname;
        panel.Q<Label>("TrooptypeCountry").text = p.country.memberName + " " + p.troopType.Name;
        panel.Q<Label>("Skills").text = "[sword]" + p.meleeSkill + " [range]" + p.rangeSkill;

        UpdateHealth(p.healthSystem.bodyparts, p.healthSystem.pain);
    }

    public void UpdateHealth(List<Bodypart> bps, float pain)
    {
        bplist.Clear();
        foreach (Bodypart b in bps)
        {
            if (b.wounds.Count > 0)
            {
                ShowBodyPart(b);   
            }
        }
        string painparsed = pain > 0 ? (pain*100).ToString()+"%" : "None";
        UpdateShock("");
        hpanel.Q<Label>("pain").text = "Pain: "+painparsed;
    }

    public void UpdateVitals(List<Vital> vitals, float pain)
    {
        string vitalsParsed = "";
        foreach(Vital v in vitals)
        {
            vitalsParsed += ParseVital(v);
        }

        hpanel.Q<Label>("vitaltext").text = vitalsParsed;
        // ---------
        string painparsed = pain >= 0 ? (pain*100).ToString() : "None";
        hpanel.Q<Label>("pain").text = "Pain: " + painparsed;
    }

    public void UpdateShock(string type)
    {
        panel.Q<Label>("shockText").text = type; // todo: rich text pls
    }

    void ShowBodyPart(Bodypart b)
    {
        VisualElement bodypartnew = bodypart.CloneTree();//
        bodypartnew.Q<Label>("Name").text = b.Name;
        bodypartnew.Q<Label>("hpMakeThisABar").text = b.HP+"/"+b.TotalHP;
        bodypartnew.Q<Label>("bleedrate").text = "error";

        VisualElement woundList = bodypartnew.Q<VisualElement>("unity-content-container");
        foreach (Wound w in b.wounds) 
        {
            Label labeel = new Label(ParseWound(w));
            labeel.enableRichText = true;
            labeel.style.fontSize = 10f;
            woundList.Add(labeel);
        }
        bplist.Add(bodypartnew);
    }

    string ParseWound(Wound w)
    {
        string h = w.sourceAttack.Name.ToString();
        string e = w.sourceWeapon.Name.ToString();
        string l = w.damage.ToString();
        string ll;
        string o = w.bleedRate > 0 ? w.bleedRate.ToString() : "";

        return h + $"({e}):" + l + "Bleed:"+o;
    }

    string ParseVital(Vital v)
    {
        return v.system + ":\n" + (v.effectiveness*100)+"%\n";
    }
    
    void close()
    {
        panel.style.display = DisplayStyle.None;
    }
}
