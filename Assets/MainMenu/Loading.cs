using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour, IMenu
{
    public static Loading I = null;

    private string _status;
    public string Status 
    {
        get { return _status; } 
        set 
        { 
            _status = value;
            progress.text = value;
        } 
    }
    public bool done { get; internal set; }
    Label progress;
    Label tip;
    [SerializeField] UIDocument thisdoc;

    public List<string> tips = new List<string>();
    public void Back() { }

    protected void Start()
    {
        progress = thisdoc.rootVisualElement.Q<Label>("Progress");
        tip = thisdoc.rootVisualElement.Q<Label>("Tip");
        I = this;

        // todo: world map tips if you own the dlc
        tips.AddRange(new string[] { "You can change keybinds in settings.", "Generals have personalities.", "You can select pawns and then drag on one for it to follow a path.", 
        "If you don't want to baby a group of pawns, you can have an AI make moves for them. You can also choose the personality of the AI.", "You can hide the UI.", 
        "Melee attacks from an animal going full speed will multiply damage.", "You can't go through doors if you're on an animal.",
        "Attacks from stationary animals do 25% less damage.", 
        "Different terraintypes have different walkspeeds. Use this to plan a fast route around your enemy.",
        "You can drag around many GUI elements.", "You can add to your current pawn selection, subtract, or remove it.", 
        "Troops can spawn inside a base, around it, or just outside it. Watch out!", "Some weapons have a warmup time.", 
        "You can reorder regiments in the regiment viewer. You can also change the default sorting method.", 
        "If you hold CTRL while right click and dragging, you can select a large box for an area your pawns should go to.",
        "If you hold shift while selecting pawns or tiles, it will add to your selection. This also works for selecting regiments.",
        "Pawns can run and gun, at the cost of movement speed.",
        "You can use CTRL+Scroll to change the paintbrush size in the Scenario Creator.",
        "There is rich editing in the Scenario Creator. CTRL+Z/Y, CTRL+C/V. And rotation"});
        
        StartCoroutine(Tips());
    }

    public IEnumerator Tips()
    {
        tip.text = "<b>TIP: </b>" + tips[Random.Range(0,tips.Count)];
        yield return new WaitForSecondsRealtime(7);
        StartCoroutine(Tips());
    }

    public IEnumerator load(string scene)
    {
        yield return null;

        //Begin to load the Scene you specify
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(scene);
        //Don't let the Scene activate until you allow it to
        asyncOperation.allowSceneActivation = true;
        //When the load is still in progress, output the Text and progress bar
        if (scene == "Battle")
            UIManager.Reloads++;
        while (!asyncOperation.isDone)
        {
            //Output the current progress
            //if (asyncOperation.progress > 0.99f)
            //    asyncOperation.allowSceneActivation = true;

            yield return null;
        }

        done = true;
    }
}
