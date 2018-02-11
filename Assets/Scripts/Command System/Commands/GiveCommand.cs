using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GiveCommand : Command
{
    public GiveCommand()
    {
        Name = "give";
        parameters.Add(typeof(string));
        parameters.Add(typeof(int));
    }

    public override string Execute(object[] args)
    {
        // Give items

        string itemName = (string)args[0];
        itemName = itemName.Trim();
        int amount = (int)args[1];

        if (amount <= 0)
            return "Amount of items must be more than zero!";        

        Item prefab = Item.GetItem(itemName);

        if (prefab == null)
            return "Could not find item: '" + itemName + "'";

        PlayerInventory.Add(itemName, null, amount);

        CommandProcessing.Log("Gave local player '" + prefab.Name + "' x" + amount);

        return null;
    }
}