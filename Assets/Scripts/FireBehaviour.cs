using Baracuda.Monitoring;
using PawnFunctions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static WeatherManager;

/// <summary>
/// TODO: object pooling for instantiate
/// 
/// its slow to put a buncha triggers on all the fire and kinda spaghetti so just check if the pawn is in the tile range of the fire.
/// </summary>
public class FireBehaviour : MonitoredBehaviour
{
    [SerializeField] 
    [Range(0,100)]
    private int spreadRate;
    [SerializeField] private int tickRate;

    private int __size;
    [Monitor]
    private int Size 
    { 
      get { return __size; } 
      set { __size = Mathf.Clamp(value, 1, 10); } 
    }
    [SerializeField] SpriteRenderer spr;

    void Start()
    {
        Size = 1;
        StartCoroutine(UpdateFire());
    }

    IEnumerator UpdateFire()
    {
        // TODO DESTRYOIGN STUFF AND WHEN STUFF IS BURNT START DECREASING IN SIEZ, onfire for buildings,
        // NO FIRE OTUSIDE MAP!
        switch(WeatherManager.currentWeather)
        {
            case WeatherType.Clear:
                Size++;
                break;
            case WeatherType.Rain:
                Size -= 2;
                break;
            case WeatherType.Snow:
                Size--;
                break;
        }

        var currentBuilding = TilemapPlace.buildings[(int)transform.position.x, (int)transform.position.y];

        if (Size <= 0                                                                                    // no fire
           || currentBuilding == null                                                                    // we destroyed it
           || currentBuilding.flammability <= 0)                                                         // weird edge case
        {
            FireManager.firePositions.Remove(transform.position);
            PathfindExtra.SetFree((int)transform.position.x, (int)transform.position.y);
            Destroy(gameObject);
        }
        currentBuilding.hitpoints -= (Size / 10) * (currentBuilding.flammability/100 * currentBuilding.hitpoints);

        if (currentBuilding.hitpoints <= 0)
            TilemapPlace.DestroyBuilding(transform.position);

        if (Size >= 5 && Random.Range(0, 100) >= 100-spreadRate)
        {
            bool[] neighbors = new bool[4];
            neighbors[0] = FireManager.firePositions.Contains(transform.position + Vector3.left);
            neighbors[1] = FireManager.firePositions.Contains(transform.position + Vector3.right);
            neighbors[2] = FireManager.firePositions.Contains(transform.position + Vector3.up);
            neighbors[3] = FireManager.firePositions.Contains(transform.position + Vector3.down);

            if (neighbors.Count(x => x == true) < 4)
            {
                GameObject go = Instantiate(Loader.loader.firePrefab);
                go.transform.position = TryPosition(100, neighbors);

                var g = go.transform.position;
                g.z = -2;

                FireManager.firePositions.Add(go.transform.position);
                PathfindExtra.SetUsed((int)transform.position.x, (int)transform.position.y);
            }
        }
        spr.size = new Vector2(Size, Size);

        yield return new WaitForSeconds(tickRate);
        StartCoroutine(UpdateFire());
    }

    Vector2 TryPosition(int maxrecursion, bool[] neighbors)
    {
        if(maxrecursion <= 0)
            return Vector2.negativeInfinity;
        Vector2 lolz;
        int r = Random.Range(0, 4);
        if(!neighbors[r])
        {
            if(r == 0)
                lolz = transform.position + Vector3.left;
            else if (r == 1)
                lolz = transform.position + Vector3.right;
            else if (r == 2)
                lolz = transform.position + Vector3.up;
            else
                lolz = transform.position + Vector3.down;

            return lolz;
        }
        else
            return TryPosition(maxrecursion--, neighbors);
    }
}
