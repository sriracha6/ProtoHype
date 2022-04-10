using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blood : MonoBehaviour
{
    bool shouldDie = false;
    private void OnBecameInvisible()
    {
        shouldDie = true;
    }
    private void OnBecameVisible()
    {
        shouldDie=false;
    }

    private void Update()
    {
        if (shouldDie)
        {
            Destroy(gameObject,5f);
        }
    }
}
