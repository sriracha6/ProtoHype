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
    public static PawnInfo I;
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
        if (I == null)
            I = this;

        I.root = GetComponent<UIDocument>().rootVisualElement;
        I.panel = I.root.Q<VisualElement>("PawnInfoParent");
        I.hpanel = I.panel.Q<VisualElement>("Body").Q<VisualElement>("Bodyadada").Q<VisualElement>("Health");
        I.vitalsbox = I.panel.Q<VisualElement>("VitalStatuses");
        I.painLabel = I.vitalsbox.Q<Label>("PainLabel");

        I.closeButton = I.panel.Q<Button>("CloseButton");
        I.closeButton.clicked += delegate { I.close(); };

        I.bplist = I.hpanel.Q<VisualElement>("unity-content-container");
        I.panel.style.display = DisplayStyle.None;
        I.panel.style.visibility = Visibility.Hidden;

        UIManager.MakeDraggable(panel, I.panel.Q<VisualElement>("TitleBar"));

        I.exp = root.Q<VisualElement>("ExtraPawnInfoParent");
        I.exp.Q<Button>("CloseButton").clicked += delegate
        {
            I.exp.style.display = DisplayStyle.None;
            I.exp.style.visibility = Visibility.Hidden;
        };
        UIManager.MakeDraggable(I.exp, I.exp.Q<VisualElement>("Title"));
    }

    public void ShowPawnInfo(Pawn p)
    {
        if(Input.GetKey(Keybinds.SelectAdd))
        {
           I.exp.style.visibility = Visibility.Visible;
           I.exp.style.display = DisplayStyle.Flex;
            I.exp.BringToFront();
            I.ShowExtraPawnInfo(p);
            return;
        }
        /*foreach (Transform child in viewport.transform) // rfresh
        {
            GameObject.Destroy(child.gameObject);
        }*/
        currentSelectedPawn = p;
        I.panel.style.display = DisplayStyle.Flex; // wtf?
        I.panel.style.visibility = Visibility.Visible; // wtf?
        I.panel.BringToFront();
        foreach(VisualElement v in I.panel.Children()) // wtf?
        {
            v.visible = true;
        }
        I.panel.visible = true;

        I.ShowExtraPawnInfo(p);

        I.panel.Q<Button>("MoreInfo").clicked += delegate { I.exp.BringToFront(); I.exp.style.visibility = Visibility.Visible; I.exp.style.display = DisplayStyle.Flex; };
        I.hpanel.style.display = DisplayStyle.Flex;

        I.panel.Q<Label>("QuickInfo").text = $"{p.pname} | {p.country.memberName} {p.troopType.Name} | M: {p.meleeSkill} R: {p.rangeSkill}";
        I.UpdateHealth(p.healthSystem.bodyparts, p.healthSystem.pain);
        I.UpdateVitals(p.healthSystem.vitals, p.healthSystem.pain);
        I.UpdateShock(p.healthSystem.userFriendlyStatus, p.healthSystem.statusType);
    }
    
    public void ShowExtraPawnInfo(Pawn p)
    {
        // TODO TODO TODO TODO 
        // ITEM VIEWER SUPPORT
        string pname = p.hasPrimary ? p.heldPrimary.Name : "None";
        string sname = p.hasSidearm ? p.heldSidearm.Name : "None";
        string ssname = p.hasShield ? p.shield.Name : "None";

       I.exp.Q<Label>("Weapon").text = $"Weapon: <u>{pname}</u>";
       I.exp.Q<Label>("Weapon").RegisterCallback<MouseDownEvent>(x=> { if(pname!="None") ItemViewer.OnLinkClick(x, p.heldPrimary); });
       I.exp.Q<Label>("Sidearm").text = $"Sidearm: <u>{sname}</u>";
       I.exp.Q<Label>("Sidearm").RegisterCallback<MouseDownEvent>(x => { if (sname != "None") ItemViewer.OnLinkClick(x, p.heldSidearm); });
       I.exp.Q<Label>("Shield").text = $"Shield: <u>{ssname}</u>";
       I.exp.Q<Label>("Shield").RegisterCallback<MouseDownEvent>(x => { if (ssname != "None") ItemViewer.OnLinkClick(x, p.shield); });
       I.exp.Q<Label>("TroopType").text = $"Troop: <u>{p.troopType}</u>";
       I.exp.Q<Label>("Kills").text = $"Kills: {p.killCount}";

       I.exp.Q<VisualElement>("Right").Clear();
        foreach (Armor a in p.armor)
        {
            VisualElement v = I.armorPiece.CloneTree();
            v.Q<Label>("Armor").text = $"<u>{a.Name}</u>";
            v.Q<Label>("Armor").RegisterCallback<MouseDownEvent>(x => ItemViewer.OnLinkClick(x, a));
           I.exp.Q<VisualElement>("Right").Add(v);
        }
    }

    public void UpdateHealth(List<Bodypart> bps, float pain)
    {
        I.bplist.Clear();
        foreach (Bodypart b in bps) // thank god there was a bug here it helped me find that i was making a left right leg and a 5th 4th toe
        {
            if (b.wounds.Count > 0)
                I.ShowBodyPart(b);   
        }
        string painparsed = pain > 0 ? (pain*100).ToString()+"%" : "None";
        I.painLabel.text = "Pain: "+painparsed;
        //UpdateShock("");
    }

    public void UpdateVitals(List<Vital> vitals, float pain)
    {
        string parsedVitals = "";
        foreach (Vital v in vitals)
        {
            parsedVitals += I.ParseVital(v);
        }

        I.vitalsbox.Q<Label>("SystemsLabel").text = parsedVitals;
        // ---------
        string painparsed = pain > 0 ? (pain*100).ToString() : "None";
        I.panel.Q<Label>("PainLabel").text = "Pain: " + painparsed;
    }

    public void UpdateShock(string type, PawnShockRating typeColor)
    {
        var color = typeColor switch
        {
            PawnShockRating.Warning => new Color32(192, 181, 46, 255),
            PawnShockRating.Bad => new Color32(192, 54, 46, 255),
            PawnShockRating.Good => new Color32(46, 192, 88, 255),
            _ => new Color32(46, 192, 88, 255),
        };
        I.panel.Q<VisualElement>("ShockPanel").style.backgroundColor = new StyleColor(color);
        I.panel.Q<Label>("ShockLabel").text = type;
    }

    void ShowBodyPart(Bodypart b)
    {
        VisualElement bodypartnew = I.bodypart.CloneTree();//
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
            woundText += I.ParseWound(w) + "\n";
        }
        bodypartnew.tooltip = woundText;
        bodypartnew.AddManipulator(new ToolTipManipulator());
        
        I.bplist.Add(bodypartnew);
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
        I.panel.style.display = DisplayStyle.None;
}
