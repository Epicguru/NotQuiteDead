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

    public void OnDestroy()
    {
        if (inv == this)
            inv = null;
    }

    public void Update()
    {
        if (UI.AnyOpen)
        {
            Close();
            return;
        }

        if (!Inventory.QSI.Open && !Inventory.Options.isActiveAndEnabled && InputManager.InputDown("Inventory", true))
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
        if(!Inventory.QSI.Open && InputManager.InputDown("Escape", true))
        {
            if (Inventory.DetailsView.isActiveAndEnabled)
            {
                // Close the details view.
                Inventory.DetailsView.Exit();
            }
            else if (Inventory.Options.isActiveAndEnabled)
            {
                // Close options panel.
                Inventory.Options.Close();
            }
            else
            {
                if (inv.Inventory.gameObject.activeSelf)
                {
                    // Close
                    Close();
                }
            }
        }
    }

    public static void Add(string prefab, ItemData data, int amount)
    {
        inv.Inventory.AddItem(prefab, data, amount);
    }

    public static int Remove(string prefab, Vector2 position, bool drop = false, int amount = 1)
    {
        return inv.Inventory.RemoveItem(prefab, position, drop, amount);
    }

    public static void Open()
    {
        if (UI.AnyOpen)
            return;

        inv.Inventory.gameObject.SetActive(true);
        inv.Inventory.Open();
    }

    public static void Close()
    {
        inv.Inventory.gameObject.SetActive(false);
        inv.Inventory.Options.Close();
        inv.Inventory.QSI.Open = false;
    }
}
