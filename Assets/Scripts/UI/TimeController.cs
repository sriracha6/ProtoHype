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

    float lastSpeed;

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

    protected void Update()
    {
        if (Input.GetKeyDown(Keybinds.x1Speed))
            ChangeTimeScale(1);
        if (Input.GetKeyDown(Keybinds.x2Speed))
            ChangeTimeScale(2);
        if (Input.GetKeyDown(Keybinds.x3Speed))
            ChangeTimeScale(3);
        if (Input.GetKeyDown(Keybinds.halfSpeed))
            ChangeTimeScale(0.5f);

        if (Input.GetKeyDown(Keybinds.pauseUnpause))
        {
            if (Time.timeScale == 0)
                ChangeTimeScale(lastSpeed);
            else
                ChangeTimeScale(0);
        }
    }

    public void ChangeTimeScale(float speed)
    {
        lastSpeed = speed;
        Time.timeScale = speed;
        AudioListener.pause = speed == 0;
    }
}
