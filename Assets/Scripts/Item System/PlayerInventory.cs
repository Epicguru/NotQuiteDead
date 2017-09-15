using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public Inventory Inventory;
    public static PlayerInventory inv;
    public static bool IsOpen
    {
        get
        {
            return inv.Inventory.isActiveAndEnabled;
        }
    }

    public void Start()
    {
        inv = this;
    }

    public void Update()
    {
        if (InputManager.InputDown("Inventory", true))
        {
            if (inv.Inventory.gameObject.activeSelf)
            {
                // Close
                Close();
            }
            else
            {
                // Open
                Open();
            }
        }
    }

    public static void Add(string prefab, ItemData data, int amount = 1)
    {
        inv.Inventory.AddItem(prefab, data, amount);
    }

    public static void Remove(InventoryItem item)
    {
        inv.Inventory.RemoveItem(item, Player.Local.transform.position);
    }

    public static void Open()
    {
        inv.Inventory.gameObject.SetActive(true);
        inv.Inventory.ResetViewport();
        InputManager.Active = false;
    }

    public static void Close()
    {
        inv.Inventory.gameObject.SetActive(false);
        inv.Inventory.Options.Close();
        InputManager.Active = true;
    }
}
