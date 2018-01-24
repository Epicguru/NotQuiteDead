using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Item))]
public class GearItem : NetworkBehaviour
{

    // Represents the item object of a type of gear that is placed on the player's body.
    // Has stats such as armour, HP boost, speed penalty, warmth ect.

    [Tooltip("The slot that this equips onto.")]
    public string Slot;

    [Tooltip("The sprite rendering layer that is active when the gear is equipped.")]
    public string Layer = "Default";

    [Tooltip("A multiplier that affects player movement speed. The value is set as in:\n* 0.3 means the player moves 30% faster,\n* -0.5 means the player moves 50% slower.")]
    public float SpeedChange;

    [Tooltip("A multiplier that affects total player health. The value is set as in:\n* 0.3 means the player has 30% more health,\n* -0.5 means the player has 50% less health.")]
    public float HealthChange;

    [Tooltip("When this is equipped, should it hide the player's hair, using a Sprite Mask?\n(Only applies to head gear.)")]
    public bool HidesHair = false;

    public Item Item
    {
        get
        {
            if (_Item == null)
                _Item = GetComponent<Item>();

            return _Item;
        }
    }
    private Item _Item;

    public bool Equipped
    {
        get
        {
            return transform.parent != null;
        }
    }

    [HideInInspector]
    public List<SpriteRenderer> Renderers;

    public void Start()
    {
        Renderers = new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>());
    }

    public void Update()
    {
        // Note that the parent is set automatically from the player gear script.

        SetRendererLayer(Equipped ? Layer : "Dropped Items");

        Item.pickup.AllowPickup = !Equipped;
    }

    public void SetRendererLayer(string layer)
    {
        foreach (SpriteRenderer r in Renderers)
        {
            if(r.sortingLayerName != layer)
                if (r.gameObject.layer != 9)
                    r.sortingLayerName = layer;
        }
    }
}