using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TimeController : MonoBehaviour
{
    public Button halfSpeed;
    public Button fullSpeed;
    public Button doubleSpeed;
    public Button tripleSpeed;
    public Button pause;

    void Start()
    {
        //
        var root = GetComponent<UIDocument>().rootVisualElement;

        halfSpeed = root.Q<Button>("half");
        fullSpeed = root.Q<Button>("full");
        doubleSpeed = root.Q<Button>("2");
        tripleSpeed = root.Q<Button>("3");
        pause = root.Q<Button>("pause");
         
        pause.clicked += delegate { ChangeTimeScale(0); };
        fullSpeed.clicked += delegate { ChangeTimeScale(1); };
        doubleSpeed.clicked += delegate { ChangeTimeScale(2); };
        tripleSpeed.clicked += delegate { ChangeTimeScale(3); };
        halfSpeed.clicked += delegate { ChangeTimeScale(0.5f); };
    }

    public void ChangeTimeScale(float speed)
    {
        Time.timeScale = speed;
        if (speed == 0)
        {
            AudioListener.pause = true;
        }
    }
}
