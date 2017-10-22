using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerNetUtils : NetworkBehaviour
{
    [Command]
    public void CmdToggleMiningDrill(GameObject obj, bool active)
    {
        obj.GetComponent<MiningDrill>().Active = active;
    }

    [Command]
    public void CmdSetPositionSync(GameObject obj, Vector3 position)
    {
        obj.GetComponent<NetPositionSync>().Position = position;
    }

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
    public void CmdReqAuth(GameObject obj)
    {
        obj.GetComponent<NetworkIdentity>().AssignClientAuthority(Player.Local.NetworkIdentity.connectionToClient);
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
    public void CmdToggleFloodlight(GameObject light, bool active)
    {
       light.GetComponent<Floodlight>().Active = active;
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
        x.GetComponent<Explosion>().Init();
    }

    [Command]
    public void CmdAddKill(string killer, string killed, string item)
    {
        RpcAddKill(killer, killed, item);
    }

    [ClientRpc]
    public void RpcAddKill(string killer, string killed, string item)
    {
        KillFeed.Instance.AddKill(killer, killed, item);
    }

    [Command]
    public void CmdSetName(string name)
    {
        if (!GetComponent<Player>().PlayerHasName(name))
            GetComponent<Player>().Name = name;
        else
            Debug.LogError("Cannot set player name to " + name + " because another player already has that name (or very similar).");
    }

    public void CmdSetTeam(string newTeam)
    {
        GetComponent<Player>().Team = newTeam;
    }

    [Command]
    public void CmdRemoteCommand(GameObject player, string command)
    {
        RpcRemoteCommand(player, Player.Local.Name, command);
    }

    [ClientRpc]
    public void RpcRemoteCommand(GameObject player, string sender, string command)
    {
        if(player.GetComponent<NetworkIdentity>().netId == Player.Local.netId)
        {
            CommandProcessing.Log("Remote command from '" + sender + "':");
            CommandProcessing.Process(command);
        }
    }
}