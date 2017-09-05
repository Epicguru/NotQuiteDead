using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ItemsCommand : Command
{
    public ItemsCommand()
    {
        Name = "items";
        parameters.Add(typeof(string));
    }

    public override string Execute(object[] args)
    {
        string action = (string)args[0];

        switch (action)
        {
            case "list":

                foreach(Item i in Item.Items.Values)
                {
                    CommandProcessing.Log(i.Name);
                }

                return null;
            case "count":

                CommandProcessing.Log("There are " + (Item.Items == null ? 0 : Item.Items.Count) + " loaded items, and " + GameObject.FindObjectsOfType<Item>().Length + " instances of items.");

                return null;
            case "clear":

                Item[] items = GameObject.FindObjectsOfType<Item>();
                CommandProcessing.Log("Destroying " + items.Length + " items.");

                foreach(Item item in items)
                {
                    Player.Local.NetUtils.CmdDestroyItem(item.gameObject);
                }

                return null;
            default:

                return "Invalid action. Valid actions are list, count and clear.";
        }
    }
}
