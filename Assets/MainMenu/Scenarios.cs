using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using XMLLoader;
using Countries;

public class Scenarios : MonoBehaviour, IMenu
{
    public static Scenarios I;
    VisualElement root;
    [SerializeField] UIDocument uidoc;

    [SerializeField] VisualTreeAsset SCENARIO;
    Scenario currentScenario;

    protected void Awake() =>
        I = this;

    protected void Start()
    {
        root = uidoc.rootVisualElement;
        root.Q<Button>("BackButton").clicked += Back;

        //PopulateScenarios();

        root.Q<Button>("Play").clicked += delegate { StartCoroutine(LoadFile()); };
        root.Q<DropdownField>("Difficulty").RegisterValueChangedCallback(delegate {
            if (System.Enum.TryParse(root.Q<DropdownField>("Difficulty").value, out Difficulty myStatus))
                WCMngr.I.difficulty = myStatus;
        });
        root.Q<Toggle>("AttackMode").RegisterValueChangedCallback(
            delegate {
                PawnManager.AttackMode = root.Q<Toggle>("AttackMode").value ? AttackMode.Attack : AttackMode.Defend;
                List<Country> old = new List<Country>(Player.friends);
                Player.friends = Player.enemies;
                Player.enemies = old;
            });
    }

    void SelectScenario(Scenario scenario)
    {
        currentScenario = scenario;
        root.Q<VisualElement>("Image").style.backgroundImage = Loaders.LoadTexNonWC(scenario.Directory + scenario.LocalImagePath);
        root.Q<Label>("Title").text = scenario.Name;
        root.Q<Label>("Description").text = scenario.Description;
    }

    void PopulateScenarios()
    {
        foreach(Scenario s in Scenario.List)
        {
            VisualElement v = SCENARIO.CloneTree();
            v.Q<VisualElement>("Image").style.backgroundImage = s.LocalImagePath != "" ? Loaders.LoadTexNonWC(s.Directory + s.LocalImagePath) : null;
            v.Q<Label>("Title").text = s.Name;
            v.RegisterCallback<MouseDownEvent>(delegate { SelectScenario(s); });
            root.Q<ScrollView>("Scenarios").Add(v);
        }
    }

    public void Back() =>
        Menus.I.SwitchTo(Menus.I.start, Play.I);

    IEnumerator LoadFile()
    {
        Menus.I.inBattle = true;
        StartCoroutine(Loading.I.load("Battle"));
        yield return new WaitUntil(() => Loading.I.done);
        LoadScenario.LoadScenarioFromPath(currentScenario.Filepath);
        Loading.I.done = false;
    }
}
