using Body;
using Armors;
using PawnFunctions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum PawnShockRating { Good, Warning, Bad}
public class PawnInfo : MonoBehaviour
{
    VisualElement root;
    VisualElement panel;
    VisualElement hpanel;
    VisualElement vitalsbox;
    VisualElement bplist;
    Label painLabel;

    public static Pawn currentSelectedPawn; // useful for mods too
    public Button closeButton;

    public VisualTreeAsset bodypart;
    public VisualTreeAsset vital;

    public VisualTreeAsset armorPiece;

    VisualElement exp;

    protected void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        panel = root.Q<VisualElement>("PawnInfoParent");
        hpanel = panel.Q<VisualElement>("Body").Q<VisualElement>("Bodyadada").Q<VisualElement>("Health");
        vitalsbox = panel.Q<VisualElement>("VitalStatuses");
        painLabel = vitalsbox.Q<Label>("PainLabel");

        closeButton = panel.Q<Button>("CloseButton");
        closeButton.clicked += delegate { close(); };

        bplist = hpanel.Q<VisualElement>("unity-content-container");
        panel.style.display = DisplayStyle.None;

        UIManager.MakeDraggable(panel, panel.Q<VisualElement>("TitleBar"));

        exp = root.Q<VisualElement>("ExtraPawnInfoParent");
        exp.Q<Button>("CloseButton").clicked += delegate
        {
            exp.style.visibility = Visibility.Hidden;
        };
        UIManager.MakeDraggable(exp, exp.Q<VisualElement>("Title"));
    }

    public void ShowPawnInfo(Pawn p)
    {
        /*foreach (Transform child in viewport.transform) // rfresh
        {
            GameObject.Destroy(child.gameObject);
        }*/
        currentSelectedPawn = p;
        panel.style.display = DisplayStyle.Flex; // wtf?
        foreach(VisualElement v in panel.Children()) // wtf?
        {
            v.visible = true;
        }
        panel.visible = true;

        panel.Q<Button>("MoreInfo").clicked += delegate { ShowExtraPawnInfo(p); };
        hpanel.style.display = DisplayStyle.Flex;
        panel.Q<Label>("QuickInfo").text = $"{p.pname} | {p.country.memberName} {p.troopType.Name} | [sword] {p.meleeSkill} [bow] {p.rangeSkill}";
        UpdateHealth(p.healthSystem.bodyparts, p.healthSystem.pain);
        UpdateVitals(p.healthSystem.vitals, p.healthSystem.pain);
        UpdateShock(p.healthSystem.userFriendlyStatus, p.healthSystem.statusType);
    }
    
    public void ShowExtraPawnInfo(Pawn p)
    {
        // TODO TODO TODO TODO 
        // ITEM VIEWER SUPPORT
        exp.style.visibility = Visibility.Visible;

        string pname = p.hasPrimary ? p.heldPrimary.Name : "None";
        string sname = p.hasSidearm ? p.heldSidearm.Name : "None";
        string ssname = p.hasShield ? p.shield.Name : "None";

        exp.Q<Label>("Weapon").text = $"<u>Weapon: {pname}</u>";
        exp.Q<Label>("Sidearm").text = $"<u>Sidearm: {sname}</u>";
        exp.Q<Label>("Shield").text = $"<u>Shield: {ssname}</u>";
        exp.Q<Label>("Kills").text = $"Kills: {p.killCount}";

        foreach(Armor a in p.armor)
        {
            VisualElement v = armorPiece.CloneTree();
            v.Q<Label>("Armor").text = $"<u>{a.Name}</u>";
            exp.Q<VisualElement>("Right").Add(v);
        }
    }

    public void UpdateHealth(List<Bodypart> bps, float pain)
    {
        bplist.Clear();
        foreach (Bodypart b in bps) // thank god there was a bug here it helped me find that i was making a left right leg and a 5th 4th toe
        {
            if (b.wounds.Count > 0)
                ShowBodyPart(b);   
        }
        string painparsed = pain > 0 ? (pain*100).ToString()+"%" : "None";
        painLabel.text = "Pain: "+painparsed;
        //UpdateShock("");
    }

    public void UpdateVitals(List<Vital> vitals, float pain)
    {
        string parsedVitals = "";
        foreach (Vital v in vitals)
        {
            parsedVitals += ParseVital(v);
        }

        vitalsbox.Q<Label>("SystemsLabel").text = parsedVitals;
        // ---------
        string painparsed = pain > 0 ? (pain*100).ToString() : "None";
        panel.Q<Label>("PainLabel").text = "Pain: " + painparsed;
    }

    public void UpdateShock(string type, PawnShockRating typeColor)
    {
        var color = typeColor switch
        {
            PawnShockRating.Warning => new Color(192, 181, 46),
            PawnShockRating.Bad => new Color(192, 54, 46),
            PawnShockRating.Good => new Color(46, 192, 88),
            _ => new Color(46, 192, 88),
        };
        panel.Q<VisualElement>("ShockPanel").style.backgroundColor = color;
        panel.Q<Label>("ShockLabel").text = type;
    }

    void ShowBodyPart(Bodypart b)
    {
        VisualElement bodypartnew = bodypart.CloneTree();//
        bodypartnew.Q<Label>("Info").text = $"{b.Name}  |  {b.HP}/{b.TotalHP}";
        if (b.bleedingRate <= 0)
        {
            bodypartnew.Q<VisualElement>("BloodIcon").style.display = DisplayStyle.None;
            bodypartnew.Q<Label>("Info2").text = "";
        }
        else
        {
            bodypartnew.Q<VisualElement>("BloodIcon").style.display = DisplayStyle.Flex;
            bodypartnew.Q<Label>("Info2").text = b.bleedingRate.ToString();
        }

        string woundText = "";
        foreach (Wound w in b.wounds) 
        {
            woundText += ParseWound(w) + "\n";
        }
        bodypartnew.tooltip = woundText;
        bodypartnew.AddManipulator(new ToolTipManipulator());
        
        bplist.Add(bodypartnew);
    }

    string ParseWound(Wound w)
    {
        string one = w.sourceAttack.Name.ToString();
        string two = w.sourceWeapon.Name.ToString();
        string three = w.damage.ToString();

        string four="";
        string fivesixseveneight="";
        string MSinmybankaccount="";
        string o = w.bleedRate > 0 ? w.bleedRate.ToString() : "";

        return one + $"({two}):" + three + " | Bleed:"+o+four+fivesixseveneight+MSinmybankaccount;
    }

    string ParseVital(Vital v)
    {
        return v.system + "  :  " + (v.effectiveness*100)+"%\n";
    }
    
    void close() =>
        panel.style.display = DisplayStyle.None;
}
