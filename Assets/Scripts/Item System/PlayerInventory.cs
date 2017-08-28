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
        if (InputManager.InputDown("Inventory"))
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

    public static void Add(Item item)
    {
        inv.Inventory.AddItem(item);
    }

    public static void Remove(InventoryItem item)
    {
        inv.Inventory.RemoveItem(item);
    }

    public static void Open()
    {
        inv.Inventory.gameObject.SetActive(true);
        inv.Inventory.ResetViewport();
    }

    public static void Close()
    {
        inv.Inventory.gameObject.SetActive(false);
        inv.Inventory.Options.Close();
    }
}
