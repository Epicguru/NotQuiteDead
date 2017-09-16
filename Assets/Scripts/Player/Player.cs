using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{
    [SyncVar]
    public string Name = "Player Name";

    public PlayerAnimation Animation;
    public PlayerDirection Direction;
    public PlayerHeadLook HeadLook;
    public PlayerHolding Holding;
    public PlayerLooking Looking;
    public PlayerMovement Movement;
    public PlayerEquipment Equipment;
    public PlayerNetUtils NetUtils;
    public HandyRemoval HandyRemoval;
    public NetworkIdentity NetworkIdentity;
    public Health Health;
    public SpriteLighting Lighting;
    public Player _Player;

    public static Player Local;
    public List<Player> AllPlayers = new List<Player>();

    public void Start()
    {
        Debug.Log("PLAYER!");
        if (isLocalPlayer)
        {
            Local = this;
            Local._Player = this;
            Camera.main.GetComponent<CameraFollow>().Target = transform;

            Health.UponDeath += UponDeath;
        }

        foreach(SpriteRenderer r in GetComponentsInChildren<SpriteRenderer>())
        {
            r.sortingLayerName = "Player";
        }
    }

    public void OnDestroy()
    {
        AllPlayers.Remove(this);
    }

    public Player GetPlayer(string name)
    {
        foreach(Player p in AllPlayers)
        {
            if (p.Name.Trim().ToLower() == name.Trim().ToLower())
                return p;
        }

        return null;
    }

    public bool PlayerHasName(string name)
    {
        foreach (Player p in AllPlayers)
        {
            if (p.Name.Trim().ToLower() == name.Trim().ToLower())
                return true;
        }
        return false;
    }

    public void Update()
    {
        gameObject.name = Name;
    }

    private void UponDeath()
    {
        // 'Respawn'
        Health.CmdHeal(Health.GetMaxHealth());
        transform.position = Vector3.zero;
        Debug.Log("Player died:");
        string report = Health.GetDamageReport("killed");
        Debug.Log("Local player was " + report);

        // Add to kill feed.
        Player.Local.NetUtils.CmdAddKill(Health.GetKiller(), this.Name, Health.IsActiveKiller() ? Health.GetKillerItem() : null);
    }

    /// <summary>
    /// Requests for authority of this game object. The object must be mamaged (spawned) by the server.
    /// </summary>
    public void RequestAuthority(GameObject obj)
    {
        if (!isLocalPlayer)
        {
            Debug.LogError("Somebody tried to request authority from a player that is not theirs.");
            return;
        }
        Debug.Log("Requesting authority for " + obj.name);
        CmdReqAuth(obj);
    }

    [Command]
    private void CmdReqAuth(GameObject obj)
    {
        obj.GetComponent<NetworkIdentity>().AssignClientAuthority(NetworkIdentity.connectionToClient);
    }
}