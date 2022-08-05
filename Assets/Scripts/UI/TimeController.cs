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

    public float lastSpeed;
    private Button lastButton;

    public static TimeController I;

    protected void Start()
    {
        //
        if (I == null)
            I = this;

        var root = GetComponent<UIDocument>().rootVisualElement;

        I.halfSpeed = root.Q<Button>("half");
        I.fullSpeed = root.Q<Button>("full");
        I.doubleSpeed = root.Q<Button>("2");
        I.tripleSpeed = root.Q<Button>("3");
        I.pause = root.Q<Button>("pause");
         
        I.pause.clicked += delegate { I.ChangeTimeScale(0, I.pause); };
        I.fullSpeed.clicked += delegate { I.ChangeTimeScale(1, I.fullSpeed); };
        I.doubleSpeed.clicked += delegate { I.ChangeTimeScale(2, I.doubleSpeed); };
        I.tripleSpeed.clicked += delegate { I.ChangeTimeScale(3, I.tripleSpeed); };
        I.halfSpeed.clicked += delegate { I.ChangeTimeScale(0.5f, I.halfSpeed); };
    }

    protected void Update()
    {
        if (Input.GetKeyDown(Keybinds.x1Speed))
            I.ChangeTimeScale(1, I.fullSpeed);
        if (Input.GetKeyDown(Keybinds.x2Speed))
            I.ChangeTimeScale(2, I.doubleSpeed);
        if (Input.GetKeyDown(Keybinds.x3Speed))
            I.ChangeTimeScale(3, I.tripleSpeed);
        if (Input.GetKeyDown(Keybinds.halfSpeed))
            I.ChangeTimeScale(0.5f, I.halfSpeed);

        if (Input.GetKeyDown(Keybinds.pauseUnpause))
        {
            if (Time.timeScale == 0)                            // UNPAUSe
                I.ChangeTimeScale(I.lastSpeed, I.lastButton);
            else                                               // PAUSE
                I.ChangeTimeScale(0, I.pause);
        }
    }

    private void UpdateColor(Button src)
    {
        Color defaul = new Color(57/255f,56/255f,54/255f);
        
        if(src!=null)
            src.style.backgroundColor = new Color(241/255f, 232/255f, 200/255f);

        if (I.pause != src)        I.pause.style.backgroundColor = defaul;
        if (I.fullSpeed != src)    I.fullSpeed.style.backgroundColor = defaul;
        if (I.halfSpeed != src)    I.halfSpeed.style.backgroundColor = defaul;
        if (I.doubleSpeed != src)  I.doubleSpeed.style.backgroundColor = defaul;
        if (I.tripleSpeed != src)  I.tripleSpeed.style.backgroundColor = defaul;
    }

    public void ChangeTimeScale(float speed, Button src)
    {
        I.lastSpeed = speed;
        I.lastButton = src;
        Time.timeScale = speed;
        AudioListener.pause = speed == 0;

        UpdateColor(src);
    }
}
