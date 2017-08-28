using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        int amount = (int)args[1];

        if (amount <= 0)
            return "Amount of items must be more than zero!";

        Item prefab = Item.FindItem("Items/" + itemName);

        if(prefab == null)
        {
            return "Could not find item: '" + "Items/" + itemName + "'";
        }

        PlayerInventory.Add(prefab, amount);
        CommandProcessing.Log("Gave local player '" + prefab.Name + " x" + amount);

        return null;
    }
}
