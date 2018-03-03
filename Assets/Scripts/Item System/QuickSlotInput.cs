using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class QuickSlotInput : MonoBehaviour
{
    public bool Open
    {
        get
        {
            return _Open;
        }
        set
        {
            if(value != _Open)
            {
                if (value)
                {
                    SetText();
                    gameObject.SetActive(true);
                }
                else
                {
                    gameObject.SetActive(false);
                }
                _Open = value;
            }
        }
    }
    private bool _Open;

    public Text Text;
    public QuickSlotSelectEvent SelectedEvent = new QuickSlotSelectEvent();

    public void SetText()
    {
        Text.text = "Press a quick slot key to bind\nPress " + InputManager.GetInput("Escape") + " to cancel.";
    }

    public void Update()
    {
        if (Open)
        {
            if (InputManager.InputDown("Escape", true))
            {
                SelectedEvent.RemoveAllListeners();
                Open = false;
                return;
            }

            for (int i = 0; i < Player.Local.QuickSlot.Slots.Length; i++)
            {
                if (Input.GetKeyDown(Player.Local.QuickSlot.Slots[i]))
                {
                    InventoryItemData item = null;

                    foreach(InventoryItemData x in PlayerInventory.inv.Inventory.Contents)
                    {
                        if (x.Data == null)
                            continue;
                        if(x.Data.Get("Quick Slot", -1) == i + 1)
                        {
                            item = x;
                            break;
                        }
                    }

                    if(item == null)
                    {
                        if(Player.Local != null)
                        {
                            if(Player.Local.Holding.Item != null)
                            {
                                if(Player.Local.Holding.Item.Data.Get("Quick Slot", -1) == i + 1)
                                {
                                    Player.Local.Holding.Item.Data.Update("Quick Slot", 0);
                                }
                            }
                        }
                    }

                    if (item != null)
                    {
                        item.Data.Update("Quick Slot", 0);
                        PlayerInventory.inv.Inventory.Refresh = true;
                    }

                    SelectedEvent.Invoke(i + 1);
                    SelectedEvent.RemoveAllListeners();
                    Open = false;
                    break;
                }
            }
        }
    }
}

[System.Serializable]
public class QuickSlotSelectEvent : UnityEvent<int>
{

}