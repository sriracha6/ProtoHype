using Projectiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PawnFunctions;
using Shields;
using System;

using Random = UnityEngine.Random;
using Attacks;

public static class CombatFunctions
{
    public static Projectile getRandomProjectile(List<Projectile> source)
    {
        return source[Random.Range(0, source.Count)];
    }

    public static Transform GetClosestEnemy(List<Transform> enemies, Transform source)
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = source.position;
        foreach (Transform potentialTarget in enemies.AsReadOnly())
        {
            Vector3 directionToTarget = potentialTarget.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }

        return bestTarget;
    }

    public static float calculateInaccuracy(bool shouldRunAndGun, Pawn p, float runAndGunInaccuracy)
    {
        if (shouldRunAndGun)
            return Skills.EffectToAccuracy(p.rangeSkill) * runAndGunInaccuracy;
        else
            return Skills.EffectToAccuracy(p.rangeSkill);
    }

    public static bool shieldBlocked(Shield s, Pawn target)
    {
        if (target == null || s == null || target.shield == null)
            return false;
        if (target.hasShield)
        {
            if (Random.Range(0, 2) >= s.baseBlockChance + Skills.EffectToDodgeChance(target.meleeSkill))
                return true;
            else
                return false;
        }
        else
            return false;
    }

    public static float randomVariation(float orig)
    {
        return (float)Math.Round(Random.Range(0.75f, 1.25f) * orig, MidpointRounding.AwayFromZero);
    }

    public static bool MeleeDodged(float ourHitChance, float enemyDodgeChance, float ourMoveSpeed, float enemyMoveSpeed)
    {
        //Debug.Log($"US:{(ourHitChance + 0.1f * ourMoveSpeed)} them: {(enemyDodgeChance + 0.1f * enemyMoveSpeed)}");
        if ((ourHitChance + 0.1f * ourMoveSpeed) < (enemyDodgeChance + 0.1f * enemyMoveSpeed))
            return false;  // im sure that, since i wrote this code, this is completely foolproof and won't result in any exploits
        else
        {
            if (randomVariation(1) <= 0.9) // prevents inf stalemates of equal enemies and makes cool rnadom yes
                return false;
            else
                return true;// they dodged :(
        }
    }

    public static Attack GetAttack(List<Attack> attks)
    {
        float rng = Random.Range(0, 100);
        Attack currentAttack;
        if (rng <= 3) // rare attack. Returns random rare attack.
        {
            List<Attack> rares = attks.FindAll(x => x.isRare == true);
            if (rares.Count > 0)
            {
                currentAttack = rares[Random.Range(0, rares.Count)];
                return currentAttack;
            }
            else
            {
                List<Attack> nonrares = attks.FindAll(x => x.isRare == false);
                currentAttack = nonrares[Random.Range(0, nonrares.Count)];
                return currentAttack;
            }
        }
        else // i just know this is going to cause some kind of index error sometmie
        {
            List<Attack> nonrares = attks.FindAll(x => x.isRare == false);
            Debug.Log($"{nonrares.Count}");
            currentAttack = nonrares[Random.Range(0, nonrares.Count)];
            return currentAttack;
        }
    }
}
