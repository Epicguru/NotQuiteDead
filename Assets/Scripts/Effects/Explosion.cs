using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {

    public void Init()
    {
        GetComponent<Animator>().SetTrigger("Explode");
    }

	public void ExplosionEnded()
    {
        ObjectPool.Destroy(gameObject, PoolType.EXPLOSION);
    }

    public static void Spawn(Vector2 position)
    {
        Player.Local.NetUtils.CmdSpawnExplosion(position);
    }
}
