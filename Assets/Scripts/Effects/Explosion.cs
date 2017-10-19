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
            ExplosionDamage(transform.position, 10, "Explosion", 20, 300, AnimationCurve.Linear(0, 1, 1, 0));
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
    public void ExplosionDamage(Vector2 point, float radius, string killer, float minDamage, float maxDamage, AnimationCurve Curve)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(point, radius);

        foreach(Collider2D c in colliders)
        {
            Health h = c.GetComponentInParent<Health>();
            if (h == null)
                continue;
            if (!h.CanHit)
                continue;
            if (h.CannotHit.Contains(c))
                continue;
            if (HitObjects.Contains(h))
                continue;
            // TODO team damage?

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
