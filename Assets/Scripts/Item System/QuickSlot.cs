using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class QuickSlot : NetworkBehaviour
{
    public KeyCode[] Slots;

    public void Update()
    {
        if (!isLocalPlayer)
            return;
        if (!InputManager.Active)
            return;

        for (int i = 0; i < Slots.Length; i++)
        {
            if (Input.GetKeyDown(Slots[i]))
            {
                Debug.Log("Pressed " + Slots[i]);
                QuickSelect(i + 1);
            }
        }
    }

    /// <summary>
    /// Selects the item for the local player. Note that the number is from 0 to 3.
    /// </summary>
    /// <param name="number"></param>
    public void QuickSelect(int number)
    {
        if (!isLocalPlayer)
            return;


        foreach (InventoryItem i in PlayerInventory.inv.Inventory.Contents)
        {
            if (i.Data == null)
                continue;
            if (i.Data.QuickSlot == number)
            {
                Item.Option_Equip(i);
                break;
            }
        }
    }
}
