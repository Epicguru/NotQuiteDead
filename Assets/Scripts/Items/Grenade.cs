using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(ThrowableInstance))]
public class Grenade : NetworkBehaviour {

    public ThrowableInstance ThrowableInstance;
    public float fuzeTime = 5f;

    public void Start()
    {
        ThrowableInstance = GetComponent<ThrowableInstance>();
    }

    public void Update()
    {
        if (!isServer)
            return;

        fuzeTime = Mathf.Clamp(fuzeTime - Time.deltaTime, 0f, float.MaxValue);

        if(fuzeTime == 0f)
        {
            DestroyAndExplode();
        }
    }

    [Server]
    public void DestroyAndExplode()
    {
        // Destroy and explode the grenade object on the server.

        // The explosion is spawned on all clients.
        Explosion.Spawn(transform.position);
        Destroy(this.gameObject);
    }
}
