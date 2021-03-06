﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

public class ItemOption
{
    public string OptionName;
    public object[] Params;
    public UnityAction<InventoryItemData, string> OnSelected;
    public InventoryItemData InvItem;
    /// <summary>
    /// Used when the InvItem cannot be set, such as in gear items.
    /// </summary>
    public string Prefab;

    public void Clicked()
    {
        if(InvItem != null)
        {
            if(Prefab == null)
            {
                Prefab = InvItem.Prefab;
            }
        }
        else
        {
            if(Prefab == null)
            {
                Debug.LogError("InvItem AND prefab string are null!");
            }
        }
        OnSelected.Invoke(InvItem, Prefab);
    }
}
