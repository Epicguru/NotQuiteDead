
using System;

[Serializable]
public class InventoryItemData
{
    public string Prefab;
    public int Count;
    public ItemData Data;

    public Item Item
    {
        get
        {
            if (_Item == null)
                _Item = Item.GetItem(Prefab);

            return _Item;
        }
    }

    private Item _Item;
}