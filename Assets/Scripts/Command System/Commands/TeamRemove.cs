using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class TeamRemove : Command
{
    public TeamRemove()
    {
        Name = "team_remove";
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
        if (!Teams.I.TeamExists(newTeamName))
        {
            return "Team '" + newTeamName + "' does not exist!";
        }
        if (!Teams.I.TeamEmpty(newTeamName))
        {
            return "Team '" + newTeamName + "' is not empty!";
        }

        Teams.I.RemoveTeam(newTeamName);
        CommandProcessing.Log("Removed team: '" + newTeamName + "'. There are now " + Teams.I.GetTeamNames().Count + " teams.");
        return null;
    }
}
