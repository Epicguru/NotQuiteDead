using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class ItemData
{
    public static Dictionary<int, ItemData> data = new Dictionary<int, ItemData>();

    private static StringBuilder builder = new StringBuilder();
    private int ID;

    public virtual void Init(int ID)
    {
        this.ID = ID;
    }

    public virtual void Apply()
    {
        // Commit the whole object to be sent to the server and other clients.
    }

    public static void Add(ItemData itemData)
    {
        int ID = GenID(data.Count);
        while (data.ContainsKey(ID))
        {
            ID = GenID(data.Count);
        }

        itemData.Init(ID);
        data.Add(itemData.ID, itemData);
    }

    public static void Remove(ItemData itemData)
    {
        data.Remove(itemData.ID);
    }

    public static void Remove(int ID)
    {
        data.Remove(ID);
    }

    public static ItemData Get(int ID)
    {
        return data[ID];
    }

    public static int GenID(int totalData)
    {
        // ID is total data count + 5 random number digit.
        // This means that when items are destroyed there is little chance of conflict.
        // TODO improve this.

        int start = totalData; // As in first data will be zero, second data will be one...
        int end = UnityEngine.Random.Range(0, 100000); // Max five digits.

        builder.Append(start);
        builder.Append(end);

        int id = int.Parse(builder.ToString());
        builder.Length = 0;

        return id;
    }
}
