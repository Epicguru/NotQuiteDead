using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Gun))]
public class GunAttachments : NetworkBehaviour
{
    public Transform MuzzleMount;
    public Transform SightMount;
    public Transform UnderBarrelMount;    
    public bool AllowMag = true;
    private Transform MagazineMount;

    private Gun gun;

    public bool AllowMagazine
    {
        get
        {
            return AllowMag;
        }
    }

    public bool AllowMuzzle
    {
        get
        {
            return MuzzleMount != null;
        }
    }

    public bool AllowSight
    {
        get
        {
            return SightMount != null;
        }
    }

    public bool AllowUnderBarrel
    {
        get
        {
            return UnderBarrelMount != null;
        }
    }

    public void UpdateData(ItemDataX data)
    {
        string magazine = AllowMagazine ? MagazineMount.childCount > 0 ? MagazineMount.GetChild(0).GetComponent<Item>().Prefab : null : null;
        string muzzle = AllowMuzzle ? MuzzleMount.childCount > 0 ? MuzzleMount.GetChild(0).GetComponent<Item>().Prefab : null : null;
        string sight = AllowSight ? SightMount.childCount > 0 ? SightMount.GetChild(0).GetComponent<Item>().Prefab : null : null;
        string underBarrel = AllowUnderBarrel ? UnderBarrelMount.childCount > 0 ? UnderBarrelMount.GetChild(0).GetComponent<Item>().Prefab : null : null;

        string name;

        // Apply magazine attachment.
        name = "Magazine Attachment";
        if(magazine != null)
        {
            data.Update(name, magazine);
        }
        else
        {
            if(data.ContainsKey(name))
                data.Remove(name);
        }

        // Apply muzzle attachment.
        name = "Muzzle Attachment";
        if (muzzle != null)
        {
            data.Update(name, muzzle);
        }
        else
        {
            if (data.ContainsKey(name))
                data.Remove(name);
        }

        // Apply sight attachment.
        name = "Sight Attachment";
        if (sight != null)
        {
            data.Update(name, sight);
        }
        else
        {
            if (data.ContainsKey(name))
                data.Remove(name);
        }

        // Apply under barrel attachment.
        name = "Under Barrel Attachment";
        if (underBarrel != null)
        {
            data.Update(name, underBarrel);
        }
        else
        {
            if (data.ContainsKey(name))
                data.Remove(name);
        }
    }

    public void ApplyData(ItemDataX data)
    {
        if (!isServer)
            return;

        string sightName = "Sight Attachment";
        string magazineName = "Magazine Attachment";
        string underBarrelName = "Under Barrel Attachment";
        string muzzleName = "Muzzle Attachment";

        if (data.ContainsKey(muzzleName))
        {
            Item item = Item.GetItem(data.Get<string>(muzzleName));
            Attachment a = item.GetComponent<Attachment>();
            SetAttachment(a.Type, item);
        }
        if (data.ContainsKey(sightName))
        {
            Item item = Item.GetItem(data.Get<string>(sightName));
            Attachment a = item.GetComponent<Attachment>();
            SetAttachment(a.Type, item);
        }
        if (data.ContainsKey(magazineName))
        {
            Item item = Item.GetItem(data.Get<string>(magazineName));
            Attachment a = item.GetComponent<Attachment>();
            SetAttachment(a.Type, item);
        }
        if (data.ContainsKey(underBarrelName))
        {
            Item item = Item.GetItem(data.Get<string>(underBarrelName));
            Attachment a = item.GetComponent<Attachment>();
            SetAttachment(a.Type, item);
        }
    }

    public void Start()
    {
        gun = GetComponent<Gun>();

        if (MagazineMount == null)
        {
            MagazineMount = new GameObject("Magazine Container").transform;
            MagazineMount.SetParent(transform);
        }
    }

    public Transform GetMountFor(AttachmentType type)
    {
        switch (type)
        {
            case AttachmentType.MUZZLE:
                return MuzzleMount;
            case AttachmentType.SIGHT:
                return SightMount;
            case AttachmentType.MAGAZINE:
                if(MagazineMount == null)
                {
                    MagazineMount = new GameObject("Magazine Container").transform;
                    MagazineMount.SetParent(transform);
                }
                return MagazineMount;
            case AttachmentType.UNDER_BARREL:
                return UnderBarrelMount;
            default:
                Debug.LogError("Mount not found.");
                return null;
        }
    }

    public bool IsValid(AttachmentType type, Attachment attachment)
    {
        bool correctType = attachment.Type == type;
        bool hasMount = GetMountFor(type) != null;
        if (type == AttachmentType.MAGAZINE && !AllowMagazine)
            return false;

        return correctType && hasMount;
    }

    public bool SetAttachment(AttachmentType type, Item attachment)
    {
        // Assumes:
        // - That the item is NOT an instance but IS a prefab.
        // Steps to add attachment: 
        // 1. Ensure that it is a valid attachment.
        // 2. Pass on to server to instnaciate.

        bool valid = IsValid(type, attachment.GetComponent<Attachment>());

        if (attachment == null) // If attachment is null, always allow. This 
            valid = true;

        if (!valid)
        {
            Debug.LogError("Not a valid attachment configuration, will not add attachment.");
            return false;
        }

        if (Player.Local == null)
            return false;

        // Is valid...
        CmdSetAttachment(type, Player.Local.gameObject, attachment == null ? null : attachment.Prefab);

        return true;
    }

    [Command]
    private void CmdSetAttachment(AttachmentType type, GameObject player, string prefab)
    {
        // This is CMD, therefore runs on the server version of this object.
        // At this point we assume that:
        // 1. The attachment passes to this object has been spawned on clients and server.
        // 2. This attachment can be applied on this weapon.
        // 3. This is a valid attachment for this slot.

        // Find the sopt where it is mounted, to check for existing attachments
        Transform mount = GetMountFor(type);

        // Create item from prefab, and instanciate.
        Item i = Item.GetItem(prefab);

        GameObject attachment = Instantiate(i.gameObject, transform.position, Quaternion.identity);
        NetworkServer.SpawnWithClientAuthority(attachment, player);

        if (mount == null)
        {
            Debug.LogError("Attachment '" + attachment + "' cannot be applied to gun '" + gun.Item.Name + "'! Mount is null!");
            return;
        }

        // Object OBJ should be instanciated by now.
        if(mount.childCount > 0)
        {
            // Has a child...
            if(mount.GetChild(0) == attachment.transform)
            {
                // Already attached! Do nothing!
                Debug.LogError("Attachment '" + attachment == null ? "null" : attachment.GetComponent<Item>().Name + "' is already applied!");
            }
            else
            {
                // Something else is attached! Remove it and put in inventory.
                RpcGiveAttachmentItem(player, mount.GetChild(0).GetComponent<Item>().Prefab, 1); // Give item to player...
                Destroy(mount.GetChild(0).GetComponent<Attachment>().gameObject); // Destroy the old attachment.

                if (attachment == null)
                    return; // Stop here if we do not have a new object to apply.

                // Now add attachment.
                Attachment a = attachment.GetComponent<Attachment>();

                a.Gun = gameObject; // This sets the attached state, and the attachment will automatically apply to this gun.
            }
        }
        else
        {
            // Nothing applied to the item...

            if(attachment == null)
            {
                // Do nothing, because there is no attachment applied.
                return;
            }

            // Get attachment.
            Attachment a = attachment.GetComponent<Attachment>();

            a.Gun = gameObject; // This sets the attached state, and the attachment will automatically apply to this gun.
        }
    }

    [ClientRpc]
    private void RpcGiveAttachmentItem(GameObject player, string prefab, int amount)
    {
        if(Player.Local.NetworkIdentity.netId == player.GetComponent<NetworkIdentity>().netId)
            PlayerInventory.Add(prefab, null, amount); // Add to inventory...
    }
}
