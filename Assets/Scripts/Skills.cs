using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// I'm somewhat proud of this class.
/// </summary>
public static class Skills
{
    public static string GetSkillName(int level)
    {
        switch (level)
        {
            case 0:
                return "Incapable";
            case 1:
                return "Amateur";
            case 2:
                return "In Training";
            case 3:
                return "Deployable";
            case 4:
                return "Passionate";
            case 5:
                return "Seasoned";
            case 6:
                return "Professional";
            case 7:
                return "Very Skilled";
            case 8:
                return "Mentor";
            case 9:
                return "Gifted";
            case 10:
                return "Legend";
            default:
                return "not a skill level. someone messed up.";
        }
    }

    public static float EffectToHitChance(int level)
    {
        return (level * 0.7f * level) / 100; // for percentage
    } // MELEE
    public static float EffectToDodgeChance(int level) // MELEE
    {
        return (level * 0.55f * level) / 100; // for percentages
    }
    /// <summary>
    /// Obsolete
    /// </summary>
    public static float EffectToDamage(int level) // MELEE
    {
        return (level * 0.25f)/100;
    }

    public static float EffectToAccuracy(int level)
    {
        return level * 0.07f;
    } // RANGED
    public static float EffectToAimTime(int level)
    {
        return (level * -0.8f * level) / 100;
    } // RANGED
}
