using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Projectiles;
using Weapons;
using Attacks;

public class ProjectileBehaviour : MonoBehaviour
{
    [SerializeField]
    float projectileMaxLifetime;

    public float projectileForce;

    Projectile thisProjectile;
    float weaponDamage;

    DamageType thisType;
    Weapon thisWeapon;

    [SerializeField]
    Rigidbody2D rb;
    [SerializeField]
    SpriteRenderer sprite;
    [SerializeField]
    Collider2D ourCollider;

    bool isFire;
    float damage;
    DamageType damageType;

    PawnFunctions.Pawn sourcePawn;
    bool isThrow;

    protected void Start() =>
        Destroy(gameObject, projectileMaxLifetime);

    protected void OnDestroy()
    {
        if(isFire)
        {
            var go = Instantiate(WCMngr.I.firePrefab);
            Vector2Int p = Vector2Int.FloorToInt(transform.position);
            go.transform.position = new Vector3(p.x, p.y, -2);
        }
    }

    // take end parameter to auto destroy there?
    public void CreateAndMove(Transform target, float inaccuracy, Projectile projectileType, float thisWeaponDamage, PawnFunctions.Pawn sourcePawn, Weapon thisWeapon, float range, bool isFire)
    {
        Destroy(gameObject, range * 1.25f);

        thisProjectile = projectileType;
        weaponDamage = thisWeaponDamage;
        thisType = projectileType.damageType;
        this.sourcePawn = sourcePawn;
        this.thisWeapon = thisWeapon;
        this.isFire = isFire;
        this.damage = projectileType.damage;

        sprite.sprite = PawnRenderer.getProjectile(projectileType.ID);

        Vector3 targetNew = target.position;
        targetNew.x += Random.Range(-inaccuracy, inaccuracy);       // this is genius
        targetNew.y += Random.Range(-inaccuracy, inaccuracy);
        targetNew.z = 1;

        Vector2 diff = target.position - transform.position;
        diff.Normalize();

        //rb.rotation = Mathf.Atan2(diff.y,diff.x) * Mathf.Rad2Deg;
        transform.right = ((Vector2)transform.position + diff) - (Vector2)transform.position;
        rb.AddForce((targetNew-transform.position) * projectileForce, ForceMode2D.Impulse);
    }
                                                            // we need this reference for the image
    public void DoThrow(Transform target, float inaccuracy, Weapon weapon, float damage, float range)
    {
        if (weapon.attacks.Count <= 0)
            return;                   // why are we even here>
        Destroy(gameObject, range * 1.25f);

        isThrow = true;
        weaponDamage = damage;
        thisType = weapon.attacks[(int)(Random.value * weapon.attacks.Count)].damageType;
        thisWeapon = weapon;
        this.damage = damage;

        Vector3 targetNew = target.position;
        targetNew.x += Random.Range(-inaccuracy, inaccuracy);       // this is genius
        targetNew.y += Random.Range(-inaccuracy, inaccuracy);
        targetNew.z = 1;

        Vector2 diff = target.position - transform.position;
        diff.Normalize();

        //rb.rotation = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.right = ((Vector2)transform.position + diff) - (Vector2)transform.position;
        rb.AddForce(((targetNew - transform.position)) * projectileForce, ForceMode2D.Impulse);
    }

    protected void OnTriggerEnter2D(Collider2D collision) // put in healthsystem? REPLY: can't, have to get this projectile's damage so pick your poison : getcomponent here or there. better here than there
    {
        if(isFire)
        {
            var go = Instantiate(WCMngr.I.firePrefab);
            Vector2Int p = Vector2Int.FloorToInt(transform.position);
            go.transform.position = new Vector3(p.x, p.y, -2);

            Destroy(gameObject);
        }
        if (collision.gameObject == null)
            return;
        if (collision.gameObject.TryGetComponent(out HealthSystem h)) 
        {
            if (h.p == sourcePawn)
            {
                Physics2D.IgnoreCollision(collision, ourCollider);
                return;
            }
            //thisWeapon = thisWeapon == null ? WeaponManager.Get("Empty") : thisWeapon;
            float dmg = isThrow ? damage : damage * weaponDamage;
            h.TakeRangeDamage(dmg, thisWeapon, sourcePawn,
                thisType,
                new Attack("Hit",damageType,false, damage*weaponDamage));
            Destroy(gameObject);
        }
    }
}