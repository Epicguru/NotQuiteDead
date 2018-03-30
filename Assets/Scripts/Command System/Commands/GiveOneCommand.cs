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
        itemName = itemName.Trim();
        int amount = 1;

        if (itemName.ToLower() == "all")
        {
            // Give all items.
            foreach(var x in Item.Items.Values)
            {
                Player.Local.NetUtils.CmdTryGive(x.Prefab, amount, null);
            }

            // Give all tiles.
            foreach(var x in BaseTile.GetAllTiles())
            {
                Player.Local.BuildingInventory.AddItems(x, 1000);
            }

            // Give all furnitures
            foreach (var x in Furniture.Loaded.Values)
            {
                Player.Local.BuildingInventory.AddItems(x, 1000);
            }

            CommandProcessing.Log("Gave local player 1 of each and every item, and 1000 of all buildables.");

            return null;
        }

        Item prefab = Item.GetItem(itemName);

        if (prefab == null)
            return "Could not find item: '" + itemName + "'";

        Player.Local.NetUtils.CmdTryGive(prefab.Prefab, amount, null);
        CommandProcessing.Log("Gave local player '" + prefab.Name + "' x" + amount);

        return null;
    }
}
