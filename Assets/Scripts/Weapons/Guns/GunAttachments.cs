using System;
using System.Collections.Generic;
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

    public Gun Gun
    {
        get
        {
            if(_Gun == null)
            {
                _Gun = GetComponent<Gun>();
            }
            return _Gun;
        }
    }
    private Gun _Gun;

    public Gun Prefab
    {
        get
        {
            if(_Prefab == null)
            {
                _Prefab = Item.GetItem(Gun.Item.Prefab).GetComponent<Gun>();
            }
            return _Prefab;
        }
    }
    private Gun _Prefab;

    public override void OnStartClient()
    {
        foreach (AttachmentType val in Enum.GetValues(typeof(AttachmentType)))
        {
            Transform mount = GetMountFor(val);

            if (mount != null)
            {
                NetParent parent = mount.gameObject.AddComponent<NetParent>();
                parent.ID = val.ToString();
            }
        }
    }

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

    public bool SetAttachment(GameObject player, AttachmentType type, Attachment attachment, ItemData itemData)
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
        ItemData id = ItemData.TryDeserialize(data);
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
    private void Server_SetAttachment(GameObject player, AttachmentType type, Attachment a, ItemData data)
    {
        Player p = player.GetComponent<Player>();
        if(a != null)
        {
            Attachment current = GetCurrentAttachment(type);
            if (current != null)
            {
                current.Item.RequestDataUpdate();
                string pre = current.Item.Prefab;
                data = current.Item.Data;

                // Return the current attachment...
                p.TryGiveItem(pre, 1, data);

                // Destroy the object on the server and all clients.
                current.transform.parent = null; // Don't ask why, but don't remove either...
                Destroy(current.gameObject);
            }

            // Note that attachment, 'a', is a prefab.
            string prefab = a.Item.Prefab;

            // Spawn the attachment...
            Item spawnedItem = Item.NewInstance(prefab, transform.position, data);
            NetPosSync posSync = spawnedItem.NetPosSync;
            NetParent parent;
            Transform mount = GetMountFor(type);
            if(mount != null)
            {
                parent = mount.GetComponent<NetParent>();
            }
            else
            {
                Debug.LogError("Invalid mount (null) for attachment {0}, not good at all. Expect wierd bullshittery.".Form(prefab));
                Destroy(spawnedItem.gameObject);
                return;
            }

            // Spawn on server and on all client, with player authority assigned to the player who is using the attachment.
            NetworkServer.SpawnWithClientAuthority(spawnedItem.gameObject, player);

            // Mount it to the correct NetParent.
            posSync.SetParent(parent);            
        }
        else
        {
            // Attachment is null, remove any existing attachment from that slot...
            Attachment current = GetCurrentAttachment(type);
            if(current != null)
            {
                current.Item.RequestDataUpdate();
                string prefab = current.Item.Prefab;
                data = current.Item.Data;

                // Return the current attachment...
                p.TryGiveItem(prefab, 1, data);

                // Destroy the object on the server and all clients.
                Destroy(current.gameObject);
            }
        }
    }

    public void UpdateData(ItemData data)
    {
        Attachment a;
        Dictionary<AttachmentType, string> dic = new Dictionary<AttachmentType, string>();
        dic.Add(AttachmentType.MAGAZINE, "Magazine Attachment");
        dic.Add(AttachmentType.MUZZLE, "Muzzle Attachment");
        dic.Add(AttachmentType.SIGHT, "Sight Attachment");
        dic.Add(AttachmentType.UNDER_BARREL, "Under Barrel Attachment");

        foreach (var pair in dic)
        {
            a = GetCurrentAttachment(pair.Key);
            if (a != null)
            {
                data.Update(pair.Value, a.Item.Prefab);
            }
            else
            {
                if (data.ContainsKey(pair.Value))
                    data.Remove(pair.Value);
            }
        }
    }

    public void ApplyData(ItemData data)
    {
        if (Player.Local == null)
            return;

        // TODO:
        // Because of this, and the UpdateData method, attachment data is useless and non-funtional.
        string[] keys = new string[]
        {
            "Sight Attachment",
            "Magazine Attachment",
            "Under Barrel Attachment",
            "Muzzle Attachment"
        };
        GameObject player = Player.Local.gameObject;

        foreach (var key in keys)
        {
            if (data.ContainsKey(key))
            {
                Item item = Item.GetItem(data.Get<string>(key));
                Attachment a = item.GetComponent<Attachment>();
                SetAttachment(player, a.Type, a, null);
            }
        }
    }

    public void ResetEffects()
    {
        // This resets all changed values to their default state.
        // Damage
        Gun.Shooting.Damage.Damage = Prefab.Shooting.Damage.Damage;
        Gun.Shooting.Damage.DamageFalloff = Prefab.Shooting.Damage.DamageFalloff;
        Gun.Shooting.Damage.Inaccuracy = Prefab.Shooting.Damage.Inaccuracy;
        Gun.Shooting.Damage.Penetration = Prefab.Shooting.Damage.Penetration;
        Gun.Shooting.Damage.PenetrationFalloff = Prefab.Shooting.Damage.PenetrationFalloff;
        Gun.Shooting.Damage.Range = Prefab.Shooting.Damage.Range;
        Gun.Shooting.Damage.ShotsToInaccuracy = Prefab.Shooting.Damage.ShotsToInaccuracy;
        Gun.Shooting.Damage.MinIdleTimeToCooldown = Prefab.Shooting.Damage.MinIdleTimeToCooldown;

        // Capacity
        Gun.Shooting.Capacity.BulletsPerShot = Prefab.Shooting.Capacity.BulletsPerShot;
        Gun.Shooting.Capacity.MagazineCapacity = Prefab.Shooting.Capacity.MagazineCapacity;
        Gun.Shooting.Capacity.BurstShots = Prefab.Shooting.Capacity.BurstShots;
        Gun.Shooting.Capacity.BulletsConsumed = Prefab.Shooting.Capacity.BulletsConsumed;

        // Audio
        Gun.Shooting.AudioSauce.RangeMultiplier = Prefab.Shooting.AudioSauce.RangeMultiplier;
        Gun.Shooting.AudioSauce.VolumeMultiplier = Prefab.Shooting.AudioSauce.VolumeMultiplier;
        Gun.Shooting.AudioSauce.PitchMultiplier = Prefab.Shooting.AudioSauce.PitchMultiplier;

        // Animation
        Gun.Shooting.ReloadSpeedMultiplier = Prefab.Shooting.ReloadSpeedMultiplier;
        Gun.Shooting.ShootSpeedMultiplier = Prefab.Shooting.ShootSpeedMultiplier;
    }

    public Attachment[] GetAllCurrentAttachments()
    {
        return this.GetComponentsInChildren<Attachment>();
    }

    public void ApplyEffects()
    {
        var attachments = GetAllCurrentAttachments();
        foreach (var att in attachments)
        {
            if (att == null)
                continue;
            if (!att.Item.NetPosSync.IsParented)
                continue;

            // Apply the effects of the tweak.
            // There is normally only one tweak component per attachment but the system supports more that one, hence the loop.
            att.ApplyEffects(this.Gun);
        }
    }

    public void AttachmentsUpdated()
    {
        ResetEffects();
        ApplyEffects();
    }
}