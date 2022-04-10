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

/// <summary>
/// This class is uglier than I thought possible. However, it is easy to understand if you just look at it. For like an hour.
/// </summary>
public class CombatSystem : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private float runAndGunInaccuracy = 1.5f;

    [SerializeField]
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
    Attack damage;

    RaycastHit2D hit;
    LayerMask layerMask;

    float attackTimer = 0.35f;
    float extraRangeTime;

    bool shouldRunAndGun;

    // ------------------------------------------------------------ //

    #region ------- Melee ---------
    public Attack GetAttack(List<Attack> attks)
    {
        float rng = Random.Range(0, 100);
        if(rng<=2) // rare attack. Returns random rare attack.
        {
            List<Attack> rares = attks.FindAll(x => x.isRare==true);
            if (rares.Count > 0)
            {
                damage = rares[Random.Range(0, rares.Count)];
                return damage;
            }
            else
            {
                List<Attack> nonrares = attks.FindAll(x => x.isRare == false);
                damage = nonrares[Random.Range(0, nonrares.Count)];
                return damage;
            }
        }
        else // i just know this is going to cause some kind of index error sometmie
        {
            List<Attack> nonrares = attks.FindAll(x => x.isRare==false);
            damage = nonrares[Random.Range(0, nonrares.Count)];
            return damage;
        }
    }

    public float randomVariation(float orig)
    {
        return (float)Math.Round(Random.Range(0.75f, 1.25f) * orig, MidpointRounding.AwayFromZero);
    }

    public bool MeleeDodged(float ourHitChance, float enemyDodgeChance, float ourMoveSpeed, float enemyMoveSpeed)
    {
        //Debug.Log($"US:{(ourHitChance + 0.1f * ourMoveSpeed)} them: {(enemyDodgeChance + 0.1f * enemyMoveSpeed)}");
        if ((ourHitChance+0.1f * ourMoveSpeed) < (enemyDodgeChance+0.1f * enemyMoveSpeed))
        {
            return false;  // im sure that, since i wrote this code, this is completely foolproof and won't result in any exploits
        }
        else
        {
            if (randomVariation(1) <= 0.9) // prevents inf stalemates of equal enemies and makes cool rnadom yes
            {
                return false;
            }
            else
            {
                return true;// they dodged :(
            }
        }
    }

    public bool shieldBlocked(Shield s, Pawn target)
    {
        if (target == null || s == null || target.shield == null)
            return false;
        if (target.hasShield)
        {
            if (Random.Range(0, 2) >= s.baseBlockChance + Skills.EffectToDodgeChance(target.meleeSkill))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public void doMeleeAttack(Pawn target)
    {
        if (!target.dead && !MeleeDodged(Skills.EffectToHitChance(p.meleeSkill), 
            Skills.EffectToDodgeChance(target.meleeSkill), 
            p.healthSystem.GetVital(VitalSystem.Moving), 
            target.healthSystem.GetVital(VitalSystem.Moving))
            && !shieldBlocked(p.shield, target))
        {
            weaponSprite.gameObject.transform.rotation = Quaternion.identity; // reset possible bow rotation
            animator.Play("MeleeHit");
            if (p.activeWeapon.enableRangedMeleeDamage)
            {
                target.healthSystem.SubtractHealth(randomVariation(p.activeWeapon.rangedMeleeDamage),p.activeWeapon);
            }
            else
            {
                damage = GetAttack(p.activeWeapon.attacks);
                target.healthSystem.SubtractHealth(randomVariation(
                    damage.Damage),                                                   // + (damage.Damage * Skills.EffectToDamage(p.meleeSkill))
                    p.activeWeapon, damage);
                // The sacred equation. (Health -= (Current attack damage + Skill damage effect) + Armor Pen / Armor Protection) * random.
            }
        }
    }
    #endregion

    public float calculateInaccuracy()
    {
        if (shouldRunAndGun)
        {
            return Skills.EffectToAccuracy(p.rangeSkill)*runAndGunInaccuracy;
        }
        else
        {
            return Skills.EffectToAccuracy(p.rangeSkill);
        }
    }

    void doRangeAttack(Pawn target, int type) // type 0: shooter, type 1:thrower
    {
        animator.Play("Recoil");
        if (type == 0) // SHOOTER
        {
            weaponSprite.gameObject.transform.Rotate(transform.position - target.transform.position);
            // ^ we need this to rotate the weapon
            GameObject arrow = Instantiate(projectile, projectileCollection.transform);
            //arrow.GetComponent<SpriteRenderer>().sprite = ARROWTEXTURE;
            arrow.transform.position = firePoint.transform.position; // this is fairly bad
            arrow.GetComponent<ProjectileBehaviour>()
                .DoMovement(target.gameObject.transform, calculateInaccuracy(),
                // todo: no
                getRandomProjectile()
                ,p.activeWeapon.rangedDamage);
        }
        else // THROW
        {
            weaponSprite.gameObject.transform.Rotate(transform.position - target.transform.position);
            // ^ we need this to rotate the weapon
            GameObject arrow = Instantiate(projectile, projectileCollection.transform);
            //arrow.GetComponent<SpriteRenderer>().sprite = ARROWTEXTURE; // TODO: can this be put in the projectileBehaviour code?
            arrow.transform.position = firePoint.transform.position; // this is fairly bad
            arrow.GetComponent<ProjectileBehaviour>()
                .DoThrow(target.gameObject.transform, calculateInaccuracy(), p.activeWeapon,
                 p.activeWeapon.rangedDamage);
        }
    }

    public void onChangeWeapon()
    {
        meleeRange = p.activeWeapon.meleeRange;
        rangeRange = p.activeWeapon.range; 
        weaponSprite.gameObject.transform.rotation = Quaternion.identity;

        extraRangeTime = Skills.EffectToAimTime(p.rangeSkill);
    }

    void Start()
    {
        projectileCollection = GameManager2D.Instance.projectileParent; // no. todo: this is no

        onChangeWeapon(); // this is VERY bad

        int walls = 1 << LayerMask.NameToLayer("Walls");
        int defaul = 1 << LayerMask.NameToLayer("Default");
        layerMask = walls | defaul;

        InvokeRepeating(nameof(Checks), 0, 0.5f);
    }

    void Checks()
    {
        int totalEnemies = 0;
        p.enemyCountries.ForEach(delegate (Country c)  // todo: must we check this every time?
        {
            totalEnemies += c.members.Count;
        });

        if (!(totalEnemies > 0))
            return;

        if (!canAttack)
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
            closestEnemy = p.enemyCountries.Count > 1
                ? GetClosestEnemy(p.enemyCountries[Random.Range(0, p.enemyCountries.Count)].memberTransforms)
                : GetClosestEnemy(p.enemyCountries[Random.Range(0, p.enemyCountries.Count)].memberTransforms);
        //}
        //else
        if(Player.ourSelectedPawns.Count > 0)
        {
            List<Transform> list = new List<Transform>();
            foreach(Pawn p in PawnManager.GetAll())
            {
                if (!p.country.Equals(Player.playerCountry))
                {
                    list.Add(p.gameObject.transform);
                }
            }

            closestEnemy = GetClosestEnemy(list);
        }

        Pawn closestPawn = closestEnemy.GetComponent<Pawn>(); // the melee distance is practically useless if it only checks distance every half second. 
        if (checkCanMeleeAttack(closestPawn))  // TODO: NO VECTOR2.DISTANCE!!!
        {
            doMeleeAttack(closestPawn); // also bad
            canAttack = false;
            attackTimer = meleeAttackCooldown;
            return;
        }

        hit = Physics2D.Linecast(firePoint.transform.position, closestEnemy.position, layerMask);

        if (checkCanRangeAttack())                          //                    v we want all ranged to use melee, but not melee to try to use range. and also waste a sqrt call
        {
            if (p.enemyCountries.Contains(hit.transform.gameObject.GetComponent<Pawn>().country))
            {
                doRangeAttack(closestEnemy.GetComponent<Pawn>(), (int)p.activeWeapon.rangeType);
                canAttack = false;
                attackTimer = meleeAttackCooldown+extraRangeTime;
            }
        }
    }

    void DamageDealt(int fromWeapon)
    {
        if(fromWeapon == 0 && p.activeWeaponSlot == ActiveWeapon.Primary)
        {
            p.activeWeapon = p.heldSidearm;
        }
    }

    private bool checkCanMeleeAttack(Pawn closestPawn)
    {
        return p.activeWeapon.Type.Equals(WeaponType.Melee) || p.activeWeapon.enableRangedMeleeDamage && !closestPawn.pawnDowned
                && Vector2.Distance(transform.position, closestEnemy.position) <= meleeRange;
    }
    private bool checkCanRangeAttack()
    {
        return hit && p.activeWeapon.Type.Equals(WeaponType.Ranged) &&
                hit.transform.CompareTag("Pawn") && !hit.transform.GetComponent<Pawn>().pawnDowned // todo: no
                && hit.distance < (healthSystem.GetVital(VitalSystem.Sight) * rangeRange);
    }

    private void Update()
    {
        if (attackTimer <= 0f)
            canAttack = true;
        if (attackTimer > 0f)
            attackTimer -= Time.deltaTime;
    }

    private void switchToSecondary()
    {
        closestEnemy = p.enemyCountries.Count > 1
    ? GetClosestEnemy(p.enemyCountries[Random.Range(0, p.enemyCountries.Count)].memberTransforms)
    : GetClosestEnemy(p.enemyCountries[0].memberTransforms); // TODO: AAAAA I FORGOT TO PUT A TODO HERE

        // TOOD: THIS CAUSES ERRORS PROBABLY. WHAT IF THEY HAVE THE SAME PRIMARY AND SECONDARY?? ALSO VECTOR DISTANCE
        if (p.activeWeaponSlot == ActiveWeapon.Secondary
            && Vector2.Distance(transform.position,
            closestEnemy.position) >= 2f)
        {
            p.activeWeapon = p.heldPrimary;
            weaponSprite.sprite = CachedItems.renderedWeapons.Find(x => x.name == p.heldSidearm.Name).sprite;
            p.activeWeaponSlot = ActiveWeapon.Primary;
        }
        onChangeWeapon();
    }

    public Projectile getRandomProjectile()
    {
        return p.inventory[Random.Range(0,p.inventory.Count)];
    }

    Transform GetClosestEnemy(List<Transform> enemies)
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (Transform potentialTarget in enemies)
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

}