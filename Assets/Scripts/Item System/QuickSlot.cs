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
        if (PlayerInventory.inv.Inventory.isActiveAndEnabled)
            return;
        if (Player.Local != null)
            if (Player.Local.Building.InBuildMode)
                return;

        for (int i = 0; i < Slots.Length; i++)
        {
            if (Input.GetKeyDown(Slots[i]))
            {
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


        foreach (InventoryItemData i in PlayerInventory.inv.Inventory.Contents)
        {
            if (i.Data == null)
                continue;
            if (i.Data.ContainsKey("Quick Slot") && i.Data.Get<int>("Quick Slot") == number)
            {
                Item.Option_Equip(i, i.Prefab);
                break;
            }
        }
    }
}
