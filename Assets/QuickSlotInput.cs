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
                Open = false;
                SelectedEvent.RemoveAllListeners();
                return;
            }

            for (int i = 0; i < Player.Local.QuickSlot.Slots.Length; i++)
            {
                if (Input.GetKeyDown(Player.Local.QuickSlot.Slots[i]))
                {
                    SelectedEvent.Invoke(i);
                    Open = false;
                    SelectedEvent.RemoveAllListeners();
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