using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class RemoteCommand : Command
{
    public RemoteCommand()
    {
        Name = "remote";
        parameters.Add(typeof(string));
        parameters.Add(typeof(string));
    }

    public override string Execute(object[] args)
    {
        string target = args[0] as string;

        Player player = Player.Local.GetPlayer(target);
        if(player == null)
        {
            return "Did not find that player...";
        }

        string command = args[1] as string;

        // Assume it works!
        Player.Local.NetUtils.CmdRemoteCommand(player.gameObject, command);

        CommandProcessing.Log("Executing remote command '" + command + "' on '" + player.Name + "'.");

        return null;
    }
}
