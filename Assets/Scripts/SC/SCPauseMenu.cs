using Countries;
using Regiments;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SCPauseMenu : MonoBehaviour
{
    [SerializeField] VisualTreeAsset PAUSEMENU;
    VisualElement root;
    public static SCPauseMenu I;
    [HideInInspector] public bool isOpen = false;
    public static string Name;
    public static string Description;
    public static string LocalPath;

    private void OnUiChange()
    {
        // no inbattle check. theres no dontddestroyonload
        UIManager.TransferToNewUI(I.PAUSEMENU.CloneTree().Q<VisualElement>("PauseMenuParent"), "PauseMenuParent");
        I.root = UIManager.ui.rootVisualElement;
        VisualElement si = I.root.Q<VisualElement>("ScenarioInfo");

        I.root.Q<VisualElement>("PauseMenuParent").style.visibility = Visibility.Hidden;
        I.root.Q<VisualElement>("PauseMenuParent").Q<Button>("ResumeButton").clicked += Resume;
        I.root.Q<VisualElement>("PauseMenuParent").Q<Button>("MainMenuButton").clicked += delegate { StartCoroutine(GoToMainMenu()); };
        I.root.Q<Button>("EditScenarioInfo").clicked += delegate { si.style.display = DisplayStyle.Flex; si.style.visibility = Visibility.Visible; };
        I.root.Q<Button>("EditScenarioButton").clicked += ChangeScenarioSettings;
        I.root.Q<Button>("SaveButton").clicked += delegate { SaveScenario.SaveScenarioToPath(Application.persistentDataPath + "\\bsidk.xml"); };
        I.root.Q<Button>("SaveStrucButton").clicked += delegate { SaveScenario.SaveStructure(Application.persistentDataPath + "\\bsstruc.xml"); };

        si.Q<Button>("SIClose").clicked += delegate { si.style.display = DisplayStyle.None; si.style.visibility = Visibility.Hidden; };
        UIManager.MakeDraggable(si, si.Q<VisualElement>("Title"));
        si.Q<TextField>("Name").RegisterValueChangedCallback(delegate { Name = si.Q<TextField>("Name").value; });
        si.Q<TextField>("Description").RegisterValueChangedCallback(delegate { Description = si.Q<TextField>("Description").value; });
        si.Q<TextField>("LocalFilePath").RegisterValueChangedCallback(delegate { LocalPath = si.Q<TextField>("LocalFilePath").value; });
    }

    protected void Start()
    {
        if (I == null)
        {
            I = this;
            UIManager.OnUiChange += I.OnUiChange;
            OnUiChange();
        }
    }

    void ChangeScenarioSettings()
    {
        Menus.I.inSC = false;
        Menus.I.scenLoad = true;
        SaveScenario.SaveScenarioToPath(Application.persistentDataPath + "\\scenarios\\temp.xml");
        Menus.I.scenarioBackupName = Menus.I.currentScenarioFilename;
        Menus.I.currentScenarioFilename = "temp";
        StartCoroutine(Loading.I.load("MainMenu"));
    }

    public void Show()
    {
        I.isOpen = true;
        if (!UIManager.UIHidden)
            UIManager.ToggleUI();
        I.root.Q<VisualElement>("PauseMenuParent").style.visibility = Visibility.Visible;
    }

    public void Resume()
    {
        I.isOpen = false;
        if (UIManager.UIHidden)
            UIManager.ToggleUI();

        I.root.Q<VisualElement>("PauseMenuParent").style.visibility = Visibility.Hidden;
    }

    public IEnumerator GoToMainMenu()
    {
        // TODO: SAVE IF THIS IS A BATTLE IN WORLD MAP
        PawnManager.GetAll().ForEach(p => Destroy(p.gameObject));
        foreach (Country c in Country.List)
        {
            c.members.Clear();
            c.memberTransforms.Clear();
        }
        Regiment.List.Clear();
        Menus.I.inBattle = false;
        Player.enemies.Clear();
        Player.friends.Clear();
        StartCoroutine(Loading.I.load("MainMenu"));
        yield return new WaitUntil(() => Loading.I.done);
        Menus.I.SwitchTo(Menus.I.mainMenu, null);
        Loading.I.done = false;
    }

    protected void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))// && !CameraMove.I.isFollowing ) // && what else TODO
            if (I.isOpen)
                I.Resume();
            else
                I.Show();
    }
}
