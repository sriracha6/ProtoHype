using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Countries;
using Nature;
using Structures;
using XMLLoader;
using UnityEngine.UIElements;

public class CountryInfo
{
    public Country country;
    public VisualElement ve;
    public int regimentsCount = 30;

    public override string ToString()
    {
        return country != null ? country.Name : "NULL" + " | " + regimentsCount;
    }

    public CountryInfo(Country country, VisualElement V, int regimentsCount)
    {
        this.country = country;
        this.ve = V;
        this.regimentsCount = regimentsCount;
    }
}
public class QuickBattle : MonoBehaviour, IMenu
{
    VisualElement root;
    public List<CountryInfo> friends = new List<CountryInfo>();
    public List<CountryInfo> enemies = new List<CountryInfo>();

    public static QuickBattle I;
    static bool isba = true;
    public static bool IsBattleMode { get { return isba; } set { isba = value; I.RefreshButtons(); } }

    public int regimentSize = 30;

    protected void Awake() =>
        I = this;
    protected void Start()
    {
        root = Menus.I.quickstart.rootVisualElement;
        root.Q<Button>("BackButton").clicked += Back;
        root.Q<Button>("AddCountryLeft").clicked += delegate { AddCountry(true); };
        root.Q<Button>("AddCountryRight").clicked += delegate { AddCountry(false); };
        root.Q<Button>("SwapButton").clicked += Switcheroo;
        root.Q<SliderInt>("RegimentSize").RegisterValueChangedCallback(delegate
        {
            regimentSize = root.Q<SliderInt>("RegimentSize").value;
            UpdateEstimatedPawnCount();
        });
        root.Q<SliderInt>("MapSize").RegisterValueChangedCallback(delegate
        {
            MapGenerator.I.mapWidth = root.Q<SliderInt>("MapSize").value;
            MapGenerator.I.mapHeight = root.Q<SliderInt>("MapSize").value;
            UpdatePreview();
        });
        root.Q<TextField>("Seed").RegisterValueChangedCallback(delegate{
            MapGenerator.I.seed = root.Q<TextField>("Seed").value;
            MapGenerator.I.riverCount = 0;
            MapGenerator.I.seasidesCount = 0;
            UpdatePreview();
        });
        root.Q<DropdownField>("Biome").RegisterValueChangedCallback(delegate{
            MapGenerator.I.currentBiome = Biome.Get(root.Q<DropdownField>("Biome").value);
            MapGenerator.I.GenMap();
            UpdatePreview();
        });
        root.Q<DropdownField>("Difficulty").RegisterValueChangedCallback(delegate{
            if (System.Enum.TryParse(root.Q<DropdownField>("Difficulty").value, out Difficulty myStatus))
                WCMngr.I.difficulty = myStatus;
        });
        root.Q<DropdownField>("Time").RegisterValueChangedCallback(delegate{
            string v = root.Q<DropdownField>("Time").value;
            if (v == "Dawn") PositionSun.I.currentProgressPercent = 0.7f;
            if (v == "Day") PositionSun.I.currentProgressPercent = 1f;
            if (v == "Night") PositionSun.I.currentProgressPercent = 0f;
        });
        root.Q<Toggle>("DynamicTime").RegisterValueChangedCallback(delegate{
            WeatherManager.I.isWeather = root.Q<Toggle>("DynamicTime").value;   
            PositionSun.I.doDayCycle = root.Q<Toggle>("DynamicTime").value;
        });
        root.Q<Button>("AddSeaside").clicked += delegate {
            MapGenerator.I.seasidesCount++;
            UpdatePreview();
        };
        root.Q<Button>("AddRiver").clicked += delegate {
            MapGenerator.I.riverCount++;
            UpdatePreview();
        };
        root.Q<Button>("Edit").clicked += delegate
        {
            RunCzecks();
            Menus.I.inSC = true; // todo: pause menu
            Menus.I.SwitchTo(Menus.I.loading, Loading.I);
            StartCoroutine(Loading.I.load("ScenarioCreator"));
        };
        root.Q<Button>("Play").clicked += delegate {
            RunCzecks();
            Menus.I.inBattle = true;
            Player.playerCountry = friends[0].country;

            Player.enemies.AddRange(enemies.Select(x=>x.country));
            Player.friends.AddRange(friends.Select(x=>x.country));

            //if (string.IsNullOrEmpty(root.Q<TextField>("Seed").value))
            //    MapGenerator.I.seed = Random.Range(int.MinValue, int.MaxValue).ToString();
            ItemViewer.I.history.Clear();
            Menus.I.SwitchTo(Menus.I.loading, Loading.I);
            StartCoroutine(Loading.I.load("Battle"));
        };
        root.Q<Button>("RandomBuilding").clicked += delegate{
            TilemapPlace.UpdateBuildings();
            root.Q<SliderInt>("MapSize").lowValue = 100;
            Structure item = Structure.List.randomElement();
            Messages.AddMessage("Adding a \""+item.Name+"\"");
            MapGenerator.I.structure = item;
            StructureGenerator.PlaceStructure(item, MapGenerator.I.rand, root.Q<SliderInt>("MapSize"));
            StructureGenerator.GenerateStructure(item, MapGenerator.I.rand, root.Q<SliderInt>("MapSize"));
        };
        root.Q<DropdownField>("WeatherSelection").RegisterValueChangedCallback(delegate{
            WeatherManager.weatherQueue = (WeatherManager.WeatherType)System.Enum.Parse(typeof(WeatherManager.WeatherType), root.Q<DropdownField>("WeatherSelection").value);
        });
        root.Q<Button>("RandomSeed").clicked += delegate
        {
            root.Q<TextField>("Seed").value = Random.Range(int.MinValue, int.MaxValue).ToString();
        };

        root.Q<TextField>("Seed").value = Random.Range(int.MinValue, int.MaxValue).ToString();

        List<string> biomes = getBiomes();
        root.Q<DropdownField>("Biome").choices = biomes;
        root.Q<DropdownField>("Biome").value = biomes[0];
    }

    void RunCzecks()
    {
        if (friends.Count == 0 || enemies.Count == 0)
        {
            if (friends.Count == 0)
                UIManager.I.Flash(root.Q<Button>("AddCountryLeft"));
            if (enemies.Count == 0)
                UIManager.I.Flash(root.Q<Button>("AddCountryRight"));
            Messages.I.Add("You must have at least one country on each side");
            return;
        }
    }

    public void Back()
    {
        if(IsBattleMode)
            Menus.I.SwitchTo(Menus.I.start, Play.I);
        else
            Menus.I.SwitchTo(Menus.I.mainMenu, Play.I);
    }

    void RefreshButtons()
    {
        if(IsBattleMode)
        {
            root.Q<Button>("Edit").style.display = DisplayStyle.None;
            root.Q<Button>("Play").style.display = DisplayStyle.Flex;
        }
        else
        {
            root.Q<Button>("Edit").style.display = DisplayStyle.Flex;
            root.Q<Button>("Play").style.display = DisplayStyle.None;
        }
    }

    protected void OnBecameVisible()
    {
        if(root != null)
            root.Q<SliderInt>("RegimentSize").value = 30;
    }
    private void UpdatePreview()
    {
        MapGenerator.I.drawMode = MapGenerator.DrawMode.ColorMap;
        MapGenerator.I.GenMap();
        root.Q<VisualElement>("Preview").style.backgroundImage = MapGenerator.I.currentTexture;
    }

    private void UpdateEstimatedPawnCount()
    {
        if (regimentSize <= 0)
        {
            root.Q<SliderInt>("RegimentSize").value = 30;
            regimentSize = 30;
        }
        string text = "Est. Pawns : ";
        int est = 0;
        foreach (CountryInfo c in friends)
            for(int i = 0; i < c.regimentsCount; i++)
                est += (int)(Random.Range(0.75f, 1.25f) * regimentSize);
        foreach (CountryInfo c in enemies)
            for (int i = 0; i < c.regimentsCount; i++)
                est += (int)(Random.Range(0.75f, 1.25f) * regimentSize);

        text += est;

        root.Q<Label>("EstPawns").text = text;
    }

    private List<string> getBiomes()
    {
        List<string> b = new List<string>();
        foreach(Biome a in Biome.List)
            b.Add(a.Name);
        return b;
    }

    private void Switcheroo()
    {
        List<CountryInfo> oldfren = new List<CountryInfo>(friends);
        List<CountryInfo> olden = new List<CountryInfo>(enemies);
        var frens = new List<VisualElement>(root.Q<VisualElement>("FriendlyCountryAdder").Q<VisualElement>("unity-content-container").Children());
        var ens = new List<VisualElement>(root.Q<VisualElement>("EnemyCountryAdder").Q<VisualElement>("unity-content-container").Children());

        KillKids(root.Q<VisualElement>("FriendlyCountryAdder").Q<VisualElement>("unity-content-container"));
        BirthKids(ens, root.Q<VisualElement>("FriendlyCountryAdder").Q<VisualElement>("unity-content-container"));

        KillKids(root.Q<VisualElement>("EnemyCountryAdder").Q<VisualElement>("unity-content-container"));
        BirthKids(frens, root.Q<VisualElement>("EnemyCountryAdder").Q<VisualElement>("unity-content-container"));

        friends = olden;
        enemies = oldfren;
    }

    private void AddCountry(bool isFriendly)
    {
        UpdateEstimatedPawnCount();
        if (friends.Count + enemies.Count >= Country.List.Count) return;
        if (isFriendly && friends.Count == Country.List.Count - 1) return;
        if (!isFriendly && enemies.Count == Country.List.Count - 1) return;

        VisualElement v = Menus.I.prefab_country.visualTreeAsset.CloneTree();

        CountryInfo cinfo = new CountryInfo(null, v, 5);
        if(isFriendly)
            friends.Add(cinfo);
        else
            enemies.Add(cinfo);

        v.Q<DropdownField>("Country").choices = updateCountryList();
        v.Q<DropdownField>("Country").value = v.Q<DropdownField>("Country").choices[0];
        cinfo.country = Country.Get(v.Q<DropdownField>("Country").choices[0]);

        v.Q<Button>("Minus").clicked += delegate { removeCountry(cinfo); };
        v.Q<DropdownField>("Country").RegisterValueChangedCallback(delegate {
            cinfo.country = Country.Get(v.Q<DropdownField>("Country").value);
            updateAllCountries();
        });
        v.Q<TextField>("Count").RegisterValueChangedCallback(
            delegate {
                if (int.TryParse(v.Q<TextField>("Count").value, out int o))
                {
                    UpdateEstimatedPawnCount();
                    cinfo.regimentsCount = o;
                }
            });
        
        if(isFriendly)
            root.Q<VisualElement>("FriendlyCountryAdder").Q<VisualElement>("unity-content-container").Add(v);
        else
            root.Q<VisualElement>("EnemyCountryAdder").Q<VisualElement>("unity-content-container").Add(v);

        UpdateEstimatedPawnCount();
    }

    private List<string> updateCountryList()
    {
        List<string> v = new List<string>();
        foreach(Country c in Country.List)
        {
            if (enemies.Exists(x => x.country == c) || friends.Exists(x => x.country == c)) continue;
            else
                v.Add(c.Name);
        }
        return v;
    }

    private void updateAllCountries()
    {
        List<CountryInfo> cs = new List<CountryInfo>();
        cs.AddRange(friends);
        cs.AddRange(enemies);

        foreach(CountryInfo ci in cs)
        {
            List<string> a = ci.ve.Q<DropdownField>("Country").choices;
            List<string> names = new List<string>();
            
            foreach (CountryInfo c in friends)
                names.Add(c.country.Name);
            foreach (CountryInfo c in enemies)
                names.Add(c.country.Name);

            foreach(string s in a)
                Debug.Log($"LOLZ: {s}");
            if (a.Any(b => names.Contains(b)))
            {
                ci.ve.Q<DropdownField>("Country").choices = updateCountryList();
            }
        }
    }

    private void removeCountry(CountryInfo c)
    {
        if (friends.Contains(c) && friends.Count == 1) return;
        if (enemies.Contains(c) && enemies.Count == 1) return;

        if(friends.Contains(c))
            friends.Remove(c);
        if(enemies.Contains(c))
            enemies.Remove(c);

        c.ve.parent.Remove(c.ve);

        updateAllCountries();
    }

    private void KillKids(VisualElement v)
    {
        v.Clear();
        //foreach (VisualElement asd in v.Children())
        //    v.Remove(asd);
    }
    private void BirthKids(List<VisualElement> vs, VisualElement v)
    {
        foreach(VisualElement asd in vs)
            v.Add(asd);
    }
}
