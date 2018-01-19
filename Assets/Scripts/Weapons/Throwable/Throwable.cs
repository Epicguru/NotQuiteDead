using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Item))]
public class Throwable : NetworkBehaviour
{
    //[HideInInspector]
    public bool DoneEquipping;
    //[HideInInspector]
    public bool PreparingToThrow;
    //[HideInInspector]
    public bool PreparedToThrow;

    public GameObject Prefab;
    public Transform SpawnPostion;
    public Animator Anim;

    private Item Item;

    public void Start()
    {
        Item = GetComponent<Item>();
    }

    public void Update()
    {
        // TODO MAKE ME DECENT!
        if (!hasAuthority)
            return;
        if (!Item.IsEquipped())
            return;

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
        if(hasAuthority)
            SpawnInstance();
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
        GetComponentInParent<PlayerHolding>().CmdDrop(false, true, Player.Local.gameObject, new ItemData()); // Completely destroy this object...
    }

    [Client]
    private void SpawnInstance()
    {
        if (SpawnPostion == null)
            return;

        // Make new object.
        CmdSpawnPrefab(Player.Local.gameObject, SpawnPostion.position, SpawnPostion.rotation, InputManager.GetMousePos());

        // Look for new throwable of same type in inventory, and if we find one...
        InventoryItem i = PlayerInventory.inv.Inventory.GetOfType(Item.Prefab);
        if (i != null)
        {
            // Equip it!
            Item.Option_Equip(i, i.ItemPrefab);
        }
    }

    [Command]
    public void CmdSpawnPrefab(GameObject player, Vector2 position, Quaternion rotation, Vector2 targetPos)
    {
        GameObject x = Instantiate(this.Prefab, position, rotation);
        x.GetComponent<ThrowableInstance>().Target = targetPos;
        x.GetComponent<ThrowableInstance>().StartPosition = position;
        x.transform.position = position;
        NetworkServer.Spawn(x);
    }
}