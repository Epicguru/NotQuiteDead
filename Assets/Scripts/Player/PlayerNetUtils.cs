using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerNetUtils : NetworkBehaviour
{
    private Player player;

    public Player GetPlayer()
    {
        if (player == null)
            player = GetComponent<Player>();

        return player;
    }

    [Command]
    public void CmdGiveItems(string prefab, int count, string data)
    {
        GetPlayer().TryGiveItem(prefab, count, ItemData.TryDeserialize(data));
    }

    [Command]
    public void CmdRemoveItems(string prefab, int count)
    {
        int removed;
        ItemData data;
        GetPlayer().Inventory.Remove(prefab, count, out removed, out data);
    }

    [Command]
    public void CmdDropAtFeet(string prefab, int count)
    {
        int removed;
        ItemData data;
        bool worked = GetPlayer().Inventory.Remove(prefab, count, out removed, out data);

        if (worked)
        {
            for (int i = 0; i < removed; i++)
            {
                GetPlayer().SpawnItemAtFeet(prefab, data);
            }
        }
    }

    [Command]
    public void CmdRequestPickup(GameObject player, GameObject item)
    {
        Debug.Log("Incoming pickup request from " + player.name);
        if(item != null)
        {
            Item i = item.GetComponent<Item>();
            if(i != null)
            {
                i.RequestDataUpdate();
                RpcPickupAccepted(player, item, i.Data.Serialize());
                Destroy(item.gameObject);
            }
        }
    }

    [ClientRpc]
    private void RpcPickupAccepted(GameObject player, GameObject item, string data)
    {
        if (player == null || item == null)
            return;

        if(Player.Local.netId == player.GetComponent<Player>().netId)
        {
            Item i = item.GetComponent<Item>();

            if(i != null)
            {
                i.Pickup.PickupAccepted(ItemData.TryDeserialize(data));
            }
        }
    }

    [Command]
    public void CmdPlaceFurniture(string prefab, bool isNull, int x, int y)
    {
        World.Instance.Furniture.RequestingPlace(isNull ? null : prefab, x, y);
    }

    [Command]
    public void CmdRequestTileChange(string prefab, int x, int y, string layer)
    {
        BaseTile tile = (string.IsNullOrEmpty(prefab) ? null : BaseTile.GetTile(prefab));
        World.Instance.TileMap.GetLayer(layer).ClientRequestingTileChange(tile, x, y);
    }

    [Command]
    public void CmdRequestChunk(GameObject player, int x, int y, string layer)
    {
        World.Instance.TileMap.GetLayer(layer).CmdRequestChunk(x, y, player);
    }

    [Command]
    public void CmdDropGear(string slot, string data)
    {
        BodyGear g = GetPlayer().GearMap[slot];

        if(g.GetGearItem() != null)
        {
            Item old = g.GetGearItem().Item;
            g.SetItem(GetPlayer().gameObject, null, null, false);

            Item instance = Item.NewInstance(old.Prefab, GetPlayer().transform.position, ItemData.TryDeserialize(data));

            NetworkServer.Spawn(instance.gameObject);
        }
    }

    [Command]
    public void CmdRequestGear(GameObject player)
    {
        // So this is called on a client.
        // On another player's gameobject.
        // In order to get an updated version of their gear.

        Player p = player.GetComponent<Player>();

        foreach(var v in p.BodyGear)
        {
            v.SendUpdatedGear();
        }
    }

    [ClientRpc]
    public void RpcSetIGO(string name, GameObject go)
    {
        GetPlayer().GearMap[name].RemoteSetIGO(go);
    }

    [Command]
    public void CmdTryGive(string prefab, int count, string data)
    {
        ItemData d = null;
        d = ItemData.TryDeserialize(data);
        GetPlayer().TryGiveItem(prefab, count, d);
    }

    [Command]
    public void CmdSetGear(string name, string prefab, string data, bool returnOldItem)
    {
        GetPlayer().GearMap[name].SetItem(player.gameObject, prefab == null ? null : Item.GetItem(prefab), ItemData.TryDeserialize(data), returnOldItem);
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
        if (!GetPlayer().PlayerHasName(name))
            GetPlayer().Name = name;
        else
            Debug.LogError("Cannot set player name to " + name + " because another player already has that name (or very similar).");
    }

    public void CmdSetTeam(string newTeam)
    {
        GetPlayer().Team = newTeam;
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