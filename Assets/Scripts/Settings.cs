using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RegimentSortOrder { Size = 0, Random, MeleeSkill, RangeSkill}
public static class Settings
{
    public static int SliderStep = 5;
    public static bool RunAndGunDefaultState = true;
    public static bool SpawnBlood = true;
    public static bool EnableScreenshake = true;
    public static RegimentSortOrder RegimentSortOrder;
    public static bool MasterMode = false;
    public static bool DeveloperMode = false;

    public static float HUDOpacity01 = 1f;
    public static bool ShowFPS = false;
}
