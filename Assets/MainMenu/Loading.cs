using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    public static Loading I = null;

    AbstractProgressBar progress;
    Label tip;
    [SerializeField] UIDocument thisdoc;

    public List<string> tips = new List<string>();

    protected void Start()
    {
        progress = thisdoc.rootVisualElement.Q<AbstractProgressBar>("Progress");
        tip = thisdoc.rootVisualElement.Q<Label>("Tip");
        I = this;

        tips.Clear();
        // todo: world map tips if you own the dlc
        tips.AddRange(new string[] { "You can change keybinds in settings.", "Generals have personalities.", "You can select pawns and then drag on one for it to follow a path.", 
        "If you don't want to baby a group of pawns, you can have an AI make moves for them. You can also choose the personality of the AI.", "You can hide the UI.", 
        "Melee attacks from an animal going full speed will multiply damage.", "You can't go through doors if you're on an animal.",
        "Attacks from stationary animals do 25% less damage.", "Different terraintypes have different walkspeeds. Use this to plan a fast route around your enemy.",
        "You can drag around many GUI elements.", "You can add to your current pawn selection, subtract, or remove it."});
        
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
        while (!asyncOperation.isDone)
        {
            //Output the current progress
            progress.value = asyncOperation.progress * 100;

            //if (asyncOperation.progress > 0.99f)
            //    asyncOperation.allowSceneActivation = true;

            yield return null;
        }
    }
}
