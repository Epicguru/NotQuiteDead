using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Health), typeof(Item), typeof(PlaceablePreview))]
public class Placeable : NetworkBehaviour
{
    [SyncVar]
    public bool IsPlaced;

    [Tooltip("Is this placeable static (not moving) when placed?")]
    public bool Static = true;

    [HideInInspector] public Item Item;
    [HideInInspector] public Health Health;
    [HideInInspector] public Rigidbody2D Rigidbody;
    [HideInInspector] public PlaceablePreview Preview;

    public Vector3 PlacedScale = Vector3.one, ItemScale = new Vector3(0.1f, 0.1f, 1);

    public void Start()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        Item = GetComponent<Item>();
        Health = GetComponent<Health>();
        Preview = GetComponent<PlaceablePreview>();

        Item.Lighting.Exceptions = Preview.Preview.transform.GetComponentsInChildren<SpriteRenderer>(true);

        if(isServer)
            Health.UponDeathServer += UponDeathServer;
    }

    private void UponDeathServer()
    {
        // Die!
        Destroy(this.gameObject);
        //Debug.Log("Placeable destroyed!");
    }

    private void SetCollidersAsTriggers(bool trigger)
    {
        foreach(Collider2D c in GetComponentsInChildren<Collider2D>())
        {
            c.isTrigger = trigger;
        }

        if (!trigger)
            SetItemPickupAsTrigger();
    }

    private void SetItemPickupAsTrigger()
    {
        Item.pickup.collider.isTrigger = true;
    }

    [Command]
    public void CmdPlace(Vector2 position, Quaternion rotation)
    {
        Item newItem = Item.NewInstance(Item.Prefab);

        //Debug.Log("Placing at : " + position);

        newItem.transform.position = position;
        newItem.transform.rotation = rotation;

        newItem.GetComponent<Placeable>().IsPlaced = true;

        NetworkServer.Spawn(newItem.gameObject);
    }

    public void UpdatePlacing()
    {
        if (IsPlaced == true)
            return;
        if (!Item.IsEquipped())
            return;

        if (InputManager.InputDown("Shoot"))
        {
            // Place!
            float angle = Preview.GetRotationAngles();
            CmdPlace(Preview.GetPosition(), Preview.GetRotation());

            // Remove from hands
            GetComponentInParent<PlayerHolding>().CmdDrop(false, true, Player.Local.gameObject);

            // TODO : Look for new one in inventory and equip.
            InventoryItem i = PlayerInventory.inv.Inventory.GetOfType(Item.Prefab);
            if(i != null)
            {
                Item.Option_Equip(i);
                PlaceablePreview.LastRotation = angle;
            }
        }
    }

    public void Update()
    {
        if (isServer)
        {
            // TOOD

            // Do not allow pickup when placed!
            if(Item != null && Item.pickup != null)
                Item.pickup.AllowPickup = !IsPlaced;

            Health.CanHit = IsPlaced;
            
        }
        if(isClient && hasAuthority) // More checks in method
        {
            UpdatePlacing();
        }

        SetCollidersAsTriggers(!IsPlaced);
        transform.localScale = IsPlaced ? PlacedScale : ItemScale;

        Rigidbody.bodyType = (IsPlaced && Static) ? RigidbodyType2D.Static : RigidbodyType2D.Dynamic;
    }
}
