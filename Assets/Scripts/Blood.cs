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

    protected void OnBecameInvisible()
    {
        shouldDie = true;
        //Destroy(gameObject);
    }
    protected void OnBecameVisible()
    {
        shouldDie=false;
    }

    protected void FixedUpdate()
    {
        age += Time.fixedDeltaTime;
        if (shouldDie && age > 5 * 3000) // 3000 = (60 / fixeddeltateim)
            Destroy(gameObject);
    }
}
