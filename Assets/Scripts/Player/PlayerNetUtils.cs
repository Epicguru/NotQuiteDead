using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerNetUtils : NetworkBehaviour
{
    [Command]
    public void CmdSpawnDroppedItem(string prefab, Vector3 position)
    {
        Item newItem = Item.NewInstance(prefab);
        newItem.transform.position = position; // Set position.

        NetworkServer.Spawn(newItem.gameObject);
    }

    [Command]
    public void CmdDestroyItem(GameObject obj)
    {
        Destroy(obj);
    }

    [Command]
    public void CmdOpenGate(GameObject gate, bool open)
    {
        gate.GetComponent<Gate>().IsOpen = open;
    }
}