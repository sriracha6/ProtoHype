using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blood : MonoBehaviour
{
    bool shouldDie = false;
    float age = 0;

    [SerializeField] SpriteRenderer sr;

    protected void Start() =>
	    sr.sprite = CachedItems.bloodSplatters[Random.Range(0, CachedItems.bloodSplatters.Count)];

    private void OnBecameInvisible()
    {
        shouldDie = true;
        //Destroy(gameObject);
    }
    private void OnBecameVisible()
    {
        shouldDie=false;
    }

    protected void FixedUpdate() =>
        age += Time.fixedDeltaTime;
}
