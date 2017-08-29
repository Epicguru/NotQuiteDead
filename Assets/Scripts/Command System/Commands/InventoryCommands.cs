using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class InventoryCommands : Command
{
    public InventoryCommands()
    {
        Name = "inventory";
        parameters.Add(typeof(string));
    }

    public override string Execute(object[] args)
    {
        string action = (string)args[0];

        switch (action)
        {
            case "clear":

                int items = PlayerInventory.inv.Inventory.Contents.Count;
                PlayerInventory.inv.Inventory.Clear();

                CommandProcessing.Log("Cleared " + items + " items out of the player's inventory.");
                return null;
            case "drop":
                items = PlayerInventory.inv.Inventory.Contents.Count;
                PlayerInventory.inv.Inventory.DropAll(2f, Player.Local.transform.position);

                CommandProcessing.Log("Dropped " + items + " items from the player's inventory.");
                return null;
            default:
                return "Not a valid action. Valid actions are: clear, drop.";
        }
    }
}
