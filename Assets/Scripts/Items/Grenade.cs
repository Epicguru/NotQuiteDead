using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Grenade : NetworkBehaviour {

    public ThrowableInstance ThrowableInstance;

    public void Start()
    {
        ThrowableInstance = GetComponent<ThrowableInstance>();
        if (isServer)
            ThrowableInstance.AuthorityHitTarget += DestroyAndExplode;
    }

    public void DestroyAndExplode()
    {
        // Destroy and explode the grenade object.
        Explosion.Spawn(transform.position);
        Destroy(this.gameObject);
    }
}
