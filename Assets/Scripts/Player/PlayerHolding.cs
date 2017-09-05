using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

// A class that handles the equiping of item on the player.

// An item object, which exists only on the ground or equiped, can be equiped by the player and is then
// held in the players hands.

[NetworkSettings(sendInterval = 0.05f)]
public class PlayerHolding : NetworkBehaviour
{
    public Transform Holding; // The game obejct that holds items that are currenty equiped.
    public Item Item; // The currently equipped item.

    private HandyRemoval handyRemoval;
    private PlayerLooking looking;
    private float timer, timer2, timer3;
    private float lerp;
    private float SendRate = 10f;

    [SyncVar(hook = "ReceiveAngle")]
    private float rotation;
    private float oldRotation;

    public void Start()
    {
        handyRemoval = GetComponent<HandyRemoval>();
        looking = GetComponent<PlayerLooking>();
    }

    [Command]
    public void CmdEquip(string prefab, GameObject localPlayer, ItemData data)
    {
        // We need to equip this item.
        // This runs on the server, which we can think of as in the middle of nowhere.
        // We need to create the item here, but do net mess with settings because that will
        // not transmit to clients.

        // Checks
        if (string.IsNullOrEmpty(prefab))
        {
            Debug.LogError("Prefab string was null or empty.");
            return;
        }

        Item created = Item.NewInstance(prefab);
        created.Data = data; // This should sync.
        if(data == null)
        {
            data = new ItemData();
            data.Flag = 1;
        }
        // Data is applied upon 'Start'

        // Assuming that this item has not been spawned...
        NetworkServer.SpawnWithClientAuthority(created.gameObject, localPlayer);

        // Setting the item state to equipped sets the parent automatically.

        // Set states
        created.SetEquipped(true, localPlayer);

        RpcSetItem(created.gameObject, localPlayer);

        //Debug.Log("Created item!");
    }

    [ClientRpc]
    public void RpcSetItem(GameObject item, GameObject player)
    {
        if(this.Item != null)
        {
            // Put in inventory.
            CmdDrop(false, false, Player.Local.gameObject, this.Item.Data);
            Debug.Log("Already holding item, put away...");
        }
        this.Item = item.GetComponent<Item>();
        Debug.Log("Equipped item '" + this.Item.Name + "'");

        // Check if is gun and is local player
        if(player.GetComponent<NetworkIdentity>().netId == Player.Local.NetworkIdentity.netId)
        {
            if(item.GetComponent<Gun>() != null)
            {
                // Is gun!
                GunHUD.Instance.SetHolding(item.GetComponent<Gun>());
            }
        }
    }

    // TODO network!
    [Command]
    public void CmdDrop(bool drop, bool destroy, GameObject localPlayer, ItemData data)
    {
        // Drops the currently held item, if holding anything.

        // Drop - If true then the item is dropped to the floor, if false then it is stored in the players inventory.
        // Destroy - If true the item is not stored, not dropped and is removed from existence.

        if (destroy)
        {
            // Completely destroy.
            Destroy(this.Item.gameObject); // Does this work!?!?
            return;
        }

        if (!drop)
        {
            // Put in inventory.
            this.Item.RequestDataUpdate(); // Refresh data.
            PlayerInventory.Add(this.Item);
            Destroy(this.Item.gameObject); // Does this work!?!?
        }
        else
        {
            // Drop item
            Player.Local.NetUtils.CmdSpawnDroppedItem(Item.Prefab, Player.Local.transform.position, data);
            Destroy(this.Item.gameObject); // Does this work!?!?
        }

    }

    public void Update()
    {
        if (isLocalPlayer)
        {
            if(handyRemoval.IsShowingHands() != (Item == null))
                handyRemoval.CmdSetShowHands(Item == null);
            //looking.Looking = Item != null;
        }

        UpdateRotation();
    }

    // Rotation of hands
    private void UpdateRotation()
    {
        // Run only on local player.
        if (!isLocalPlayer)
        {
            // Set angle based on data send from authorative player.
            LerpOtherPlayerAngle();
            return;
        }

        if (Item == null)
        {
            if(rotation != 0)
                CmdSetAngle(0);
            looking.Looking = false;
            return;
        }            

        Gun g = Item.GetComponent<Gun>();
        if (g == null)
        {
            // Is not gun, quit.
            if (rotation != 0)
                CmdSetAngle(0);
            timer = 0;

            looking.Looking = false;
        }
        else
        {
            // Is gun, activate.

            // Detect aiming...
            if (InputManager.InputPressed("Aim") && !g.Animation.IsReloading && !g.Animation.IsChambering)
            {
                // Increate percentage and lerp.
                timer += Time.deltaTime;                
                looking.Looking = true;
            }
            else
            {
                timer -= Time.deltaTime;
                looking.Looking = false;
            }

            if (timer > g.Aiming.AimTime)
                timer = g.Aiming.AimTime;
            if (timer < 0)
                timer = 0;

            float p = timer / g.Aiming.AimTime;

            lerp = g.Aiming.Curve.Evaluate(p);


            // Finally send angle update call!
            SendAngle(); // Does not send every frame.

            // Only runs on the local player.
            SetRealAngle(GetFinalAngle());
        }
    }

    private float GetFinalAngle()
    {
        float targetAngle = CalculateAngle();
        float neutral = 0f;

        float interpolated = Mathf.Lerp(neutral, targetAngle, this.lerp);

        return interpolated;
    }

    private void SendAngle()
    {
        timer2 += Time.deltaTime;

        float interval = 1f / SendRate;
        if(timer2 >= interval)
        {
            timer2 -= interval;

            float angle = GetFinalAngle();

            // Send
            if(rotation != angle)
            {
                CmdSetAngle(angle);
            }
        }
    }

    private void ReceiveAngle(float newAngle)
    {
        // This is a syncvar hook

        // Set old angle
        oldRotation = rotation;

        rotation = newAngle;

        timer3 = 0;
    }

    private void LerpOtherPlayerAngle()
    {
        // To be run only on other players!

        timer3 += Time.deltaTime;

        if (timer3 > 1f / SendRate)
            timer3 = 1f / SendRate;

        float p = timer3 / (1f / SendRate);

        float lerpedAngle = Mathf.Lerp(oldRotation, rotation, p);

        // Set this angle.
        SetRealAngle(lerpedAngle);
    }
    private void SetRealAngle(float angle)
    {
        // Set the rotation of the holding object angle.
        Holding.localRotation = Quaternion.Euler(0, 0, angle);
    }

    private static Vector2 myPos = new Vector2();
    public float CalculateAngle()
    {
        Vector2 mousePos = InputManager.GetMousePos();
        myPos.Set(transform.position.x, transform.position.y);

        Vector2 dst = mousePos - myPos;

        if (!Player.Local.Direction.Right)
        {
            dst.x = -dst.x;
        }

        float angle = Mathf.Atan2(dst.y, dst.x) * Mathf.Rad2Deg;

        return angle;
    }

    [Command]
    private void CmdSetAngle(float angle)
    {
        rotation = angle;
    }
}