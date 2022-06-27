using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
//using Pathfinding;
using System;
using Random = UnityEngine.Random;

using PawnFunctions;
using Countries;
using Attacks;
using Weapons;
using Projectiles;
using Shields;
using Body;

using CS = CombatFunctions;

/// <summary>
/// This class is uglier than I thought possible. However, it is easy to understand if you just look at it. For like an hour.
/// </summary>
public class CombatSystem : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private float runAndGunInaccuracy = 1.5f;

    public float meleeAttackCooldown = 0.35f;
    public bool canAttack = true;
    [Space]

    [Header("Components")]
    [SerializeField]
    //private Seeker seeker;
    //[SerializeField]
    private PawnPathfind pawnPathfind;
    [SerializeField]
    private Pawn p;
    [SerializeField]
    private HealthSystem healthSystem;
    [SerializeField]
    private SpriteRenderer weaponSprite;

    public Animator animator;

    [SerializeField]
    GameObject projectile;

    [SerializeField]
    private GameObject projectileCollection;
    [SerializeField]
    private GameObject firePoint;
    // --- PRIVATES ---
    Transform closestEnemy;

    float meleeRange;
    float rangeRange;
    Attack currentAttack;

    Pawn targetUntilDeath;

    RaycastHit2D hit;
    LayerMask layerMask;

    float attackTimer = 0.35f;
    float extraRangeTime;

    bool shouldRunAndGun;

    // ------------------------------------------------------------ //

    void Start()
    {
        projectileCollection = WCMngr.I.projectileParent; // this is no  RESPONSE: it's alright. I mean, it organizes

        onChangeWeapon(); // this is VERY bad

        int walls = 1 << LayerMask.NameToLayer("Walls");
        int defaul = 1 << LayerMask.NameToLayer("Default");
        layerMask = walls | defaul;

        InvokeRepeating(nameof(Checks), 0, 0.5f);
    }

    private void Update()
    {
        if (attackTimer <= 0f)
            canAttack = true;
        if (attackTimer > 0f)
            attackTimer -= Time.deltaTime;
    }

    public void doMeleeAttack(Pawn target)
    {
        if (!target.dead && !target.pawnDowned &&                           // not downed or dead
            !CS.MeleeDodged(Skills.EffectToHitChance(p.meleeSkill),         // not dodged
                Skills.EffectToDodgeChance(target.meleeSkill),              //  ...
                p.healthSystem.GetVital(VitalSystem.Moving),                //  ...
                target.healthSystem.GetVital(VitalSystem.Moving))           //  ...
            && !CS.shieldBlocked(p.shield, target))                         // didnt shield block
        {
            weaponSprite.gameObject.transform.rotation = Quaternion.identity; // reset possible bow rotation
            animator.Play("MeleeHit");
            if (p.activeWeapon.enableRangedMeleeDamage)
            {
                currentAttack = CS.GetAttack(p.activeWeapon.attacks);
                target.healthSystem.TakeMeleeDamage(CS.randomVariation(p.activeWeapon.rangedMeleeDamage), p.activeWeapon, p,
                    currentAttack);
            }
            else
            {
                if (p.animal != null)
                {
                    if (Random.Range(0, 101) >= p.animal.sourceAnimal.hitChance)
                    {
                        p.animal.TakeDamage((int)(CS.randomVariation(currentAttack.Damage) * p.healthSystem.GetVital(VitalSystem.Dexterity)));
                        return;
                    }
                }
                float currentAttackDamage = 1;
                if (p.animal == null)
                    currentAttackDamage = currentAttack.Damage;
                else
                {
                    currentAttackDamage = currentAttack.Damage;
                    if (p.pawnPathfind.isMoving)
                        currentAttackDamage *= p.animal.trueSpeed;
                    else
                        currentAttackDamage *= 0.75f;
                }
                currentAttack = CS.GetAttack(p.activeWeapon.attacks);
                target.healthSystem.TakeMeleeDamage(CS.randomVariation(
                    currentAttackDamage),                                                   // + (damage.Damage * Skills.EffectToDamage(p.meleeSkill))
                    p.activeWeapon, p, currentAttack);
                // The sacred equation. (Health -= (Current attack damage + Skill damage effect) + Armor Pen / Armor Protection) * random.
            }
        }
    }

    void doRangeShootaAttack(Pawn target)
    {
        if (p.inventory == null || p.inventory.Count == 0)
            return; // we have no fucking bullets??? what?? i still dont know why this happens??? i hate this file on god
        animator.Play("Recoil");

        weaponSprite.gameObject.transform.Rotate(transform.position - target.transform.position);
        // ^ point to target
        GameObject arrow = Instantiate(projectile, projectileCollection.transform);
        float xpos = pawnPathfind.orientation == PawnOrientation.Right ? firePoint.transform.position.x + 0.5f : firePoint.transform.position.x - 0.5f;

        var projectilef = CS.getRandomProjectile(p.inventory);

        arrow.transform.position = new Vector2(xpos, firePoint.transform.position.y); // no idea why i need to do this because the firepoint is rotated too but ok!
        arrow.GetComponent<ProjectileBehaviour>()
            .CreateAndMove(target.gameObject.transform, CS.calculateInaccuracy(shouldRunAndGun, p, runAndGunInaccuracy),
             projectilef
                , p.activeWeapon.rangedDamage, p, p.activeWeapon, healthSystem.GetVital(VitalSystem.Sight) * rangeRange,
             projectilef.hasFire);
    }
    void doRangeThrowerAttack(Pawn target)
    {
        animator.Play("Recoil");

        weaponSprite.gameObject.transform.Rotate(new Vector3(0,0,(transform.position - target.transform.position).z));
        // ^ we need this to point to the target
        GameObject arrow = Instantiate(projectile, projectileCollection.transform);
        arrow.transform.position = firePoint.transform.position; // this is fairly bad
        arrow.GetComponent<ProjectileBehaviour>()
            .DoThrow(target.gameObject.transform, CS.calculateInaccuracy(shouldRunAndGun, p, runAndGunInaccuracy), p.activeWeapon,
                p.activeWeapon.rangedDamage, healthSystem.GetVital(VitalSystem.Sight) * rangeRange);
    }

    public void onChangeWeapon()
    {
        // its the weapons that's causing issues
        try
        {
            meleeRange = p.activeWeapon.meleeRange;
            rangeRange = p.activeWeapon.range;
            weaponSprite.gameObject.transform.rotation = Quaternion.identity;

            extraRangeTime = Skills.EffectToAimTime(p.rangeSkill);
        }
        catch (Exception e)
        {
            Debug.Log($"WARNING: This weird shit is happening with combat systemonchange weapon error and animalbehavior.");
            DB.NullCount(Weapon.List);
            DB.Null(p);
            DB.Null(p.activeWeapon);
        }
    }

    void Checks()
    {
        int totalEnemies = 0;
        p.enemyCountries.ForEach(delegate (Country c)  // must we check this every time?
        {                                              // ans : no, but it's fast so who gives 2 shits? also its very useful
            totalEnemies += c.members.Count;
        });

        if (totalEnemies <= 0)
            return;

        if (!canAttack && healthSystem.lastDamageTime <= 12f * Time.fixedDeltaTime // 0.2 sec
            && healthSystem.lastAttacker != null && p.enemyCountries.Contains(healthSystem.lastAttacker.country)) // needed so we dont get ready to defend against ourself :/ 
        {
            switchToSecondary();
            return;
        }

        if (shouldRunAndGun && pawnPathfind.isMoving)
        {
            pawnPathfind.speed = p.healthSystem.GetVital(VitalSystem.Moving) *
                pawnPathfind.runAndGunEffectToSpeed * pawnPathfind.speed;
        }

        // -------- END GAURD CAUSES -------- //

        shouldRunAndGun = pawnPathfind.isRunAndGun;

        //if (!MoveControls.findQueue(p).actions[0].Type.Equals("Attack"))
        //{
        if(targetUntilDeath == null)
        {
            closestEnemy = p.enemyCountries.Count > 1
                ? CS.GetClosestEnemy(p.enemyCountries[Random.Range(0, p.enemyCountries.Count)].memberTransforms,   transform)
                : CS.GetClosestEnemy(p.enemyCountries[Random.Range(0, p.enemyCountries.Count)].memberTransforms,   transform);

            targetUntilDeath = closestEnemy.GetComponent<Pawn>();
            //Debug.Log($"new enemy: {p.country} : target is null");
        }
        //else
        if(Player.ourSelectedPawns.Count > 0)
        {
            List<Transform> list = new List<Transform>();
            foreach(Pawn p in PawnManager.GetAll())
            {
                if (!p.country.Equals(Player.playerCountry))
                    list.Add(p.gameObject.transform);
            }

            closestEnemy = CS.GetClosestEnemy(list, transform);
        }
        
        if (checkCanMeleeAttack(targetUntilDeath) && targetUntilDeath != null)
        {
            doMeleeAttack(targetUntilDeath); // also bad
            canAttack = false;
            attackTimer = meleeAttackCooldown;
            return;
        }

        hit = Physics2D.Linecast(transform.position, closestEnemy.position, layerMask);

        if (checkCanRangeAttack())                          //                    v we want all ranged to use melee, but not melee to try to use range. and also waste a sqrt call
        {
            var xdir = (hit.transform.position - transform.position).x;

            Country t = null;

            if (xdir < 0 || xdir >= 0)
                t = hit.transform.gameObject.GetComponent<Pawn>().country;

            if (xdir < 0 && p.enemyCountries.Contains(t)
                /*&& pawnPathfind.orientation != PawnOrientation.Left*/)
            {
                transform.localScale = new Vector3(-1, 1);
                pawnPathfind.orientation = PawnOrientation.Left;
            }
            else if(xdir >= 0 && p.enemyCountries.Contains(t)
                /*&& pawnPathfind.orientation != PawnOrientation.Right*/)
            {
                transform.localScale = new Vector3(1, 1);
                pawnPathfind.orientation = PawnOrientation.Right;
            }
            if(p.activeWeapon.rangeType == RangeType.Shooter)
                doRangeShootaAttack(closestEnemy.GetComponent<Pawn>());
            else if(p.activeWeapon.rangeType == RangeType.Thrown)
                doRangeThrowerAttack(closestEnemy.GetComponent<Pawn>());

            canAttack = false;
            attackTimer = meleeAttackCooldown + extraRangeTime;

            return;
        }
    }

    void DamageDealt(int fromWeapon)
    {
        if(fromWeapon == 0 && p.activeWeaponSlot == ActiveWeapon.Primary)
            p.activeWeapon = p.heldSidearm;
    }

    private bool checkCanMeleeAttack(Pawn closestPawn)
    {
        if (closestPawn.pawnDowned || closestPawn.dead)
        {
            targetUntilDeath = null;
            return false;
        }
        if (p == null || p.dead == true)
            return false;
        Vector2 offset = closestEnemy.position - transform.position;
        float sqrLen = offset.sqrMagnitude;
        return p.activeWeapon.Type.Equals(WeaponType.Melee) || p.activeWeapon.enableRangedMeleeDamage && !closestPawn.pawnDowned
                && sqrLen <= meleeRange * meleeRange;
    }
    private bool checkCanRangeAttack()
    {
        if (p == null || p.dead == true)
            return false;

        if (hit && p.activeWeapon.Type.Equals(WeaponType.Ranged) &&
                hit.transform.CompareTag("Pawn"))
        {
            if(!hit.transform.GetComponent<Pawn>().pawnDowned 
                && hit.distance < (healthSystem.GetVital(VitalSystem.Sight) * rangeRange))
                return true;
            else
                targetUntilDeath = null;
        }
        return false;
    }

    private void switchToSecondary()
    {
        closestEnemy = p.enemyCountries.Count > 1
            ? CS.GetClosestEnemy(p.enemyCountries[Random.Range(0, p.enemyCountries.Count)].memberTransforms,   transform)
            : CS.GetClosestEnemy(p.enemyCountries[0].memberTransforms,   transform); 
        targetUntilDeath = closestEnemy.GetComponent<Pawn>();

        float distance = (closestEnemy.position - transform.position).sqrMagnitude;

        if (p.activeWeaponSlot == ActiveWeapon.Secondary
            && distance >= (2f*2f))
        {
            p.activeWeapon = p.heldPrimary;
            weaponSprite.sprite = CachedItems.renderedWeapons.Find(x => x.id == p.heldSidearm.ID).sprite;
            p.activeWeaponSlot = ActiveWeapon.Primary;
        }
        onChangeWeapon();
    }
}