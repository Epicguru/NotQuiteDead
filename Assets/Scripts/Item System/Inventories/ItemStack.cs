
using Newtonsoft.Json;
using System;

[Serializable]
[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
public class ItemStack
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
    [JsonIgnore]
    private Item _Item;

    public ItemStack()
    {

    }

    public ItemStack(string prefab, int count, ItemData data)
    {
        Prefab = prefab;
        Count = count;
        Data = data;
    }
}