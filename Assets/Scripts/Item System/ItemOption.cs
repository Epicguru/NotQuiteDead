using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Events;

public class ItemOption
{
    public string OptionName;
    public UnityAction<InventoryItem> OnSelected;

    public void Clicked(InventoryItem item)
    {
        OnSelected.Invoke(item);
    }
}
