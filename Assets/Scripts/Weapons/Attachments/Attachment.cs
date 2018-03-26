using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Item))]
public class Attachment : NetworkBehaviour
{
    [Header("Info")]
    public AttachmentType Type = AttachmentType.SIGHT;
    public bool Hidden = false;

    [Header("Muzzle Only")]
    public Transform CustomBulletSpawn = null;

    public bool IsAttached
    {
        get
        {
            return Item.NetPosSync.IsParented;
        }
    }

    public Gun Gun
    {
        get
        {
            if (!IsAttached)
            {
                _Gun = null;
                return null;
            }
            if(_Gun == null)
            {
                _Gun = GetComponentInParent<Gun>();
            }
            return _Gun;
        }
    }
    private Gun _Gun;

    public Item Item
    {
        get
        {
            if (_Item == null)
            {
                _Item = GetComponent<Item>();
            }
            return _Item;
        }
    }
    private Item _Item;

    public Transform BulletSpawn
    {
        get
        {
            if(Type != AttachmentType.MUZZLE)
            {
                return null;
            }
            else
            {
                return CustomBulletSpawn;
            }
        }
    }

    private string layer;

    private void OnTransformParentChanged()
    {
        if(transform.parent != null)
        {
            GunAttachments ga = transform.GetComponentInParent<GunAttachments>();

            // Does not create infinite loop because transform parent change is ignored if the parent is equal. I am the best at explaining.
            Transform mount = ga.GetMountFor(this.Type);
            if (ga != null && transform.parent != mount)
            {
                transform.SetParent(mount);
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
                transform.localScale = Vector3.one;
                ga.AttachmentsUpdated();
            }
        }
    }

    public void Update()
    {
        if (IsAttached)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            if (Gun.Item.IsEquipped())
            {
                SetLayer("Equipped Items");
            }
            else
            {
                SetLayer("Dropped Items");
            }
        }

        if (Hidden)
        {
            foreach (SpriteRenderer r in GetComponentsInChildren<SpriteRenderer>())
            {
                r.enabled = !IsAttached;
            }
        }

        if (isServer && Item.Pickup != null)
            Item.Pickup.AllowPickup = !IsAttached;
    }

    private void SetLayer(string layer)
    {
        if (this.layer == layer)
            return;
        foreach (SpriteRenderer r in GetComponentsInChildren<SpriteRenderer>())
        {
            if (r.gameObject.layer != 9)
                r.sortingLayerName = layer;
        }

        this.layer = layer;
    }

    public void ApplyEffects(Gun gun)
    {
        // Apply all attachment effects onto the gun.
        if(Type == AttachmentType.MUZZLE)
            gun.Shooting.AudioSauce.VolumeMultiplier = 0.05f;
    }
}