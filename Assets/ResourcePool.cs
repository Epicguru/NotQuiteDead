using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ResourcePool : NetworkBehaviour
{
    [SyncVar]
    public string Team;

    [SyncVar]
    public bool Active;

    public void Update()
    {
        if (!isServer)
            return;

        UpdateActiveState();
    }

    [Server]
    public void UpdateActiveState()
    {
        Active = Teams.I.TeamExists(Team);
    }
}
