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

    [SerializeField]
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

    PawnFunctions.Pawn sourcePawn;

    private void Start() =>
        Destroy(gameObject, projectileMaxLifetime);

    // take end parameter to auto destroy there?
    public void CreateAndMove(Transform target, float inaccuracy, Projectile projectileType, float thisWeaponDamage, PawnFunctions.Pawn sourcePawn, Weapon thisWeapon)
    {
        thisProjectile = projectileType;
        weaponDamage = thisWeaponDamage;
        thisType = projectileType.damageType;
        this.sourcePawn = sourcePawn;
        this.thisWeapon = thisWeapon;

        sprite.sprite = PawnRenderer.getProjectile(projectileType.Name);

        Vector3 targetNew = target.position;
        targetNew.x += Random.Range(-inaccuracy, inaccuracy);       // this is genius
        targetNew.y += Random.Range(-inaccuracy, inaccuracy);
        targetNew.z = 1;

        Vector2 diff = target.position - transform.position;
        diff.Normalize();

        // TODO: NO ATAN2!!!!!!!!!!!!!
        rb.rotation = Mathf.Atan2(diff.y,diff.x) * Mathf.Rad2Deg;
        rb.AddForce((targetNew-transform.position) * projectileForce, ForceMode2D.Impulse);
    }
                                                            // we need this reference for the image
    public void DoThrow(Transform target, float inaccuracy, Weapon weapon, float damage)
    {
        weaponDamage = damage;
        thisType = weapon.attacks[(int)(Random.value * weapon.attacks.Count)].damageType;
        thisWeapon = weapon;

        Vector3 targetNew = target.position;
        targetNew.x += Random.Range(-inaccuracy, inaccuracy);       // this is genius
        targetNew.y += Random.Range(-inaccuracy, inaccuracy);
        targetNew.z = 1;

        Vector2 diff = target.position - transform.position;
        diff.Normalize();

        // TODO: NO ATAN2!!!!
        rb.rotation = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        rb.AddForce(((targetNew - transform.position)) * projectileForce, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision) // todo: put in healthsystem? so no component call
    {
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
            h.TakeRangeDamage(thisProjectile.damage * weaponDamage, thisWeapon, sourcePawn,
                thisType,
                new Attack("Hit",thisProjectile.damageType,false,thisProjectile.damage*weaponDamage));
            Destroy(gameObject);
        }
    }
}