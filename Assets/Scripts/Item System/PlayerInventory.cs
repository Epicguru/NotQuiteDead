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

    public void Awake()
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
        if(InputManager.InputDown("Escape", true))
        {
            if (inv.Inventory.gameObject.activeSelf)
            {
                // Close
                Close();
            }
        }
    }

    public static void Add(Item item, ItemData data, int amount)
    {
        inv.Inventory.AddItem(item.Prefab, data, amount);
    }

    public static void Add(string item, ItemData data, int amount)
    {
        inv.Inventory.AddItem(item, data, amount);
    }

    public static void Remove(Item item, int amount, bool drop = false)
    {
        if (item.CanStack)
        {
            inv.Inventory.RemoveItem(inv.Inventory.GetOfType(item.Prefab), Vector2.zero, drop, amount);
        }
        else
        {
            for (int i = 0; i < amount; i++)
            {
                InventoryItem x = inv.Inventory.GetOfType(item.Prefab);
                if (x != null)
                    inv.Inventory.RemoveItem(x, Vector2.zero, drop, 1);
                else
                    break;
            }
        }
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
        inv.Inventory.QSI.Open = false;
    }
}
