using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerNetUtils : NetworkBehaviour
{
    [Command]
    public void CmdSpawnDroppedItem(string prefab, Vector3 position, ItemData data)
    {
        Item newItem = Item.NewInstance(prefab);
        newItem.transform.position = position; // Set position.
        newItem.Data = data;

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

    [Command]
    public void CmdToggleTorch(GameObject torch, bool active)
    {
        torch.GetComponent<Torch>().Active = active;
    }
}