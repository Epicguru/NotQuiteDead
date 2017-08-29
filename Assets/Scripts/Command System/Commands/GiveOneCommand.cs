using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GiveOneCommand : Command
{
    public GiveOneCommand()
    {
        Name = "give";
        parameters.Add(typeof(string));
    }

    public override string Execute(object[] args)
    {
        // Give items

        string itemName = (string)args[0];
        int amount = 1;

        if (amount <= 0)
            return "Amount of items must be more than zero!";

        Item prefab = Item.FindItem("Items/" + itemName);

        if(prefab == null)
        {
            // Look in /Guns
            prefab = Item.FindItem("Items/Guns/" + itemName);
        }
        if (prefab == null)
        {
            // Look in /Melee
            prefab = Item.FindItem("Items/Melee/" + itemName);
        }
        if (prefab == null)
        {
            // Look in /Placeables
            prefab = Item.FindItem("Items/Placeable/" + itemName);
        }

        if(prefab == null)
            return "Could not find item: '" + itemName + "' in Items/*";

        PlayerInventory.Add(prefab, amount);
        CommandProcessing.Log("Gave local player '" + prefab.Name + "' x" + amount);

        return null;
    }
}
