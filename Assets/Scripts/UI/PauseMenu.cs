using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] VisualTreeAsset PAUSEMENU;
    [SerializeField] VisualTreeAsset LINK;
    [SerializeField] VisualTreeAsset ITEM;
    VisualElement root;
    public static PauseMenu I;
    public bool isOpen = false;

    private void OnUiChange()
    {
        // no inbattle check. theres no dontddestroyonload
        UIManager.TransferToNewUI(I.PAUSEMENU.CloneTree().Q<VisualElement>("PauseMenuParent"), "PauseMenuParent");

        I.root = UIManager.ui.rootVisualElement;
        I.root.Q<VisualElement>("PauseMenuParent").style.visibility = Visibility.Hidden;
        I.root.Q<VisualElement>("PauseMenuParent").Q<Button>("ResumeButton").clicked += Resume;
        I.root.Q<VisualElement>("PauseMenuParent").Q<Button>("MainMenuButton").clicked += GoToMainMenu;
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

        I.root.Q<VisualElement>("PauseMenuParent").Q<Label>("TotalCasualtiesLabel").text = $"Total Casualties: {Stats.TotalCasualties}";
        I.root.Q<VisualElement>("PauseMenuParent").Q<Label>("MostKillsLabel").text = $"Most Kills: <color=#6495ED><u>{Stats.PawnWithMostKills.pname}</u></color>";
        I.root.Q<VisualElement>("PauseMenuParent").Q<Label>("MostKillsLabel").RegisterCallback<MouseDownEvent>(x=>PawnInfo.I.ShowPawnInfo(Stats.PawnWithMostKills));

        Time.timeScale = 0;
    }

    public void Resume()
    {
        I.isOpen = false;
        if (UIManager.UIHidden)
            UIManager.ToggleUI();

        I.root.Q<VisualElement>("PauseMenuParent").style.visibility = Visibility.Hidden;
        Time.timeScale = TimeController.I.lastSpeed;
    }

    public void GoToMainMenu()
    {
        // TODO: SAVE IF THIS IS A BATTLE IN WORLD MAP
        Menus.I.inBattle = false;
        StartCoroutine(Loading.I.load("MainMenu"));
        Menus.I.SwitchTo(Menus.I.mainMenu);
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
