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
    public Alive Alive;
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
        }
    }

    public void Update()
    {
        if (Alive.Dead)
        {
            Destroy(gameObject); // TODO Network!
        }
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