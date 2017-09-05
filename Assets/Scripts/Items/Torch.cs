using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Torch : NetworkBehaviour {

    [SyncVar]
    public bool Active = true;

    public ParticleSystem Particles;
    public Light Light;

    private ItemPickup pickup;
    private Placeable placeable;

    public void Start()
    {
        pickup = GetComponent<ItemPickup>();
        placeable = GetComponent<Placeable>();
    }

    public void Update()
    {
        if(placeable.IsPlaced && pickup.MouseOver && InputManager.Active)
        {
            ActionHUD.DisplayAction("Press " + InputManager.GetInput("Interact") + " to " + (Active ? "put out" : "light") + ".");
            if (InputManager.InputDown("Interact"))
            {
                Player.Local.NetUtils.CmdToggleTorch(gameObject, !Active);
            }
        }

        if (!Active || !placeable.IsPlaced)
        {
            // Stop light and effects...
            if (!Particles.isStopped)
                Particles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            Light.enabled = false;
        }
        else
        {
            Light.enabled = true;
            if (!Particles.isPlaying)
                Particles.Play(true);
        }
    }
}
