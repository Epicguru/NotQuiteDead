using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class TeamTransfer : Command
{
    public TeamTransfer()
    {
        Name = "team_transfer";
        parameters.Add(typeof(string));
        parameters.Add(typeof(string));
    }

    public override string Execute(object[] args)
    {
        if (!Player.Local.isServer)
            return "You are not the host! Cannot run team commands!";
        string newTeamName = args[0] as string;
        string nextTeam = args[1] as string;

        if (string.IsNullOrEmpty(newTeamName))
        {
            return "Team name cannot be null or empty!";
        }
        if (!Teams.I.TeamExists(newTeamName))
        {
            return "Team '" + newTeamName + "' does not exist!";
        }
        if (Teams.I.TeamEmpty(newTeamName))
        {
            return "Team '" + newTeamName + "' is empty! No players to move.";
        }
        if (string.IsNullOrEmpty(nextTeam.Trim()))
        {
            return "Name of team to transfer to is empty!";
        }

        int count = 0;
        List<string> names = new List<string>();
        foreach(Player p in Teams.I.GetTeam(newTeamName))
        {
            names.Add(p.Name);
            count++;
        }

        foreach(string p in names)
        {
            Teams.I.GetPlayer(p).Team = nextTeam; // This only works because we are server.
        }

        CommandProcessing.Log("Moved " + count + " players from team '" + newTeamName + "' to team '" + nextTeam + "'.");
        return null;
    }
}
