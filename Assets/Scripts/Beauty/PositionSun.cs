using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TL;DR: I'm not making my own lighting system, so i'm using the equation floor(width+height/200)^2 to create many suns.
///     For rooves, just create a layer that intercepts the shadows. dear god i'd never get hired
///     Performance concerns? Me too!
///     
/// 
/// NEVER FUCING MIND YOU CAN USE A DIRECTIONAL LIGHT FUCK !!
/// 
/// CHEESE IN MY POCKETS VELVEETA ..
/// IM A GORILLA IN A FUCKING COOP!! 
/// 
/// </summary>
public class PositionSun : MonoBehaviour
{
    public static PositionSun I = null;

    public bool doDayCycle = false;
    [Header("Settings")]
    public float dayLength;
    public float step;
    [Space]
    public Color dayColor;
    public Color sunSetColor;
    public Color nightColor;
    [Space][Space]
    [Header("Lights")]
    public Light globalLight; // we still need this despite having like 120 suns because when they go behind the tmaps, it's *way* too dark
    public Light sunLight;

    bool inBattle;

    [Range(0f, 1f)]
    public float currentProgressPercent;
    int progress; // sun rising or setting? 1 if rising, -1 if setting

    protected void Awake()
    {
        if (I == null)
            I = this;
        else
        {
            I.globalLight = globalLight;
            I.sunLight = sunLight;
            inBattle = true;
        }
    }

    protected void Update()
    {
        if(doDayCycle && inBattle)
            globalLight.color = Color.Lerp(dayColor, nightColor, currentProgressPercent);
    }

    protected void FixedUpdate()
    {
        if (!doDayCycle || !inBattle)
            return;

        sunLight.transform.Rotate(step / dayLength, 0, 0);
        currentProgressPercent += progress*(step / dayLength)/100;

        if (currentProgressPercent > 1) // move backwards. prevents snapping back to OG color
            progress = -1;
        else
            progress = 1;

        sunLight.transform.position += transform.forward * Time.fixedDeltaTime; // crisis averted: now sun doesn't move when game is paused
    }
}