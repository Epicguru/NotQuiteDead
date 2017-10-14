using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class BlueprintLoader
{
    public static Blueprint GetBlueprint(string text)
    {
        // Basic validation.
        if (string.IsNullOrEmpty(text.Trim()))
        {
            Debug.LogError("Text for blueprint creation is null!");
            return null;
        }

        text = text.Trim();
        text = text.Replace("\n", "");

        if (!text.Contains(':'))
        {
            Debug.LogError("Incorrect blueprint structure: No ':' symbol.");
            return null;
        }

        string[] parts = text.Split(':');
        if (parts.Length != 2)
        {
            Debug.LogError("Incorrect blueprint structure: Too many parts, caused by repeat ':' symbols. (" + parts.Length + " parts)");
            return null;
        }

        for (int i = 0; i < parts.Length; i++)
        {
            parts[i] = parts[i].Trim();
        }

        // Create blueprint object.
        Blueprint b = new Blueprint();

        // Get products. (Results)
        string rawProducts = parts[0];

        if (string.IsNullOrEmpty(rawProducts.Trim()))
        {
            Debug.LogError("Invalid blueprint: No products.");
            return null;
        }

        // Loop through products.
        string[] products = rawProducts.Split(';');
        List<Item> PI = new List<Item>();
        List<int> PC = new List<int>();
        foreach(string p in products)
        {
            Item i = null;
            int count = -1;

            GetPairValues(p.Trim(), out i, out count);

            if(i != null && (count != -1 && count != 0))
            {
                PI.Add(i);
                PC.Add(count);
            }
        }

        b.Products = PI.ToArray();
        b.Quantities = PC.ToArray();

        PI.Clear();
        PC.Clear();

        // Get requirements. (Crafting materials)
        string rawRequirements = parts[1];

        if (string.IsNullOrEmpty(rawRequirements.Trim()))
        {
            Debug.LogError("Invalid blueprint: No requirements.");
            return null;
        }

        // Loop through requirements.
        string[] requirements = rawRequirements.Split(';');
        foreach (string r in requirements)
        {
            Item i = null;
            int count = -1;

            GetPairValues(r.Trim(), out i, out count);

            if (i != null && (count != -1 && count != 0))
            {
                PI.Add(i);
                PC.Add(count);
            }
        }

        b.Requirements = PI.ToArray();
        b.RequirementQuantities = PC.ToArray();

        PI.Clear();
        PC.Clear();

        return b;
    }

    public static void GetPairValues(string pair, out Item item, out int count)
    {
        item = GetItem(pair);
        count = GetItemCount(pair);
    }

    public static Item GetItem(string itemCountPair)
    {
        if (string.IsNullOrEmpty(itemCountPair.Trim()))
        {
            // Do not announce, its more of a quite error.
            return null;
        }

        string itemString = itemCountPair.Trim();

        if (itemCountPair.Contains(','))
        {
            itemString = itemString.Split(',')[0].Trim();
        }

        bool itemExists = Item.ItemExists(itemString);
        if (!itemExists)
        {
            Debug.LogError("Invalid blueprint: Item '" + itemString + "' does not exist! (Check spelling and capitalisation!)");
            return null;
        }
        else
        {
            return Item.FindItem(itemString);
        }
    }

    public static int GetItemCount(string itemCountPair)
    {
        if (string.IsNullOrEmpty(itemCountPair.Trim()))
        {
            // Do not announce, its more of a quite error.
            return 0;
        }

        if (!itemCountPair.Contains(','))
        {
            return 1;
        }
        else
        {
            string[] parts = itemCountPair.Split(',');
            parts[0] = parts[0].Trim();
            if(parts.Length != 2)
            {
                Debug.LogError("Incorrect blueprint structure: Too many item-count pair parts! (Should be 'ItemName, ItemCount') (" + parts.Length + " parts)");
                return 0;
            }
            else
            {
                string number = parts[1].Trim();
                if (string.IsNullOrEmpty(number))
                {
                    Debug.LogError("Incorrect blueprint structure: No item number specified for '" + parts[0] + "'. Remove the comma or add a number to fix this.");
                    return 0;
                }
                int count = -1;
                bool worked = int.TryParse(number, out count);

                if (!worked)
                {
                    Debug.LogError("Invalid blueprint: '" + number + "' is not a valid item count for '" + parts[0] + "'.");
                    return 0;
                }
                else
                {
                    return count;
                }
            }
        }
    }
}