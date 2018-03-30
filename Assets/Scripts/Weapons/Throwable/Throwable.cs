using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Item))]
public class Throwable : RotatingItem
{
    [HideInInspector]
    public bool DoneEquipping;
    [HideInInspector]
    public bool PreparingToThrow;
    [HideInInspector]
    public bool PreparedToThrow;
    [HideInInspector]
    public bool HasThrown;

    [Header("References")]
    public GameObject Prefab;
    public Transform SpawnPostion;
    public Animator Anim;

    [Header("Throwing")]
    public AnimationCurve AimCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float AimTime = 0.2f;

    private Item Item;

    public void Start()
    {
        Item = GetComponent<Item>();        
    }

    public void Update()
    {
        transform.localRotation = Quaternion.identity;

        if (!hasAuthority)
            return;
        if (!Item.IsEquipped())
        {
            // Item is on the gorund...

            // Don't render hands.
            Hand.RenderHands(transform, false);

            // Play the dropped animation.
            Anim.SetBool("Dropped", true);
            return;
        }
        else
        {
            // Make sure that the animator is not in the dropped mode.
            Anim.SetBool("Dropped", false);

            // Assume that hands are rendering, the y should be...
        }

        // Wait until it is equipped.
        if (!DoneEquipping)
        {
            return;
        }

        // If not preparing to throw, press Shoot to prepare the throwable.
        if (!PreparingToThrow)
        {
            if (ActionEvent())
            {
                PreparingToThrow = true;
                Anim.SetTrigger("Prepare");
            }
        }

        if (PreparedToThrow)
        {
            // The throwable can now be thrown, but we wait until the action button has been released.
            if (!ActionEvent())
            {
                // Cool, throw now!
                Anim.SetTrigger("Throw");
            }
        }
    }

    private bool ActionEvent()
    {
        // +1 for most generic name ever.
        return InputManager.InputPressed("Shoot");
    }

    public void Anim_CanNowThrow()
    {
        PreparedToThrow = true;
    }

    public void Anim_Throw()
    {
        HasThrown = true;
        if (hasAuthority)
        {
            SpawnInstance();
        }
    }

    public void Anim_DoneEquipping()
    {
        DoneEquipping = true;
    }

    public void Anim_Done()
    {
        if (!hasAuthority)
            return;
        // Remove from holding, completely destroy...
        GetComponentInParent<PlayerHolding>().CmdDrop(false, true, Player.Local.gameObject, null); // Completely destroy this object...

        // Look for new throwable of same type in inventory, and if we find one...
        ItemStack i = PlayerInventory.inv.Inventory.GetOfType(Item.Prefab);
        if (i != null)
        {
            // Equip it!
            Item.Option_Equip(i, i.Prefab);
        }
    }

    [Client]
    private void SpawnInstance()
    {
        if (SpawnPostion == null)
            return;

        // Make new object.
        CmdSpawnPrefab(Player.Local.gameObject, SpawnPostion.position, SpawnPostion.rotation, InputManager.GetMousePos());
    }

    [Command]
    public void CmdSpawnPrefab(GameObject player, Vector2 position, Quaternion rotation, Vector2 targetPos)
    {
        GameObject x = Instantiate(this.Prefab, position, rotation);
        x.GetComponent<ThrowableInstance>().TargetPosition = targetPos;
        x.GetComponent<ThrowableInstance>().Team = player.GetComponent<Player>().Team;
        x.transform.position = position;
        NetworkServer.Spawn(x);
    }

    public override bool AllowRotateNow()
    {
        return !HasThrown && (DoneEquipping && (PreparingToThrow || PreparedToThrow));
    }

    public override bool ForceRotateNow()
    {
        return PreparingToThrow;
    }

    public override float GetAimTime()
    {
        return AimTime;
    }

    public override float GetCurvedTime(float rawTime)
    {
        return AimCurve.Evaluate(rawTime);
    }
}