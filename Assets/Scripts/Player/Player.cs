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
    public Health Health;
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

            Camera.main.GetComponent<CameraFollow>().Target = transform;

            Health.UponDeath += UponDeath;
        }
        else
        {
            gameObject.name = "Other Player";
        }

        foreach(SpriteRenderer r in GetComponentsInChildren<SpriteRenderer>())
        {
            r.sortingLayerName = "Player";
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < 5; i++)
            {
                ItemData data = new ItemData();
                ItemData.Add(data); // Register!
            }
        }
    }

    private void UponDeath()
    {
        // 'Respawn'
        Health.CmdHeal(Health.GetMaxHealth());
        transform.position = Vector3.zero;
        Debug.Log("Player died:");
        Debug.Log("Local player was " + Health.GetDamageReport("killed"));
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