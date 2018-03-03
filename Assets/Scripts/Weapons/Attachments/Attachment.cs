﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Item))]
public class Attachment : NetworkBehaviour
{
    [SyncVar]
    public GameObject Gun;

    [Header("Basics")]
    public AttachmentType Type;
    public bool Hidden = false;

    [Header("Muzzle Only")]
    public Transform BulletSpawn;

    [Header("Muzzle Only - Custom Shot Sound")]
    public AudioClip CustomShotSound;
    [Header("Muzzle Only - Original Shot Effects")]
    [Range(0f, 1f)]
    public float ShotVolume = 0f;
    [Range(0f, 1f)]
    public float ShotRange = 1f;
    public float ShotPitch = 1f;
    [Header("Muzzle Only - Custom Shot Effects")]
    [Range(0f, 1f)]
    public float CustomShotVolume = 1f;
    public float CustomShotPitch = 1f, CustomShotRange = 40f;

    private bool applied = false;
    private string layer = null;
    private Item Item;
    private Gun PrefabGun;
    private Gun CacheGun;

    public bool IsAttached
    {
        get
        {
            return Gun != null;
        }
    }

    public void Start()
    {
        Item = GetComponent<Item>();
        if (Type == AttachmentType.MAGAZINE)
            Hidden = true;
    }

    public void ApplyData(ItemDataX x)
    {
        // Just apply the effects of this attachment if we have not already applied them.
        if (IsAttached)
        {
            if (!applied)
            {
                ApplyEffects();
            }
        }
    }

    public virtual void Update()
    {
        if (IsAttached)
        {
            // TODO make code on gun side.
            // Set the parent of this attachment.
            this.transform.SetParent(this.GetGun().GetComponent<GunAttachments>().GetMountFor(this.Type));

            // Reset rotation and scale
            this.transform.localRotation = Quaternion.identity;
            this.transform.localScale = Vector3.one;

            // Reset our local position
            this.transform.localPosition = Vector3.zero;

            if (!applied && transform.parent.childCount == 1)
            {
                ApplyEffects();
            }

            if(GetGun().Item.IsEquipped())
                SetLayer("Equipped Items");
            else
                SetLayer("Dropped Items");
        }
        else
        {
            // Means we are dropped. Ensure that we have no parent.
            if(this.transform.parent != null)
                this.transform.SetParent(null);
            SetLayer("Dropped Items");
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

    public void ApplyEffects()
    {
        if (applied)
            return;

        foreach (AttachmentTweak tweak in GetComponents<AttachmentTweak>())
        {
            tweak.Apply(this);
        }
        applied = true;

        if(GetGun() != null)
        {
            GetGun().OnShoot.AddListener(OnShoot);
            if(CustomShotSound != null && Type == AttachmentType.MUZZLE)
            {
                GetGun().Shooting.AudioSauce.PitchMultiplier = ShotPitch;
                GetGun().Shooting.AudioSauce.RangeMultiplier = ShotRange;
                GetGun().Shooting.AudioSauce.VolumeMultiplier = ShotVolume;
            }
        }
    }

    public void RemoveEffects()
    {
        if (!applied)
            return;

        if (GetGun() != null)
            GetGun().OnShoot.RemoveListener(OnShoot);

        if (GetGun() == null)
            return; // If gun has been destroyed!

        foreach (AttachmentTweak tweak in GetComponents<AttachmentTweak>())
        {
            tweak.Remove(this);
        }

        if (CustomShotSound != null && Type == AttachmentType.MUZZLE)
        {
            GetGun().Shooting.AudioSauce.PitchMultiplier = 1;
            GetGun().Shooting.AudioSauce.RangeMultiplier = 1;
            GetGun().Shooting.AudioSauce.VolumeMultiplier = 1;
        }

        applied = false;
    }

    public override void OnNetworkDestroy()
    {
        // TODO - If gun is being put away, we DO NOT CARE! Just do nothing and avoid errors.

        if (transform.parent == null)
            return;
        Gun gun = GetGun();
        if (gun == null)
            return;

        RemoveEffects();
        Attachment[] attachments = gun.GetComponentsInChildren<Attachment>();

        // Remove all effects from gun.
        foreach (Attachment a in attachments)
        {
            if (a == this)
                continue;

            a.RemoveEffects();
        }

        // Apply all effects again.
        foreach (Attachment a in attachments)
        {
            if (a == this)
                continue;

            a.ApplyEffects();
        }
    }

    public Gun GetGun()
    {
        if (!IsAttached)
            return null;
        if (CacheGun == null)
            CacheGun = Gun.GetComponentInParent<Gun>();

        return CacheGun;
    }

    public Gun GetPrefabGun()
    {
        if (PrefabGun == null)
            PrefabGun = Item.GetItem(GetGun().Item.Prefab).GetComponent<Gun>();

        return PrefabGun;
    }

    public virtual void OnShoot()
    {
        if(Type == AttachmentType.MUZZLE)
        {
            // Play custom sound, if necessary.
            if (CustomShotSound != null)
            {
                // Use an external audio sauce to play the sound, allowing us custom falloff and effects independent of gun sound.
                AudioManager.Instance.PlayOneShot(transform.position, CustomShotSound, CustomShotVolume, CustomShotPitch, CustomShotRange, 2f, CustomShotRange * 0.35f, 0.7f);
            }
        }
    }

    public void Effect_Accuracy(float percentage)
    {
        GetGun().Shooting.Damage.Inaccuracy.x *= percentage;
        GetGun().Shooting.Damage.Inaccuracy.y *= percentage;
    }

    public void Reset_Accuracy()
    {
        GetGun().Shooting.Damage.Inaccuracy = GetPrefabGun().Shooting.Damage.Inaccuracy;
    }

    public void Effect_Magazine(float capacity)
    {
        GetGun().Shooting.Capacity.MagazineCapacity = Mathf.CeilToInt(GetGun().Shooting.Capacity.MagazineCapacity * capacity);
    }

    public void Reset_Magazine()
    {
        GetGun().Shooting.Capacity.MagazineCapacity = GetPrefabGun().Shooting.Capacity.MagazineCapacity;
    }

    public void Effect_Range(float rangeMultiplier)
    {
        GetGun().Shooting.Damage.Range *= rangeMultiplier;
    }

    public void Reset_Range()
    {
        GetGun().Shooting.Damage.Range = GetPrefabGun().Shooting.Damage.Range;
    }

    public void Effect_Damage(float damageMultiplier)
    {
        GetGun().Shooting.Damage.Damage *= damageMultiplier;
    }

    public void Reset_Damage()
    {
        GetGun().Shooting.Damage.Damage = GetPrefabGun().Shooting.Damage.Damage;
    }

    public float Effect_DamageFalloff(float damageFalloffOffset)
    {
        float start = GetGun().Shooting.Damage.DamageFalloff;
        GetGun().Shooting.Damage.DamageFalloff = Mathf.Clamp(GetGun().Shooting.Damage.DamageFalloff + damageFalloffOffset, 0f, 1f);

        return GetGun().Shooting.Damage.DamageFalloff - start;
    }

    public void Reset_DamageFalloff()
    {
        GetGun().Shooting.Damage.DamageFalloff = GetPrefabGun().Shooting.Damage.DamageFalloff;
    }

    public void Effect_ShotVolume(Vector2 change)
    {
        Vector2 newVector = new Vector2(change.x, change.y);
        newVector.Scale(GetGun().Shooting.Audio.Volume);
        GetGun().Shooting.Audio.Volume.x = newVector.x;
        GetGun().Shooting.Audio.Volume.y = newVector.y;
    }

    public void Reset_ShotVolume()
    {
        GetGun().Shooting.Audio.Volume = GetPrefabGun().Shooting.Audio.Volume;
    }
}