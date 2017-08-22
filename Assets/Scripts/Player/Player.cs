using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{

    public PlayerAnimation Animation;
    public PlayerDirection Direction;
    public PlayerHeadLook HeadLook;
    public PlayerHolding Holding;
    public PlayerLooking Looking;
    public PlayerMovement Movement;
    public HandyRemoval HandyRemoval;
    public NetworkIdentity NetworkIdentity;
    public Creature Creature;
    public PlayerNetUtils NetUtils;
    public Player _Player;

    public static Player Local;

    public void Start()
    {
        if (isLocalPlayer)
        {
            Local = this;
            Local._Player = this;
            gameObject.name = "Local Player";
            PlayerInventory.inv.Inventory.Owner = this.transform;

            Creature.UponDeath += UponDeath;

        }
        else
        {
            gameObject.name = "Other Player";
        }
    }

    private void UponDeath()
    {
        // 'Respawn'
        Creature.CmdHeal(Creature.GetMaxHealth());
        transform.position = Vector3.zero;
        Debug.Log("Player died:");
        Debug.Log("Local player was " + Creature.GetDamageReportAsPlayer("killed"));
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