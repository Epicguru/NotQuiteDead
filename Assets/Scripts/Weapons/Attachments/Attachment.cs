using UnityEngine;
using UnityEngine.Events;
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
    private Transform oldTransform;

    public AttachmentTweak[] Tweaks
    {
        get
        {
            if(tweaks == null || tweaks.Length == 0)
            {
                tweaks = GetComponentsInChildren<AttachmentTweak>(true);
            }
            return tweaks;
        }
    }
    [HideInInspector]
    private AttachmentTweak[] tweaks;

    [HideInInspector]
    public UnityEvent UponShoot = new UnityEvent();

    private string layer;

    public void Start()
    {
        if(Type == AttachmentType.MAGAZINE)
        {
            Hidden = true;
        }
    }

    private void OnBeforeTransformParentChanged()
    {
        // NOTE: for some reason, the parent changing it's parent (like  grandparent?) also calls this. Why?
        // This is why this method detects the parent and if it is the same as the 'new' parent (see method below) then nothing happens.
        // Basically fixing the method.
        oldTransform = transform.parent;
    }

    private void OnTransformParentChanged()
    {
        Transform old = oldTransform;
        oldTransform = null;

        //Debug.LogError("Post Parent Change! ({0}) parent is now {1} and was {2}".Form(name, transform.parent == null ? "null" : transform.parent.name, old == null ? "null" : old.name));

        if (old == transform.parent)
        {
            return;
        }

        if(transform.parent != null)
        {
            GunAttachments ga = transform.GetComponentInParent<GunAttachments>();
            if (ga == null)
                return;
            Transform mount = ga.GetMountFor(this.Type);
            if (mount == null)
                return;
            if (transform.parent != mount)
                return;

            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
            ga.AttachmentsUpdated();
            Gun.OnShoot.AddListener(OnShoot);
        }
    }

    public void OnShoot()
    {
        UponShoot.Invoke();
    }

    public void OnDestroy()
    {
        // Could be at almost any time, just perform all checks...
        if (Gun != null)
        {
            Gun.OnShoot.RemoveListener(OnShoot);
        }
    }

    public void Update()
    {
        if (IsAttached)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = transform.localScale.ToAbs();

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
        foreach (var tweak in Tweaks)
        {
            tweak.Apply(gun);
        }
    }
}