
using Newtonsoft.Json;
using System;

[Serializable]
[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
public class InventoryItemData
{
    public string Prefab;
    public int Count;
    public ItemData Data;

    [JsonIgnore]
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