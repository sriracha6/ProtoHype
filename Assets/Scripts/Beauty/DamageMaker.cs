using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageMaker : MonoBehaviour
{
    [SerializeField] SpriteRenderer sr;
    [SerializeField] Sprite[] damageTextures;

    protected void Start()
    {
        sr.sprite = damageTextures.randomElement();
        sr.flipX = Random.Range(0, 101) >= 50;
        sr.flipY = Random.Range(0, 101) >= 50;
        Destroy(this);
    }
}
