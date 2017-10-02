using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EnemyTargetPlayer : NetworkBehaviour
{
    public bool Active = true;
    public float MaxTargetDistance = 20f;
    [HideInInspector] public TargetDirectionProvider Targeting;

	public void Start()
    {
        Targeting = GetComponent<TargetDirectionProvider>();
        InvokeRepeating("UpdateTarget", 1f, 1f);
    }

    public void UpdateTarget()
    {
        if (!Active)
        {
            Targeting.Target = null;
            return;
        }

        float smallestDst = float.MaxValue;
        Player closest = null;
        foreach(Player p in Player.AllPlayers)
        {
            float dst = Vector2.Distance(transform.position, p.transform.position);
            if (dst > MaxTargetDistance)
                continue;
            if (dst < smallestDst)
            {
                smallestDst = dst;
                closest = p;
            }
        }

        Targeting.Target = closest == null ? null : closest.transform;
    }
}
