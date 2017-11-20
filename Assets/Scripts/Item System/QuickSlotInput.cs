using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    public QuickSlotSelectEvent SelectedEvent = new QuickSlotSelectEvent();

    public void Update()
    {
        if (Open)
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                SelectedEvent.RemoveAllListeners();
                Open = false;
                return;
            }

            //if (Input.GetKeyUp(KeyCode.Space))
            //{
            //    SelectedEvent.Invoke(0);
            //    SelectedEvent.RemoveAllListeners();
            //    Open = false;
            //    return;
            //}

            for (int i = 0; i < Player.Local.QuickSlot.Slots.Length; i++)
            {
                if (Input.GetKeyDown(Player.Local.QuickSlot.Slots[i]))
                {
                    InventoryItem item = null;

                    foreach(InventoryItem x in PlayerInventory.inv.Inventory.Contents)
                    {
                        if (x.Data == null)
                            continue;
                        if(x.Data.QuickSlot == i + 1)
                        {
                            item = x;
                            break;
                        }
                    }

                    if (item != null)
                    {
                        item.Data.QuickSlot = 0;
                        item.SetText();
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