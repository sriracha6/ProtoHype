using Body;
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

    private void Start()
    {
        //HealthSystem.NotifyPawnBPs b = new HealthSystem.NotifyPawnBPs(UpdateHealth);
        //HealthSystem.NotifyPawnInfo i = new HealthSystem.NotifyPawnInfo(UpdateVitals);
        //HealthSystem.NotifyPawnShock s = new HealthSystem.NotifyPawnShock(UpdateShock);

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

        /*vitalsbox.Add(vital.CloneTree()); // for pain
        for(int i = 0; i < Loader.loader.defaultVitals.Count; i++)
        {
            var v = vital.CloneTree();
            vitalLabelsBCGAY.Add(v.Q<Label>("Text"));
            vitalsbox.Add(v);
        }*/
    }

    public void ShowPawnInfo(Pawn p)
    {
        /*foreach (Transform child in viewport.transform) // rfresh
        {
            GameObject.Destroy(child.gameObject);
        }*/
        currentSelectedPawn = p;
        panel.style.display = DisplayStyle.Flex;

        panel.Q<Label>("QuickInfo").text = $"{p.pname} | {p.country.memberName} {p.troopType.Name} | [sword] {p.meleeSkill} [bow] {p.rangeSkill}";

        UpdateHealth(p.healthSystem.bodyparts, p.healthSystem.pain);
        UpdateVitals(p.healthSystem.vitals, p.healthSystem.pain);
        UpdateShock(p.healthSystem.userFriendlyStatus, p.healthSystem.statusType);
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
        Color color;
        switch (typeColor)
        {
            case PawnShockRating.Warning:
                color = new Color(192,181,46);
                break;
            case PawnShockRating.Bad:
                color = new Color(192,54,46);
                break;
            default:
                color = new Color(46,192,88);
                break;
        }
        var a = panel.Q<VisualElement>("ShockPanel").style.backgroundColor.value;
        a = color; // todo: rich text pls
        panel.Q<Label>("ShockLabel").text = type; // todo: rich text pls
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
