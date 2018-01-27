
using System;

public class GiveBuildable : Command
{
    public GiveBuildable()
    {
        Name = "give_build";
        parameters.Add(typeof(string));
        parameters.Add(typeof(string));
        parameters.Add(typeof(int));
    }

    public override string Execute(object[] args)
    {
        string type = ((string)args[0]).Trim().ToLower();
        string prefab = ((string)args[1]).Trim();
        int count = (int)args[2];

        if (count <= 0)
            return "Count must be greater than 0";

        if (type == "tile")
        {
            Player.Local.BuildingInventory.AddItems(BaseTile.GetTile(prefab), count);
            return null;
        }
        else if(type == "furniture" || type == "furn")
        {
            Player.Local.BuildingInventory.AddItems(Furniture.GetFurniture(prefab), count);
            return null;
        }
        else
        {
            return "Invalid type param. Must be 'tile' or 'furniture'.";
        }
    }
}