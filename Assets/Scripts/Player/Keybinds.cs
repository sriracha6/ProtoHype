using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Keybinds
{
    public static int LeftMouse = 0;
    public static int RightMouse = 1;
    public static int MiddleMouse = 2;
    [SettingDivider]
    [Keybind("Add to Selection")] public static KeyCode SelectAdd = KeyCode.LeftShift;
    [Keybind("Keybind")] public static KeyCode Escape = KeyCode.Escape;
    [Keybind("Subtract from Selection")] public static KeyCode SubtractSelection = KeyCode.LeftControl;
    [SettingDivider]
    [Keybind("1x speed")] public static KeyCode x1Speed = KeyCode.Alpha1;
    [Keybind("2x speed")] public static KeyCode x2Speed = KeyCode.Alpha2;
    [Keybind("3x speed")] public static KeyCode x3Speed = KeyCode.Alpha3;
    [Keybind("Half speed")] public static KeyCode halfSpeed = KeyCode.Alpha0;
    [Keybind("Pause/Unpause")] public static KeyCode pauseUnpause = KeyCode.Space;
    [SettingDivider]
    [Keybind("Clear Tile Selection")] public static KeyCode clearTileSelection = KeyCode.K;
    [Keybind("Clear Pawn Selection")] public static KeyCode clearPawnSelection = KeyCode.L;
    [SettingDivider]
    [Keybind("Move Control: Cancel")] public static KeyCode mc_cancel = KeyCode.G;
    [Keybind("Move Control: Move")] public static KeyCode mc_move = KeyCode.Z;
    [Keybind("Move Control: Search & Destroy")] public static KeyCode mc_searchdestroy = KeyCode.X;
    [Keybind("Move Control: Stand Ground")] public static KeyCode mc_standground = KeyCode.C;
    [Keybind("Move Control: Switch Secondary")] public static KeyCode mc_secondary = KeyCode.V;
    [Keybind("Move Control: Run and Gun")] public static KeyCode mc_runandgun = KeyCode.B;
    [Keybind("Move Control: Follow Cursor")] public static KeyCode mc_followcursor = KeyCode.N;
    [SettingDivider]
    [Keybind("Increment Regiment Select Percent")] public static KeyCode rp_inc = KeyCode.RightArrow;
    [Keybind("Decrement Regiment Select Percent")] public static KeyCode rp_dec = KeyCode.LeftArrow;
    [SettingDivider]
    [Keybind("Hide UI")] public static KeyCode hideUI = KeyCode.F1;
    [Keybind("Faster Camera")] public static KeyCode FasterCam = KeyCode.LeftShift;
    [SettingDivider]
    [Keybind("Show Rooves")] public static KeyCode showRooves = KeyCode.Comma;
    [Keybind("Change Camera Speed w/ Scroll")] public static KeyCode changeSpeed = KeyCode.LeftControl;
    [Keybind("Bulk Tile Selection")] public static KeyCode bulkTileSelect = KeyCode.LeftControl;
}
