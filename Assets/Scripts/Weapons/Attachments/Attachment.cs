using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Item), typeof(NetworkParenting))]
public class Attachment : NetworkBehaviour
{
    [Header("Info")]
    public AttachmentType Type = AttachmentType.SIGHT;
    public bool Hidden = false;

    [Header("Muzzle Only")]
    public Transform CustomBulletSpawn = null;

    public NetworkParenting NetParent
    {
        get
        {
            if(_NetParent == null)
            {
                _NetParent = GetComponent<NetworkParenting>();
            }
            return _NetParent;
        }
    }
    private NetworkParenting _NetParent;

    public bool IsAttached
    {
        get
        {
            return NetParent.IsParented;
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
            if(ga != null)
            {
                transform.SetParent(ga.GetMountFor(this.Type));
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
                transform.localScale = Vector3.one;
            }
        }
    }

    public void Update()
    {
        if (IsAttached)
        {
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
}