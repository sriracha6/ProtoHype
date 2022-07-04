using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Messages : MonoBehaviour
{
    public VisualElement root;
    public VisualElement box;

    [SerializeField]
    float timeToFade;

    private List<string> messages = new List<string>();

    public static Messages I;

    private void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        box = root.Q<VisualElement>("MessageSystem");
        box.style.display = DisplayStyle.None;
        box.parent.pickingMode = PickingMode.Ignore;

        I = this;
    }

    public void Add(string message)
    {
        messages.Add(message);
        UpdateMsgs();
        int index = messages.Count == 1 ? messages.Count - 1 : 0;
        StartCoroutine(autoRemoveText(index));
    }
    
    /// <summary>
    /// Are you dumb??? it puts a message in the message box.
    /// </summary>
    /// <param name="obj"></param>
    public static void AddMessage(object obj)
    {
        I.Add(obj.ToString()); // giggity giggity goo
    }

    private void UpdateMsgs()
    {
        I.box.Clear();
        foreach (string msg in messages)
        {
            Label label = new Label(msg);
            label.style.color = new Color(241/255f, 232/255f, 200/255f);
            label.style.overflow = Overflow.Visible;
            label.style.flexWrap = Wrap.Wrap;
            I.box.Add(label);
        }
        if (messages.Count == 0)
            I.box.style.display = DisplayStyle.None;
        else
            I.box.style.display = DisplayStyle.Flex;
    }

    IEnumerator autoRemoveText(int index)
    {
        yield return new WaitForSecondsRealtime(timeToFade);
        messages.RemoveAt(index);
        UpdateMsgs();
    }
}
