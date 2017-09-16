using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class NameCommand : Command
{
    public NameCommand()
    {
        Name = "name";
        parameters.Add(typeof(string));
    }

    public override string Execute(object[] args)
    {
        string name = args[0] as string;

        if(name.Trim() == string.Empty)
        {
            return "Name cannot be blank!";
        }
        if (Player.Local.PlayerHasName(name))
        {
            return "Another player already has that name, or a very similar one!";
        }

        Player.Local.NetUtils.CmdSetName(name);
        CommandProcessing.Log("Set player name to '" + name + "'");

        return null;
    }
}
