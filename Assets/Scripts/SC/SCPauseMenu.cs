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

    private void OnUiChange()
    {
        // no inbattle check. theres no dontddestroyonload
        UIManager.TransferToNewUI(I.PAUSEMENU.CloneTree().Q<VisualElement>("PauseMenuParent"), "PauseMenuParent");

        I.root = UIManager.ui.rootVisualElement;
        I.root.Q<VisualElement>("PauseMenuParent").style.visibility = Visibility.Hidden;
        I.root.Q<VisualElement>("PauseMenuParent").Q<Button>("ResumeButton").clicked += Resume;
        I.root.Q<VisualElement>("PauseMenuParent").Q<Button>("MainMenuButton").clicked += delegate { StartCoroutine(GoToMainMenu()); };
    }

    protected void Start()
    {
        if (I == null)
        {
            I = this;
            UIManager.I.OnUiChange += I.OnUiChange;
            OnUiChange();
        }

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
