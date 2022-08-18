using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Buildings;

public enum TileType { Full=0, Random, Rule }
public partial class ScenarioUI: MonoBehaviour
{
    List<(System.Type type, string name, TargetLayer layer)> Categories = new List<(System.Type type, string name, TargetLayer layer)>()
    {
        (typeof(TerrainType), "Terrain", TargetLayer.Terrain),
        (typeof(Building), "Wall", TargetLayer.Solid),
        (typeof(Floor), "Floors", TargetLayer.Terrain),
        (typeof(Roof), "Rooves", TargetLayer.Roof),
        (typeof(Door), "Doors", TargetLayer.Solid),
        (typeof(Furniture), "Furniture", TargetLayer.Solid),
        (typeof(Trap), "Traps", TargetLayer.Solid)
    };
    [SerializeField] UIDocument uidoc;
    VisualElement root;
    [SerializeField] VisualTreeAsset item;
    bool state = false;
    List<(System.Type t, List<VisualElement> v)> els = new List<(System.Type, List<VisualElement>)>();
    // Start is called before the first frame update
    protected void Start()
    {
        root = uidoc.rootVisualElement;
        AddCategories();
    }

    void SwitchToCategory(System.Type category)
    {
        root.Q<ScrollView>("Body").Clear();
        foreach(var s in els.Find(x=>x.t==category).v)
        {
            root.Q<ScrollView>("Body").Add(s);
        }
    }

    void AddCategories()
    {
        foreach(var category in Categories)
        {
            Button button = new Button(delegate { SwitchToCategory(category.type); });
            button.text = category.name;
            (System.Type t, List<VisualElement> els) els = (category.type, new List<VisualElement>());
            foreach(object o in (IList)category.type.GetField("List").GetValue(category.type))
            {
                VisualElement ve = item.CloneTree();
                ve.Q<VisualElement>("Image").style.backgroundImage = XMLLoader.Loaders.LoadTex(((Item)o).SourceFile);
                ve.Q<Label>("Name").text = "<u>" + (((Item)o).Name) + "</u>";
                ve.Q<Label>("Name").RegisterCallback<MouseDownEvent>(delegate { ItemViewer.I.DisplayItem(o); });
                ve.RegisterCallback<MouseDownEvent>(delegate {
                    Placer.PlacedItem = (UnityEngine.Tilemaps.TileBase)o.GetType().GetField("tile").GetValue(o);
                    Placer.targetLayer = category.layer;
                });
                els.els.Add(ve);
            }
            this.els.Add(els);
            root.Q<ScrollView>("Categories").Add(button);
        }
    }
}
