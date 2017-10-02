using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class TeamAdd : Command
{
    public TeamAdd()
    {
        Name = "team_add";
        parameters.Add(typeof(string));
    }

    public override string Execute(object[] args)
    {
        if (!Player.Local.isServer)
            return "You are not the host! Cannot run team commands!";
        string newTeamName = args[0] as string;

        if (string.IsNullOrEmpty(newTeamName))
        {
            return "Team name cannot be null or empty!";
        }
        if (Teams.I.TeamExists(newTeamName))
        {
            return "Team '" + newTeamName + "' already exists!";
        }

        Teams.I.AddTeam(newTeamName);
        CommandProcessing.Log("Added new team: '" + newTeamName + "'. There are now " + Teams.I.GetTeamNames().Count + " teams.");
        return null;
    }
}
