using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {

    public ParticleSystem Particle;

    public void Init()
    {
        GetComponent<Animator>().SetTrigger("Explode");
        Particle.time = 0;
        Particle.Play(true);
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
}
