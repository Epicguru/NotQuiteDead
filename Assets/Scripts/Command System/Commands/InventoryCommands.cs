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

                int items = Player.Local.Inventory.ContentCount;
                Player.Local.Inventory.Clear();

                CommandProcessing.Log("Cleared " + items + " items out of the player's inventory.");
                return null;

            default:
                return "Not a valid action. Valid actions are: clear.";
        }
    }
}
