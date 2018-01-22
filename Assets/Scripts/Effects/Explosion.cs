using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Explosion : MonoBehaviour {

    public ParticleSystem Particle;

    public void Init()
    {
        GetComponent<Animator>().SetTrigger("Explode");
        Particle.time = 0;
        Particle.Play(true);

        if (NetworkServer.active)
        {
            // TODO FIXME!
            ExplosionDamage(transform.position, null, 10, "Explosion", 20, 300, AnimationCurve.Linear(0, 1, 1, 0));
        }

        Player.Local.ExplosionAt(transform.position);
    }

	public void ExplosionEnded()
    {
        ObjectPool.Destroy(gameObject, PoolType.EXPLOSION);
        Particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    public static void Spawn(Vector2 position)
    {
        Player.Local.NetUtils.CmdSpawnExplosion(position);
    }

    private static List<Health> HitObjects = new List<Health>();
    public void ExplosionDamage(Vector2 point, string team, float radius, string killer, float minDamage, float maxDamage, AnimationCurve Curve)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(point, radius);

        foreach(Collider2D c in colliders)
        {
            bool canDamage = Health.CanDamageObject(c, team);
            if (!canDamage)
                continue;
            Health h = c.GetComponentInParent<Health>();
            if (HitObjects.Contains(h))
                continue;

            // Raycast from the point of the explosion to the target collider, to ensure that it can be hit.
            RaycastHit2D[] hits = Physics2D.LinecastAll(point, c.transform.position);
            bool pathToTarget = true;
            foreach(var hit in hits)
            {
                // Just check if any are not triggers.
                if(hit.collider.isTrigger == false)
                {
                    if(hit.collider != c)
                    {
                        pathToTarget = false;
                        break;
                    }
                }
            }
            if (!pathToTarget)
                continue;

            float distanceFromCenter = Vector2.Distance(point, c.transform.position);

            float normalized = Mathf.Clamp(distanceFromCenter / radius, 0f, 1f);

            float damage = Mathf.LerpUnclamped(minDamage, maxDamage, Curve.Evaluate(normalized));

            h.ServerDamage(damage, killer, false);

            //h.BroadcastMessage("ExplosionDamage", damage, SendMessageOptions.DontRequireReceiver);

            HitObjects.Add(h);
        }

        HitObjects.Clear();
    }
}
