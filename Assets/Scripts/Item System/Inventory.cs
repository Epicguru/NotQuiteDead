using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Inventory : NetworkBehaviour
{
    // An inventory holds items.
    // It can hold an unlimited amount of items for now.
    // It is completely server authorative and clients (players) all have inventories that are sent states
    // when necessary.

    public string Name = "Inventory Name";
    public int Capacity = 100;
    public Dictionary<string, List<ItemStack>> Contents = new Dictionary<string, List<ItemStack>>();

    public int ContentCount { get; private set; }

    public bool IsFull
    {
        get
        {
            return ContentCount >= Capacity;
        }
    }

    public bool Dirty
    {
        get
        {
            return dirty;
        }
        set
        {
            if (!isServer)
            {
                Debug.LogError("Cannot set dirty flag on client!");
                return;
            }
            dirty = value;
        }
    }
    private bool dirty;

    /// <summary>
    /// Converts the value of the contents to a json format.
    /// </summary>
    /// <returns></returns>
    [Server]
    public string GetJsonContents()
    {
        return JsonConvert.SerializeObject(Contents, Formatting.None);
    }

    public bool CanFit(int extra)
    {
        return (ContentCount + extra) <= Capacity;
    }

    public void Add(string prefab, int count, ItemData data)
    {
        if (string.IsNullOrWhiteSpace(prefab))
        {
            Debug.LogWarning("Null, empty or whitespace prefab! Cannot add to inventory '{0}'!".Form(Name));
            return;
        }
        if(count <= 0)
        {
            Debug.LogWarning("Tried to add {0}x {1} to inventory '{2}'! That's just stupid!".Form(count, prefab, Name));
            return;
        }
        if(!CanAdd(prefab, count, data))
        {
            Debug.LogWarning("Cannot add {0}x {1} to the inventory '{2}'!".Form(count, prefab, Name));
            return;
        }

        // Get the item prefab
        Item prefabItem = Item.GetItem(prefab);
        if(prefabItem == null)
        {
            Debug.LogWarning("No item prefab was found for '{0}', cannot add to inventory '{1}'!".Form(prefab, Name));
            return;
        }

        // Is it stackable?
        if (prefabItem.CanStack)
        {
            // It can stack, see if we have a stack for that item...
            if (Contents.ContainsKey(prefab))
            {
                // Great add the amount to the stack.
                // Data is not used in stackable items.
                Contents[prefab][0].Count += count;
                ContentCount += count;
            }
            else
            {
                // We need to create a new stack, there is not one in the inventory!
                // Again, data is not used with stackable objects.
                ItemStack stack = new ItemStack(prefab, count, null);
                var list = new List<ItemStack>();
                list.Add(stack);
                Contents.Add(prefab, list);
                ContentCount += count;
            }
        }
        else
        {
            // The item cannot be stacked, we will always need to create and add a new stack for the item.
            // Adding more that one item like this is invalid, because they need item data.
            // Even if item data is null, you can't add more than one at a time. Just call the method multiple times.

            if(count > 1)
            {
                Debug.LogError("Cannot add more that one non-stackable item at a time: Just call the method in a loop. Tried to add {0} to inventory '{1}', will only add one.".Form(count, Name));
            }

            // Make stack
            ItemStack stack = new ItemStack(prefab, 1, data);

            // Do we contain a set of stacks?
            if (Contents.ContainsKey(prefab))
            {
                Contents[prefab].Add(stack);
            }
            else
            {
                var list = new List<ItemStack>();
                list.Add(stack);
                Contents.Add(prefab, list);
            }
            ContentCount += 1;
        }
    }

    public bool Contains(string prefab)
    {
        return Contents.ContainsKey(prefab);
    }

    public bool Contains(string prefab, int count)
    {
        if (count <= 0)
            return false;

        int amount = GetAmountOf(prefab);

        return amount >= count;
    }

    public int GetAmountOf(string prefab)
    {
        if (!Contents.ContainsKey(prefab))        
            return 0;

        List<ItemStack> stackList = Contents[prefab];
        if (stackList == null)
            return 0;

        int count = 0;
        foreach (var stack in stackList)
        {
            count += stack.Count;
        }

        return count;
    }

    public bool Remove(string prefab, int count, out int removed, out ItemData data)
    {
        if(count <= 0)
        {
            Debug.LogWarning("Tried to remove {0}x {1} from inventory '{2}'. Why?".Form(count, prefab, Name));
            removed = 0;
            data = null;
            return false;
        }
        if (!Contains(prefab))
        {
            Debug.LogWarning("Cannot remove any {0}(s), none in this '{1}' inventory.".Form(prefab, Name));
            removed = 0;
            data = null;
            return false;
        }

        Item item = Item.GetItem(prefab);
        if(item == null)
        {
            Debug.LogWarning("No item prefab found for '{0}', cannot remove any from the '{1}' inventory!".Form(prefab, Name));
            removed = 0;
            data = null;
            return false;
        }

        if (item.CanStack)
        {
            // Is only in one stack, and we definitely have a stack because of the checks above...
            int stored = Contents[prefab][0].Count;

            // More than enough...
            if(stored > count)
            {
                Contents[prefab][0].Count -= count;
                ContentCount -= count;

                removed = count;
                data = null; // Item data is now allowed for stackables.
                return true;
            }
            // Just enough...
            if(stored == count)
            {
                // Remove the stack list from the inventory completely, because it's count value will be 0 anyway.
                Contents[prefab][0].Count = 0; // Not really necessary...
                Contents.Remove(prefab);
                ContentCount -= count;

                removed = count;
                data = null; // Item data is now allowed for stackables.
                return true;
            }
            // Not enough... Just remove as many as we can!
            if(stored < count)
            {
                // Remove as many as possible.
                Contents[prefab][0].Count = 0; // Not really necessary...
                Contents.Remove(prefab);

                removed = stored;
                data = null; // Item data is now allowed for stackables.
                return true;
            }

            // Will never reach here...
            // Stupid compiler doesn't realise that I did covered all possible options above.
            removed = 0;
            data = null;
            return false;
        }
        else
        {
            if(count != 1)
            {
                Debug.LogWarning("Tried to remove {0} non-stackable items ({1}) from inventory '{2}'. Can only remove one at a time! Only one will be removed. Use a loop!".Form(count, prefab, Name));
                count = 1;
            }

            // The item cannot stack so we need to go through the stack list.
            List<ItemStack> stacks = Contents[prefab];

            // Do a little check, not really necessary...
            if(stacks == null || stacks.Count == 0)
            {
                // Should never happen!
                Debug.LogError("Null stack list (for non-stackable item {1}) in inventory '{0}'".Form(Name, prefab));
                removed = 0;
                data = null;
                return false;
            }

            // Get the first stack from the list of stacks (Heap maybe? Heap: A stack of stacks... Hmmm...)
            ItemStack stack = stacks[0];

            if(stack == null)
            {
                // Should never happen!
                Debug.LogError("Null stack (for non-stackable item {1}) in inventory '{0}'".Form(Name, prefab));
                removed = 0;
                data = null;
                return false;
            }

            // Get data for this item...
            ItemData d = stack.Data;
            // Ignore stack count value: Should always be one.
            int r = 1;

            // Remove the stack from the heap.
            stacks.Remove(stack);

            data = d;
            removed = 1;
            return true;
        }
    }

    public virtual bool CanAdd(string prefab, int count, ItemData data)
    {
        if (IsFull)
            return false;

        return CanFit(count);
    }
}