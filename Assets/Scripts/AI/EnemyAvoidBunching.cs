using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAvoidBunching : DirectionProvider
{
    public float BunchWeight = 10f;
    public float BunchEffectRange = 5f;

    [HideInInspector]
    public Enemy Self;

    [HideInInspector]
    public Enemy Closest;

    private float timer = 0f;

    public override void Start()
    {
        base.Start();

        Self = GetComponent<Enemy>();
    }

	public override void Update ()
    {
        base.Update();

        timer += Time.deltaTime;
        Enemy closest = null;
        float minDst = float.MaxValue;
        bool run = false;

        if(timer >= 0.5f)
        {
            timer -= 0.5f;
            foreach (Enemy e in Enemy.Enemies)
            {
                if (e == Self)
                    continue;

                float dst = Vector2.Distance(e.transform.position, transform.position);

                if (dst < minDst)
                {
                    closest = e;
                    minDst = dst;
                }
            }

            run = true;
            Closest = closest;
        }

        if(Closest != null)
        {
            Active = true;
            Direction = transform.position - Closest.transform.position;
            Weight = Mathf.Lerp(BunchWeight, 0f, Mathf.Clamp((run ? minDst : Vector2.Distance(transform.position, Closest.transform.position)) / BunchEffectRange, 0f, 1f));
        }
        else
        {
            Active = false;
        }
	}
}
