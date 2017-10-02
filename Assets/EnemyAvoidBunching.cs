using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAvoidBunching : DirectionProvider
{
    public float BunchWeight = 10f;
    public float BunchEffectRange = 5f;

    [HideInInspector]
    public Enemy Self;

    public override void Start()
    {
        base.Start();

        Self = GetComponent<Enemy>();
    }

	public override void Update ()
    {
        base.Update();

        Enemy closest = null;
        float minDst = float.MaxValue;

        foreach(Enemy e in Enemy.Enemies)
        {
            if (e == Self)
                continue;

            float dst = Vector2.Distance(e.transform.position, transform.position);

            if(dst < minDst)
            {
                closest = e;
                minDst = dst;
            }
        }

        if(closest != null)
        {
            Active = true;
            Direction = transform.position - closest.transform.position;
            Weight = Mathf.Lerp(BunchWeight, 0f, Mathf.Clamp(minDst / BunchEffectRange, 0f, 1f));
        }
        else
        {
            Active = false;
        }
	}
}
