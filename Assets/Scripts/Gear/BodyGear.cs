using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class BodyGear : NetworkBehaviour
{
    // Calls when gear is placed on or removed from the player.
    public static UnityEvent GearChangeEvent = new UnityEvent();

    public string Name;
    public Transform GearParent;

    // Currently equipped, is an instance. This only works on server when setting.
    private GameObject IGO
    {
        get
        {
            return Sync;
        }
        set
        {
            if (isServer)
            {
                Sync = value;
                if (netUtils == null)
                    netUtils = GetComponent<PlayerNetUtils>();
                netUtils.RpcSetIGO(Name, value);
            }
            else
            {
                // Just set value normally
                Sync = value;
            }
        }
    }
    private GameObject Sync;
    private PlayerNetUtils netUtils;

    // The item of the ItemGO, always in sync.
    private GearItem _Item;

    public GearItem GetGearItem()
    {
        if (IGO == null)
        {
            _Item = null;
            return null;
        }

        if(_Item == null)
        {
            _Item = IGO.GetComponent<GearItem>();
            return _Item;
        }

        return this._Item;
    }

    public void RemoteSetIGO(GameObject go)
    {
        // Avoid an infinite loop on host.
        this.Sync = go;

        OnItemChange(go);
    }

    [Server]
    public void SendUpdatedGear()
    {
        // Sends the current gear of this player to all clients.
        // For when the new player joins the game when people have gear on.

        IGO = IGO; // Lol this is really it.
    }

    [Server]
    public void SetItem(GameObject player, Item item, ItemData data, bool returnItem)
    {
        // Item is a PREFAB!

        if(item == null)
        {
            SetItem(player, null, returnItem);            
            return;
        }

        // Make new instance of item...
        if(data == null)
        {
            data = new ItemData();
        }
        Item instance = Item.NewInstance(item.Prefab, transform.position, data);
        NetworkServer.Spawn(instance.gameObject);

        SetItem(player, instance, returnItem);
    }

    [Server]
    private void SetItem(GameObject player, Item item, bool returnOldItem)
    {
        // Item is an INSTANCE!

        if (item == GetGearItem())
            return;

        if(item == null)
        {
            DisposeOfOldItem(player, returnOldItem);
        }
        else
        {
            ApplyNewItem(item, player, returnOldItem);
        }

        GearChangeEvent.Invoke();
    }

    private void OnItemChange(GameObject i)
    {
        this._Item = null;

        if (isLocalPlayer)
        {
            Item item = GetGearItem() == null ? null : GetGearItem().Item;
            GearUI.GearItems[Name].SetItem(item);
        }
    }

    private void SetupItem()
    {
        if (IGO == null)
            return;

        IGO.transform.SetParent(GearParent);
        IGO.transform.localPosition = Vector2.zero;
        IGO.transform.localRotation = Quaternion.identity;
    }

    [Server]
    private void DisposeOfOldItem(GameObject owner, bool returnToPlayer)
    {
        if (IGO == null)
            return;

        if (returnToPlayer)
        {
            // Give item back...
            RpcReturnItem(GetGearItem().Item.Prefab, GetGearItem().Item.Data, owner);
        }

        // Destroy the old equipped item.
        Destroy(IGO);

        IGO = null;
    }

    [Server]
    private void ApplyNewItem(Item item, GameObject owner, bool returnOldItem)
    {
        DisposeOfOldItem(owner, returnOldItem); // Does not run if old was null.

        // Set itemGO to this.
        this.IGO = item.gameObject;
    }

    [ClientRpc]
    public void RpcReturnItem(string itemPrefab, ItemData data, GameObject owner)
    {
        if (data == null)
            data = new ItemData(); // Should not happen.

        if (Player.Local.netId != owner.GetComponent<Player>().netId)
            return;

        PlayerInventory.Add(itemPrefab, data, 1);
    }

    public void Update()
    {
        if (IGO != null)
            SetupItem();
    }
}