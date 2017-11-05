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
    public void SetItem(Item item, ItemData data)
    {
        // Item is a PREFAB!

        if(item == null)
        {
            SetItem(null);            
            return;
        }

        // Make new instance of item...
        Item instance = Item.NewInstance(item.Prefab, transform.position);
        if(data == null)
        {
            data = new ItemData();
        }
        instance.Data = data;
        NetworkServer.Spawn(instance.gameObject);

        SetItem(instance);
    }

    [Server]
    private void SetItem(Item item)
    {
        // Item is an INSTANCE!

        if (item == GetGearItem())
            return;

        if(item == null)
        {
            DisposeOfOldItem();
        }
        else
        {
            ApplyNewItem(item);
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
    private void DisposeOfOldItem()
    {
        if (IGO == null)
            return;

        // Give item back...
        RpcReturnItem(GetGearItem().Item.Prefab, GetGearItem().Item.Data);

        // Destroy the old equipped item.
        Destroy(IGO);

        IGO = null;
    }

    [Server]
    private void ApplyNewItem(Item item)
    {
        DisposeOfOldItem(); // Does not run if old was null.

        // Set itemGO to this.
        this.IGO = item.gameObject;
    }

    [ClientRpc]
    public void RpcReturnItem(string itemPrefab, ItemData data)
    {
        if (data == null)
            data = new ItemData(); // Should not happen.

        PlayerInventory.Add(itemPrefab, data, 1);
    }

    public void Update()
    {
        if (IGO != null)
            SetupItem();
    }
}