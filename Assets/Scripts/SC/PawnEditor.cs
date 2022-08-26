using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PawnFunctions;
using Weapons;
using Shields;
using Armors;
using Animals;
using UnityEngine.UIElements;

public enum SearchMode { Weapons, Shields, Armor, Animals }
public class PawnEditor : MonoBehaviour
{
    bool isVisible;
    [SerializeField] UIDocument uidoc;
    public VisualElement root;
    VisualElement parent;
    [SerializeField] VisualTreeAsset obj;

    List<VisualElement> currentVES = new List<VisualElement>();

    ScrollView armorView;
    List<Button> armorButtons = new List<Button>();
    Button currentArmorButton;
    int currentArmorIndex;

    SearchMode mode;
    bool isPrimary;
    Button currentButton;

    Pawn currentPawn;

    protected void Awake() => root = uidoc.rootVisualElement;

    protected void Start()
    {
        parent = root.Q<VisualElement>("PawnEditor");
        UIManager.MakeDraggable(parent, parent.Q<VisualElement>("TopBar"));
        armorView = parent.Q<ScrollView>("ArmorView");

        root.Q<Button>("Edit").clicked += Show;
        parent.Q<Button>("CloseButton").clicked += delegate
        {
            parent.style.visibility = Visibility.Hidden;
            parent.style.display = DisplayStyle.None;   
            isVisible = false;
            Placer.canPlace = true;
        };
        parent.Q<Button>("ChangePrimary").clicked += delegate { isPrimary = true; currentButton = parent.Q<Button>("ChangePrimary"); ChangeMode(SearchMode.Weapons); };
        parent.Q<Button>("ChangeSecondary").clicked += delegate { isPrimary = false; currentButton = parent.Q<Button>("ChangeSecondary"); ChangeMode(SearchMode.Weapons); };
        parent.Q<Button>("ChangeShield").clicked += delegate { currentButton = parent.Q<Button>("ChangeShield"); ChangeMode(SearchMode.Shields); };
        parent.Q<Button>("ChangeAnimal").clicked += delegate { currentButton = parent.Q<Button>("ChangeAnimal"); ChangeMode(SearchMode.Armor); };
        
        parent.Q<TextField>("ESearch").RegisterValueChangedCallback(delegate { Search(parent.Q<TextField>("ESearch").value); });

        parent.Q<Button>("AddArmor").clicked += AddArmor;
        parent.Q<Button>("SubtractArmor").clicked += SubtractArmor;
        Placer.canPlace = false; Placer.currentItem = null; Placer.PlacedItem = null;
    }

    void AddArmor()
    {
        Button b = new Button();
        int c = currentPawn.armor.Count;
        b.clicked += delegate
        {
            currentArmorButton = b;
            currentArmorIndex = c;
            ChangeMode(SearchMode.Armor);
        };
        armorButtons.Add(b);
        b.text = "[ Select Armor ]";
    }

    void SubtractArmor()
    {
        foreach (Button b in armorView.Children())
            if (b == armorButtons[armorButtons.Count - 1])
                armorView.Remove(b);
        armorButtons.RemoveAt(armorButtons.Count - 1);
    }

    void Search(string text)
    {
        parent.Q<ScrollView>("EScrollView").Clear();
        foreach (var item in currentVES)
        {
            if (item.Q<Label>("Name").text.ToLower().Contains(text.ToLower()))
                parent.Q<ScrollView>("EScrollView").Add(item);
        }
    }
    
    void Show()
    {
        Placer.canPlace = false; Placer.currentItem = null; Placer.PlacedItem = null;
        parent.style.visibility = Visibility.Visible;
        parent.style.display = DisplayStyle.Flex;
        isVisible = true;
        armorView.Clear();
        parent.Q<ScrollView>("EScrollView").Clear();
        currentVES.Clear();
        PopulateArmor();
        if(currentPawn != null)
        {
            parent.Q<Button>("ChangePrimary").text = currentPawn.hasPrimary ? "Primary: " + currentPawn.heldPrimary.Name : "[ Choose Primary ]";
            parent.Q<Button>("ChangeSecondary").text = currentPawn.hasSidearm ? "Secondary: " + currentPawn.heldSidearm.Name : "[ Choose Secondary ]";
            parent.Q<Button>("ChangeShield").text = currentPawn.hasShield ? "Shield: " + currentPawn.shield.Name : "[ Choose Shield ]";
            parent.Q<Button>("ChangeAnimal").text = currentPawn.animal != null ? "Animal: " + currentPawn.animal.sourceAnimal.Name : "[ Choose Animal ]";
        }
    }

    void PopulateArmor()
    {
        if (currentPawn == null) return;
        int i = 0;
        foreach(Armor a in currentPawn.armor)
        {
            Button b = new Button();
            b.clicked += delegate 
            {
                currentArmorButton = b; 
                currentArmorIndex = i; 
                ChangeMode(SearchMode.Armor); 
            };
            b.text = a.Name;
            armorButtons.Add(b);
            i++;
        }
    }

    void ChangeMode(SearchMode mode)
    {
        this.mode = mode;
        parent.Q<Label>("Browsing").text = $"Browsing: {mode}";

        parent.Q<ScrollView>("EScrollView").Clear();
        currentVES.Clear();
        List<Item> list = null;
        if (mode == SearchMode.Weapons) list = Weapon.List.ConvertAll<Item>(x=>x);
        if (mode == SearchMode.Shields) list = Shield.List.ConvertAll<Item>(x=>x);
        if (mode == SearchMode.Armor)   list = Armor.List.ConvertAll<Item>(x=>x);
        if (mode == SearchMode.Animals) list = Animal.List.ConvertAll<Item>(x=>x);

        foreach (Item w in list)
        {
            VisualElement ve = obj.CloneTree();
            ve.Q<Label>("Name").text = $"<u>{w.Name}</u>";
            ve.Q<Label>("Name").RegisterCallback<MouseDownEvent>(delegate { ItemViewer.I.DisplayItem(w); });
            ve.Q<VisualElement>("Image").style.backgroundImage = XMLLoader.Loaders.LoadTex(w.SourceFile);
            ve.Q<VisualElement>("Bok").RegisterCallback<MouseDownEvent>(delegate { 
                if (currentPawn == null) { Debug.Log($"returning: null"); return; }
                currentButton.text = $"{(mode == SearchMode.Weapons ? (isPrimary ? "Primary :" : "Secondary :") : "")} {w.Name}";
                if (mode == SearchMode.Weapons && isPrimary) { currentPawn.heldPrimary = (Weapon)w; currentPawn.hasPrimary = true; }
                if (mode == SearchMode.Weapons && !isPrimary) { currentPawn.heldSidearm = (Weapon)w; currentPawn.hasSidearm = true; }
                if (mode == SearchMode.Shields) { currentPawn.shield = (Shield)w; currentPawn.hasShield = true; }
                if (mode == SearchMode.Animals) PawnManager.I.AddHorse((Animal)w, currentPawn);
                if (mode == SearchMode.Armor) 
                {
                    if (currentArmorIndex > currentPawn.armor.Count)
                        currentPawn.armor.Add((Armor)w);
                    else
                        currentPawn.armor[currentArmorIndex] = (Armor)w; 
                    currentArmorButton.text = ((Armor)w).Name;
                }
                currentPawn.pawnRenderer.ReRender();
            });
            currentVES.Add(ve);
            parent.Q<ScrollView>("EScrollView").Add(ve);
        }    
    }

    protected void Update()
    {
        // middle mouse or click if open   
        if(Input.GetMouseButtonDown(Keybinds.MiddleMouse) || (Input.GetMouseButtonDown(Keybinds.LeftMouse) && isVisible && !UIManager.mouseOverUI))
        {
            currentPawn = PawnManager.GetAll().Find(x=>x.thisPawnMouseOver);
            Show();
        }
    }
}
