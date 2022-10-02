using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using Buildings;

public enum TileType { Full=0, Random, Rule }
public partial class ScenarioUI: MonoBehaviour
{
    List<(System.Type type, string name, TargetLayer layer, string list)> Categories = new List<(System.Type, string, TargetLayer, string)>()
    {
        (typeof(TerrainType), "Terrain", TargetLayer.Terrain, "tilemap"),
        (typeof(Building), "Wall", TargetLayer.Solid, "buildings"),
        (typeof(Floor), "Floors", TargetLayer.Terrain, "floors"),
        (typeof(Roof), "Rooves", TargetLayer.Roof, "rooves"),
        (typeof(Door), "Doors", TargetLayer.Solid, "doors"), 
        (typeof(Furniture), "Furniture", TargetLayer.Solid, "furnitures"),
        (typeof(Trap), "Traps", TargetLayer.Solid, "traps")
    };
    [SerializeField] UIDocument uidoc;
    VisualElement root;
    [SerializeField] VisualTreeAsset item;
    bool state = false;
    List<(System.Type t, List<VisualElement> v)> els = new List<(System.Type, List<VisualElement>)>();
    Button paintbrush;
    Button pencil;
    Button bucket;
    Button line;
    VisualElement previousVE;
    // Start is called before the first frame update
    protected void Start()
    {
        root = uidoc.rootVisualElement;
        AddCategories();

        root.Q<Button>("Clear").clicked += Unselect;
        root.Q<Toggle>("ShowRooves").RegisterValueChangedCallback(delegate { Player.isRoofShow = root.Q<Toggle>("ShowRooves").value; RoofPlacer.I.Refresh(); });
        root.Q<Toggle>("EraserMode").RegisterValueChangedCallback(delegate { Placer.eraserMode = root.Q<Toggle>("EraserMode").value; });

        pencil = root.Q<Button>("Pencil"); pencil.clicked += delegate { SwitchBrush(Brush.Pencil, pencil); };
        paintbrush = root.Q<Button>("Brush"); paintbrush.clicked += delegate { SwitchBrush(Brush.Brush, paintbrush); };
        bucket = root.Q<Button>("Bucket"); bucket.clicked += delegate { SwitchBrush(Brush.Bucket, bucket); };
        line = root.Q<Button>("Line"); line.clicked += delegate { SwitchBrush(Brush.Box, line); };

        root.Q<TextField>("Search").RegisterValueChangedCallback(delegate { Search(root.Q<TextField>("Search").value); });
        root.Q<TextField>("Search").Blur();

        SwitchBrush(Brush.Pencil, pencil);
    }

    protected void Update()
    {
        if (Input.GetKeyDown(Keybinds.Escape)) Unselect();
    }

    public void Search(string text)
    {
        root.Q<ScrollView>("Body").Clear();
        foreach(var dasd in els)
        {
            foreach(VisualElement v in dasd.v)
            {
                if (v.Q<Label>("Name").text.ToLower().Contains(text.ToLower()))
                    root.Q<ScrollView>("Body").Add(v);
            }
        }
    }

    void Unselect()
    {
        Placer.canPlace = false; Placer.currentItem = null; Placer.PlacedItem = null;
        Player.selectedPawns.Clear();
        Player.ourSelectedPawns.Clear();
        Player.selectedTilePoses.Clear();
        Player.selectedTiles.Clear();
    }

    void SwitchBrush(Brush brush, Button b)
    {
        Placer.BrushType = brush;
        b.style.backgroundColor = Color.yellow;
        if (b != pencil) pencil.style.backgroundColor = new StyleColor(StyleKeyword.Auto);
        if (b != bucket) bucket.style.backgroundColor = new StyleColor(StyleKeyword.Auto);
        if (b != line) line.style.backgroundColor = new StyleColor(StyleKeyword.Auto);
        if (b != paintbrush) paintbrush.style.backgroundColor = new StyleColor(StyleKeyword.Auto);
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
            var total = (IList)category.type.GetField("List").GetValue(category.type);
            // having to hard-code water + mountains in sucks.
            if (category.type == typeof(TerrainType))
            { 
                total.Insert(0,MapGenerator.terrainTypes.Find(x=>x.type == SpecialType.Water));
                total.Insert(1,MapGenerator.terrainTypes.Find(x=>x.type == SpecialType.Mountain));
            }
            foreach(object o in total)
            {
                VisualElement ve = item.CloneTree();
                ve.Q<VisualElement>("Image").style.backgroundImage = XMLLoader.Loaders.LoadTex(((Item)o).SourceFile);
                ve.Q<Label>("Name").text = "<u>" + ((Item)o).Name + "</u>";
                ve.Q<Label>("Name").RegisterCallback<MouseDownEvent>(delegate { ItemViewer.I.DisplayItem(o); });
                ve.RegisterCallback<MouseDownEvent>(delegate {
                    if(category.type != typeof(Furniture) && category.type != typeof(Door))
                    {
                        TileBase item = (TileBase)o.GetType().GetField("tile").GetValue(o);
                        if (category.type == typeof(TerrainType) && (((Item)o).Name) == "Mountain")
                            item = (TileBase)o.GetType().GetField("thisIsVeryBadSpaghettiButImOutOfIdeas").GetValue(o);
                        Placer.furnitureMode = false;
                        Placer.PlacedItem = item;
                    }
                    else
                    {
                        Tile[,] item = (Tile[,])o.GetType().GetField("tile").GetValue(o);
                        Placer.furnitureMode = true;
                    }
                    if(previousVE != null)
                        previousVE.Q<VisualElement>("Bok").style.backgroundColor = new StyleColor(StyleKeyword.Initial);
                    ve.Q<VisualElement>("Bok").style.backgroundColor = new StyleColor(Color.yellow);
                    previousVE = ve;

                    Placer.targetLayer = category.layer;
                    Placer.currentMap = (Item[,])typeof(TilemapPlace).GetField(category.list).GetValue(typeof(TilemapPlace));
                    Placer.currentItem = (Item)o;
                    FirePlacer.active = false;
                    Placer.canPlace = true;
                });
                els.els.Add(ve);
            }
            this.els.Add(els);
            root.Q<ScrollView>("Categories").Add(button);
        }
        Button fireButton = new Button(delegate { Unselect(); root.Q<ScrollView>("Body").Clear(); FirePlacer.active = true; });
        fireButton.text = "Fire";
        root.Q<ScrollView>("Categories").Add(fireButton);
    }
}
