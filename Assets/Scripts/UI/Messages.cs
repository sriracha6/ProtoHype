using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Messages : MonoBehaviour
{
    public VisualElement root;
    public TextField messageBox;

    [SerializeField]
    float timeToFade;

    private List<string> messages = new List<string>();

    protected static Messages instance;

    private void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        messageBox = root.Q<VisualElement>("MessageSystem").Q<TextField>("Messages");
        messageBox.style.display = DisplayStyle.None;

        instance = this;
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
        instance.Add(obj.ToString()); // giggity giggity goo
    }

    private void UpdateMsgs()
    {
        messageBox.SetValueWithoutNotify("");
        foreach (string msg in messages)
        {
            messageBox.SetValueWithoutNotify(messageBox.text + msg + "\n");
        }

        if (messages.Count == 0)
            messageBox.style.display = DisplayStyle.None;
        else
            messageBox.style.display = DisplayStyle.Flex;
    }

    IEnumerator autoRemoveText(int index)
    {
        yield return new WaitForSecondsRealtime(timeToFade);
        messages.RemoveAt(index);
        UpdateMsgs();
    }
}
