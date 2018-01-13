using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class PathfindingCommand : Command
{
    public PathfindingCommand()
    {
        //base.
    }

    public override string Execute(object[] args)
    {
        string arg = (string)args[0];

        if(arg.Trim().ToLower() == "stats")
        {
            CommandProcessing.Log("Max open nodes: " + Pathfinding.MAX);
            CommandProcessing.Log("Max pending requests: " + PathfindingManager.MAX_PENDING);
            CommandProcessing.Log(PathfindingManager.GetPendingIDs());
            return null;
        }

        return "Error: Unknown sub-command '" + arg + "'";
    }
}
