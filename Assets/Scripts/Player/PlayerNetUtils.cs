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
        if (data == null)
            data = new ItemData();
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

    [Command]
    public void CmdDamageHealth(GameObject target, float health, string dealer, bool isSecondary)
    {
        target.GetComponent<Health>().ServerDamage(health, dealer, isSecondary);
    }

    [Command]
    public void CmdSpawnBulletTrail(Vector2 start, Vector2 end)
    {
        RpcSpawnBulletTrail(start, end);
    }

    [ClientRpc]
    private void RpcSpawnBulletTrail(Vector2 start, Vector2 end)
    {
        GameObject GO = Instantiate(GunEffects.Instance.BulletTrail.gameObject);
        GO.GetComponent<BulletPath>().Setup(start, end);
    }

    [Command]
    public void CmdSpawnExplosion(Vector2 position)
    {
        RpcSpawnExplosion(position);
    }

    [ClientRpc]
    private void RpcSpawnExplosion(Vector2 position)
    {
        GameObject x = ObjectPool.Instantiate(ExplosionPrefab.Prefab, PoolType.EXPLOSION);

        x.transform.position = position;
    }
}