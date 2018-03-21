using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Gun))]
public class GunAttachments : NetworkBehaviour
{
    // The component attached to the guns that handles attachments on that gun at all times.

    public Transform MuzzleMount;
    public Transform SightMount;
    public Transform UnderBarrelMount;
    public bool AllowMag = true;

    private Transform MagazineMount
    {
        get
        {
            if(_MagazineMount == null)
            {
                _MagazineMount = new GameObject("Hidden Attachment Mount").transform;
                _MagazineMount.SetParent(this.transform);
            }
            return _MagazineMount;
        }
    }
    private Transform _MagazineMount;

    public virtual bool IsValid(AttachmentType type, Attachment attachment)
    {
        switch (type)
        {
            case AttachmentType.MUZZLE:
                return MuzzleMount != null;
            case AttachmentType.SIGHT:
                return SightMount != null;
            case AttachmentType.MAGAZINE:
                return AllowMag;
            case AttachmentType.UNDER_BARREL:
                return UnderBarrelMount != null;
            default:
                return false;
        }
    }

    public virtual Transform GetMountFor(AttachmentType type)
    {
        switch (type)
        {
            case AttachmentType.MUZZLE:
                return MuzzleMount;
            case AttachmentType.SIGHT:
                return SightMount;
            case AttachmentType.MAGAZINE:
                return MagazineMount;
            case AttachmentType.UNDER_BARREL:
                return UnderBarrelMount;
            default:
                Debug.LogError("Unsported attachment type, cannot get mount: " + type);
                return null;
        }
    }

    public bool SetAttachment(GameObject player, AttachmentType type, Attachment attachment, ItemDataX itemData)
    {
        if(!IsValid(type, attachment))
        {
            Debug.LogError(string.Format("Invalid attachment setup (Type: {0}, Attachment: {1})", type, attachment == null ? "null" : attachment.GetComponent<Item>().Prefab));
            return false;
        }
        if(player == null)
        {
            Debug.LogError("Null played passed to set attachment!");
            return false;
        }

        // Check client-server status...
        if (isServer)
        {
            Server_SetAttachment(player, type, attachment, itemData);
        }
        else
        {
            CmdSetAttachment(player, type, attachment == null ? null : attachment.Item.Prefab, itemData == null ? null : itemData.Serialize());
        }

        return true;
    }

    public Attachment GetCurrentAttachment(AttachmentType type)
    {
        Transform mount = GetMountFor(type);
        Attachment att = mount == null ? null : mount.GetComponentInChildren<Attachment>();

        return att;
    }

    [Command]
    private void CmdSetAttachment(GameObject player, AttachmentType type, string prefab, string data)
    {
        ItemDataX id = ItemDataX.TryDeserialize(data);
        Attachment a;
        if (!string.IsNullOrWhiteSpace(prefab))
        {
            Item i = Item.GetItem(prefab);
            a = i.GetComponent<Attachment>();
        }
        else
        {
            a = null;
        }
        Server_SetAttachment(player, type, a, id);
    }

    [Server]
    private void Server_SetAttachment(GameObject player, AttachmentType type, Attachment a, ItemDataX itemData)
    {
        if(a != null)
        {
            Attachment current = GetCurrentAttachment(type);
            if (current != null)
            {
                current.Item.RequestDataUpdate();
                itemData = current.Item.Data;
                string itemDataString = itemData.Serialize();

                // Return the current attachment...
                RpcReturnAttachment(player, current.Item.Prefab, itemDataString);

                // Destroy the object on the server and all clients.
                Destroy(current.gameObject);
            }

            // Note that attachment, 'a', is a prefab.
            string prefab = a.Item.Prefab;

            // Spawn the attachment...
            Item spawnedItem = Item.NewInstance(prefab, transform.position, itemData);
            Attachment spawnedAttachment = spawnedItem.GetComponent<Attachment>();
            NetworkParenting parenting = spawnedItem.GetComponent<NetworkParenting>();

            // Spawn on server and on all client, with player authority assigned to the player who is using the attachment.
            NetworkServer.SpawnWithClientAuthority(spawnedItem.gameObject, player);

            // Mount it to this object. It needs to be on 'this' object because the mount code requires a NetworkIdentity to act upon.
            // The real mount is assigned from within the Attachment.cs script.
            parenting.SetParent(this.transform);
            
        }
        else
        {
            // Attachment is null, remove any existing attachment from that slot...
            Attachment current = GetCurrentAttachment(type);
            if(current != null)
            {
                current.Item.RequestDataUpdate();
                itemData = current.Item.Data;
                string itemDataString = itemData.Serialize();

                // Return the current attachment...
                RpcReturnAttachment(player, current.Item.Prefab, itemDataString);

                // Destroy the object on the server and all clients.
                Destroy(current.gameObject);
            }
        }
    }

    [ClientRpc]
    private void RpcReturnAttachment(GameObject player, string prefab, string data)
    {
        if(player == Player.Local)
        {
            ItemDataX itemData = null;
            if (!string.IsNullOrWhiteSpace(data))
            {
                itemData = ItemDataX.TryDeserialize(data);
            }
            PlayerInventory.Add(prefab, itemData, 1);
        }
    }
}