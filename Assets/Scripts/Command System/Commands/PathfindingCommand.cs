using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class PathfindingCommand : Command
{
    public PathfindingCommand()
    {
        base.Name = "pathfinding";
        base.parameters.Add(typeof(string));
    }

    public override string Execute(object[] args)
    {
        string arg = ((string)(args[0])).Trim().ToLower();

        if(arg == "stats")
        {
            CommandProcessing.Log("Max open nodes: " + Pathfinding.MAX);
            CommandProcessing.Log("Max pending requests: " + PathfindingManager.MAX_PENDING);
            CommandProcessing.Log(PathfindingManager.GetPendingIDs());
            return null;
        }

        if (arg == "wipe")
        {
            CommandProcessing.Log("Wiping: " + PathfindingManager.GetPending().Count + " pending requests.");
            PathfindingManager.DissolveAllPending();
            return null;
        }

        return "Error: Unknown pathing sub-command '" + arg + "'. Valid commands are 'stats' and 'wipe'";
    }
}
