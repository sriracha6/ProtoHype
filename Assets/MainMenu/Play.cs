using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Play : MonoBehaviour, IMenu
{
    public static Play I;

    protected void Start()
    {
        I = this;
        var root = Menus.I.start.rootVisualElement;
        root.Q<Button>("BackButton").clicked += Back;
        root.Q<Button>("QuickBattle").clicked += QuickBattl;
        //root.Q<Button>("File").clicked += delegate { StartCoroutine(LoadFile()); };
        root.Q<Button>("Scenario").clicked += Scenario;
    }

    private void Scenario() =>
        Menus.I.SwitchTo(Menus.I.scenarios, Scenarios.I);
    public void Back() =>
        Menus.I.SwitchTo(Menus.I.mainMenu, null);

    private void QuickBattl()
    {
        QuickBattle.IsBattleMode = true;
        Menus.I.SwitchTo(Menus.I.quickstart, QuickBattle.I);
    }

    IEnumerator LoadFile()
    {
        Menus.I.inBattle = true;
        StartCoroutine(Loading.I.load("Battle"));
        yield return new WaitUntil(() => Loading.I.done);
        LoadScenario.LoadScenarioFromPath(Application.persistentDataPath + "\\scenario\\" + Menus.I.currentScenarioFilename +".xml");
        Loading.I.done = false;
    }
}
