﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    // An inventory with little to no physical structure, that has a limit of items.
    // The default limits are either:
    // 1. A maximum amount of items, or
    // 2. A maximum weight or possibly
    // 3. Both, a limit on items and weight.

    [Tooltip("The display name of the inventory.")]
    public string Name = "Default Name";

    [Tooltip("The owner of this inventory, and where items will be spawned when they are dropped.")]
    public Transform Owner;

    [Tooltip("The inventory item prefab.")]
    public GameObject InventoryItemPrefab;

    [Tooltip("The details text object rendererd at the top right of the screen.")]
    public Text Details;

    [Tooltip("The title of the inventory.")]
    public Text Title;

    [Tooltip("The details sub-screen.")]
    public InventoryDetailsView DetailsView;

    [Tooltip("The parent of the item game objects.")]
    public Transform ItemsParent;

    [Tooltip("The content object in the viewport of a scrollable.")]
    public RectTransform ViewportContent;

    [Tooltip("The maximum amount of items in this inventory. Negaive numbers means that there is no limit.")]
    public int MaxItems = -1; // If negative, ignored.

    [Tooltip("The maximum combined weight of all items in this inventory. Negaitve numbers means that there is no limit.")]
    public float MaxWeight = 100f; // If negative, ignored.

    private List<InventoryItem> Contents = new List<InventoryItem>();
    private float weight;
    [HideInInspector] public ItemOptionsPanel Options;

    public void Start()
    {
        Options = GetComponentInChildren<ItemOptionsPanel>(true);
    }

    public void Update()
    {
        // TODO make less frequent.
        Details.text = "Items: " + Contents.Count + "/" + (MaxItems < 0 ? "---" : MaxItems.ToString()) + "\nWeight: " + weight + "/" +  (MaxWeight < 0 ? "---" : MaxWeight.ToString() + "Kg");

        if(Title.text != Name)
        {
            Title.text = Name;
        }
    }

    public void ResetViewport()
    {
        ViewportContent.localPosition = Vector2.zero;
    }

    public InventoryItem AddItem(Item item, int amount = 1)
    {
        if (CanAdd(item))
        {
            // Add this item to the inventory.
            // Make the InventoryItem prefab and set it up.
            // Also TODO configure the size of the inventory in UI.

            bool needNewRow = !item.CanStack;

            if (needNewRow || GetOfType(item.Prefab) == null)
            {
                GameObject spawned = Instantiate(InventoryItemPrefab, ItemsParent);
                RectTransform rect = spawned.GetComponent<RectTransform>();
                InventoryItem i = spawned.GetComponent<InventoryItem>();

                // For now, all items are separated and cannot be stacked: TODO.

                // Configure item.
                i.ItemPrefab = item.Prefab;
                i.Inventory = this;
                i.ItemCount = amount;

                // Place at bottom
                rect.localPosition = new Vector2(0, -Contents.Count * 30);

                i.Init();

                // Add item
                Contents.Add(i);

                // Add weight
                weight += i.Item.InventoryInfo.Weight;

                // Extend viewport
                ViewportContent.sizeDelta = new Vector2(0, Contents.Count * 30);

                return i;
            }
            else
            {
                InventoryItem stack = GetOfType(item.Prefab);
                stack.SetItemCount(stack.ItemCount + amount);

                // Add weight
                weight += stack.Item.InventoryInfo.Weight * amount;

                return stack;
            }
        }
        else
        {
            return null;
        }
    }

    public void RemoveItem(InventoryItem item, bool drop = true, int amount = 1)
    {
        int index = this.Contents.IndexOf(item);

        bool lastItem = Contents[index].ItemCount <= amount;
        if (lastItem)
        {
            Debug.Log("Last item!");
        }
        if(Contents[index].ItemCount < amount)
        {
            Debug.LogError("Cannot remove that many items : there are only " + Contents[index].ItemCount);
            // Do not return, just drop as many as we have!
            amount = Contents[index].ItemCount;
        }

        weight -= item.Item.InventoryInfo.Weight * amount;

        if(lastItem)
            this.Contents.Remove(item);

        // Spawn item on ground
        if (drop)
        {
            Vector3 position = Vector3.zero;
            if (Owner != null)
                position = Owner.position;

            // Create new instance...
            Player.Local.NetUtils.CmdSpawnDroppedItem(item.Item.Prefab, position);
        }

        if(!lastItem)
            Contents[index].SetItemCount(Contents[index].ItemCount - amount);

        if (!lastItem)
            return;
        Destroy(item.gameObject);

        // Move everything below this upwards, and make viewport smaller.
        for(int i = index; i < Contents.Count; i++)
        {
            RectTransform x = Contents[i].GetComponent<RectTransform>();
            x.localPosition = new Vector2(x.localPosition.x, x.localPosition.y + 30);
        }

        // Viewport
        ViewportContent.sizeDelta = new Vector2(0, ViewportContent.sizeDelta.y - 30);
    }

    private InventoryItem GetOfType(string prefab)
    {
        foreach (InventoryItem item in Contents)
        {
            if (item.ItemPrefab == prefab)
                return item;
        }
        return null;
    }

    public bool CanAdd(Item i)
    {
        return CanAddWeight(i) && CanAddCapacity(i);
    }

    public bool CanAddCapacity(Item i)
    {
        return this.CanAddCapacity(1); // Each item counts as 1 item! Shocking!
    }

    public bool CanAddCapacity(int extra)
    {
        if (MaxItems < 0)
            return true;

        return Contents.Count + extra <= MaxItems;           
    }

    public bool CanAddWeight(Item item)
    {
        return this.CanAddWeight(item.InventoryInfo.Weight);
    }

    public bool CanAddWeight(float weight)
    {
        if (MaxWeight < 0)
            return true;
        return this.weight + weight <= MaxWeight;
    }

    public float GetCombinedWeight()
    {
        float weight = 0;

        foreach(InventoryItem i in Contents)
        {
            weight += i.Item.InventoryInfo.Weight * i.ItemCount;
        }

        return weight;
    }
}